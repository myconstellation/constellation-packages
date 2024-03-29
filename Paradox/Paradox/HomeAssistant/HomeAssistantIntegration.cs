﻿using Constellation.Package;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Paradox.Events;
using Paradox.HomeAssistant.DiscoveryConfig;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Paradox.HomeAssistant
{
    /// <summary>
    /// The HomeAssistant MQTT Alarm Panel integration
    /// </summary>
    public class HomeAssistantIntegration
    {
        private readonly ParadoxManager paradox = ParadoxManager.Instance;
        private readonly MqttDiscoveryConfigFactory configsFactory = new MqttDiscoveryConfigFactory();

        #region Singleton

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static HomeAssistantIntegration Instance { get; private set; }

        /// <summary>
        /// Starts the HA integration in a .NET task.
        /// </summary>
        /// <param name="configuration">The HA configuration.</param>
        public static void Start(HomeAssistantConfiguration configuration)
        {
            Instance = new HomeAssistantIntegration() { Configuration = configuration };
            Task.Run(() => Instance.Start());
        }

        #endregion

        /// <summary>
        /// Gets or sets the HA configuration.
        /// </summary>
        public HomeAssistantConfiguration Configuration { get; set; }

        /// <summary>
        /// Starts the integration.
        /// </summary>
        public async Task Start()
        {
            PackageHost.WriteInfo("Starting the HomeAssistant MQTT integration to mqtt://{0}:{1}", Configuration.Mqtt.Server, Configuration.Mqtt.Port);

            // Create the MQTT client
            var mqttFactory = new MqttFactory();
            using (var managedMqttClient = mqttFactory.CreateManagedMqttClient())
            {
                // Create the MQTT client options
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(Configuration.Mqtt.Server, Configuration.Mqtt.Port)
                    .WithClientId(Configuration.Mqtt.ClientId ?? Configuration.Identifier)
                    .WithCredentials(Configuration.Mqtt.Username, Configuration.Mqtt.Password)
                    .WithWillTopic(configsFactory.MqttAvailabilityTopic)
                    .WithWillPayload(AvailabilityState.Offline)
                    .Build();
                var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                    .WithClientOptions(mqttClientOptions)
                    .Build();
                // Start the MQTT client
                await managedMqttClient.StartAsync(managedMqttClientOptions);

                // On MQTT message received on MqttAlarmCommandTopic
                managedMqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var data = e.ApplicationMessage.ConvertPayloadToString().Split(';');
                    if (data[0] == HomeAssistantAlarmCommand.Disarm)
                    {
                        // Disarm
                        await this.paradox.AreaDisarm(Area.Area1, Configuration.Code ?? data[1]);
                    }
                    else if (Enum.TryParse<ArmingMode>(data[0], out var armingMode))
                    {
                        // Arm
                        await this.paradox.AreaArm(Area.Area1, armingMode, Configuration.Code ?? data[1]);
                    }
                };
                await managedMqttClient.SubscribeAsync(configsFactory.MqttAlarmCommandTopic);

                // Generate and publish configurations to HA
                PackageHost.WriteInfo("Publishing MQTT discovery configuration");
                foreach (var config in configsFactory.GetDiscoveryConfigs())
                {
                    await managedMqttClient.PublishDiscoveryDocument(config);
                }

                // Publish initial status
                await Task.Delay(1000);
                PackageHost.WriteInfo("Publishing Paradox states to MQTT");
                var constellationPackage = PackageHost.Package as Program;

                // Alarm states
                var areaInfo = constellationPackage.GetItem<AreaInfo>((int)Area.Area1);
                await managedMqttClient.EnqueueAsync(configsFactory.MqttAvailabilityTopic, AvailabilityState.Online, retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.MqttAlarmStateTopic, ToAlarmState(areaInfo), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Trouble), ToOnOff(areaInfo.HasTrouble), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Strobe), ToOnOff(areaInfo.Strobe), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.AreaThreat), ToOnOff(!areaInfo.IsReady), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.AlarmInMemory), ToOnOff(areaInfo.ZoneInMemory), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Programming), ToOnOff(areaInfo.IsInProgramming), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.ACFailure), BinaryState.Off, retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.BatteryFailure), BinaryState.Off, retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.BellAbsent), BinaryState.Off, retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.ClockTrouble), BinaryState.Off, retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.CombusFault), BinaryState.Off, retain: true);

                // Zone states
                foreach (var zone in Configuration.Zones)
                {
                    var zoneInfo = constellationPackage.GetItem<ZoneInfo>(zone.Id);
                    await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic(zone.Id), JsonConvert.SerializeObject(new
                    {
                        Tamper = ToOnOff(zoneInfo.IsTamper),
                        Status = ToOnOff(zoneInfo.IsOpen),
                    }), retain: true);
                    await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic(zone.Id, HomeAssistantZoneSensor.Alarm), ToOnOff(zoneInfo.InAlarm), retain: true);
                    await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic(zone.Id, HomeAssistantZoneSensor.Supervision), ToOnOff(!zoneInfo.SupervisionLost), retain: true);
                    if (zone.Fire)
                    {
                        await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic(zone.Id, HomeAssistantZoneSensor.FireAlarm), ToOnOff(zoneInfo.InFireAlarm), retain: true);
                    }
                    if (zone.Battery)
                    {
                        await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic(zone.Id, HomeAssistantZoneSensor.BatteryLow), ToOnOff(zoneInfo.LowBattery), retain: true);
                    }
                }

                // Attach to Paradox events
                this.SubscribeParadoxEvent(managedMqttClient);

                // Attach to Paradox events
                PackageHost.WriteInfo("HomeAssistant integration started!");

                // Run until the package is stopping
                SpinWait.SpinUntil(() => !PackageHost.IsRunning);
            }
        }

        private void SubscribeParadoxEvent(IManagedMqttClient managedMqttClient)
        {
            this.paradox.Interface.ConnectionStateChanged += async (s, e) => await managedMqttClient.EnqueueAsync(configsFactory.MqttAvailabilityTopic, this.paradox.Interface.IsConnected ? AvailabilityState.Online : AvailabilityState.Offline);

            this.paradox.AreaStatusChanged += async (s, e) =>
            {
                if (e.InAlarm)
                {
                    await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Triggered);
                }
                else if (e.Status == AreaStatus.Armed)
                {
                    await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.ArmedAway, false);
                }
                else if (e.Status == AreaStatus.ForceArmed || e.Status == AreaStatus.InstantArmed)
                {
                    await SetAlarmState(managedMqttClient, Configuration.AllowBypassMode ? HomeAssistantAlarmState.ArmedForced : HomeAssistantAlarmState.ArmedAway, false);
                }
                else if (e.Status == AreaStatus.StayArmed)
                {
                    await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.ArmedHome, false);
                }
                else if (e.Status == AreaStatus.Disarmed)
                {
                    await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Disarmed, false);
                }
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Trouble), ToOnOff(e.HasTrouble), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Strobe), ToOnOff(e.Strobe), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.AreaThreat), ToOnOff(!e.IsReady), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.AlarmInMemory), ToOnOff(e.ZoneInMemory), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Programming), ToOnOff(e.IsInProgramming), retain: true);
            };

            this.paradox.ZoneStatusChanged += async (s, e) =>
            {
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone), JsonConvert.SerializeObject(new
                {
                    Tamper = ToOnOff(e.Status == ZoneStatus.Open),
                    Status = ToOnOff(e.Status == ZoneStatus.Tampered),
                }), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone, HomeAssistantZoneSensor.Alarm), ToOnOff(e.InAlarm), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone, HomeAssistantZoneSensor.Supervision), ToOnOff(!e.SupervisionLost), retain: true);
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone, HomeAssistantZoneSensor.FireAlarm), ToOnOff(e.InFireAlarm));
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone, HomeAssistantZoneSensor.BatteryLow), ToOnOff(e.LowBattery));
            };

            this.paradox.EventManager.ZoneChanged += async (s, e) =>
            {
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone), JsonConvert.SerializeObject(new
                {
                    Tamper = ToOnOff(e.EventGroup == EventGroup.ZoneTampered),
                    Status = ToOnOff(e.EventGroup == EventGroup.ZoneOpen)
                }), retain: true);
                paradox.RequestArea(Area.Area1);
            };

            this.paradox.EventManager.ZoneInAlarm += async (s, e) => await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone, HomeAssistantZoneSensor.Alarm), BinaryState.On, retain: true);
            this.paradox.EventManager.ZoneAlarmRestored += async (s, e) =>  await managedMqttClient.EnqueueAsync(configsFactory.GetMqttZoneTopic((int)e.Zone, HomeAssistantZoneSensor.Alarm), BinaryState.Off, retain: true);

            this.paradox.EventManager.Arming += async (s, e) =>
            {
                // Force to refresh the area
                previousState = null;
                paradox.RequestArea(Area.Area1);
                await PublishUserInfo(managedMqttClient, e.UserId);
            };
            this.paradox.EventManager.Disarming += async (s, e) =>
            {
                await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Disarmed);
                await PublishUserInfo(managedMqttClient, e.UserId);
            };
            this.paradox.AreaDisarmed += async (s, e) =>
            {
                await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Disarmed);
            };

            this.paradox.EventManager.Status1Changed += async (s, e) =>
            {
                if (e.StatusType == Status1EventType.AudibleAlarm)
                {
                    await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Triggered);
                }
            };

            this.paradox.EventManager.SpecialEvent += async (s, e) =>
            {
                if (e.EventType == SpecialEventType.WinLoadInConnected || e.EventType == SpecialEventType.InstallerInProgramming)
                {
                    await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Programming), ToOnOff(true), retain: true);
                }
                else if (e.EventType == SpecialEventType.WinLoadOutDisconnected || e.EventType == SpecialEventType.InstallerOutOfProgramming)
                {
                    await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Programming), ToOnOff(false), retain: true);
                }
            };

            this.paradox.EventManager.Status2Changed += async (s, e) =>
            {
                switch (e.StatusType)
                {
                    case Status2EventType.Ready:
                        await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.AreaThreat), BinaryState.Off, retain: true);
                        break;
                    case Status2EventType.ExitDelay:
                        await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Arming);
                        break;
                    case Status2EventType.EntryDelay:
                        await SetAlarmState(managedMqttClient, HomeAssistantAlarmState.Pending);
                        break;
                    case Status2EventType.SystemInTrouble:
                        await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.Trouble), BinaryState.On, retain: true);
                        break;
                    case Status2EventType.AlarmInMemory:
                        await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(HomeAssistantAlarmSensor.AlarmInMemory), BinaryState.On, retain: true);
                        break;
                }
            };

            this.paradox.EventManager.Trouble += async (s, e) => await UpdateTrouble(managedMqttClient, e, BinaryState.On);
            this.paradox.EventManager.TroubleRestored += async (s, e) => await UpdateTrouble(managedMqttClient, e, BinaryState.Off);
            this.paradox.EventManager.ModuleTrouble += async (s, e) => await UpdateModuleTrouble(managedMqttClient, e, BinaryState.On);
            this.paradox.EventManager.ModuleTroubleRestored += async (s, e) => await UpdateModuleTrouble(managedMqttClient, e, BinaryState.Off);
        }

        private async Task PublishUserInfo(IManagedMqttClient managedMqttClient, int userId)
        {
            var constellationPackage = PackageHost.Package as Program;
            var userInfo = constellationPackage.GetItem<UserInfo>(userId);
            await managedMqttClient.EnqueueAsync(configsFactory.MqttAlarmAttributesTopic, JsonConvert.SerializeObject(new
            {
                LastUserIdAction = userInfo.Id,
                LastUserAction = userInfo.Name,
            }), retain: true);
        }

        private string previousState = null;
        private async Task SetAlarmState(IManagedMqttClient managedMqttClient, string alarmState, bool force = true)
        {
            if (!force)
            {
                // Do not update the alarm state when Entry or Exit delays occurs
                if (previousState == HomeAssistantAlarmState.Pending || previousState == HomeAssistantAlarmState.Arming) return;
            }
            await managedMqttClient.EnqueueAsync(configsFactory.MqttAlarmStateTopic, alarmState, retain: true);
            previousState = alarmState;
        }

        private async Task UpdateTrouble(IManagedMqttClient managedMqttClient, TroubleEventArgs e, string state)
        {
            var sensor = e.TroubleType == TroubleType.ACFailure ? HomeAssistantAlarmSensor.ACFailure :
                                         e.TroubleType == TroubleType.BatteryFailure ? HomeAssistantAlarmSensor.BatteryFailure :
                                         e.TroubleType == TroubleType.BellAbsent ? HomeAssistantAlarmSensor.BellAbsent :
                                         e.TroubleType == TroubleType.ClockTrouble ? HomeAssistantAlarmSensor.ClockTrouble : null;
            if (sensor != null)
            {
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(sensor), state, retain: true);
            }
        }

        private async Task UpdateModuleTrouble(IManagedMqttClient managedMqttClient, ModuleTroubleEventArgs e, string state)
        {
            var sensor = e.ModuleTroubleType == ModuleTroubleType.ACFailure ? HomeAssistantAlarmSensor.ACFailure :
                                         e.ModuleTroubleType == ModuleTroubleType.BatteryFailure ? HomeAssistantAlarmSensor.BatteryFailure :
                                         e.ModuleTroubleType == ModuleTroubleType.CombusFault ? HomeAssistantAlarmSensor.CombusFault : null;
            if (sensor != null)
            {
                await managedMqttClient.EnqueueAsync(configsFactory.GetMqttAlarmTopic(sensor), state, retain: true);
            }
        }

        private string ToOnOff(bool input)
        {
            return input ? BinaryState.On : BinaryState.Off;
        }

        private string ToAlarmState(AreaInfo areaInfo)
        {
            if (areaInfo.InAlarm)
            {
                return HomeAssistantAlarmState.Triggered;
            }
            else if (areaInfo.IsFullArmed)
            {
                return HomeAssistantAlarmState.ArmedAway;
            }
            else if (areaInfo.IsStayArmed)
            {
                return HomeAssistantAlarmState.ArmedHome;
            }
            else
            {
                return HomeAssistantAlarmState.Disarmed;
            }
        }
    }
}

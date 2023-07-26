using Constellation.Package;
using Paradox.HomeAssistant.DiscoveryConfig.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Paradox.HomeAssistant.DiscoveryConfig
{
    /// <summary>
    /// The factory to create the MQTT discovery configurations
    /// </summary>
    public class MqttDiscoveryConfigFactory
    {
        private const string CommandTemplate = "{{ action }};{{ code }}";
        private const string RemoteCode = "REMOTE_CODE";
        private const string ParadoxPrtModelName = "APR-PRT3";
        private const string StateTopicName = "state";

        /// <summary>
        /// Gets the Paradox integration identifier (default: Paradox).
        /// </summary>
        public string Identifier => HomeAssistantIntegration.Instance.Configuration.Identifier;

        /// <summary>
        /// Gets the MQTT topic prefix.
        /// </summary>
        public string MqttTopicPrefix => $"{nameof(Paradox)}/{Identifier}";

        /// <summary>
        /// Gets the MQTT alarm state topic.
        /// </summary>
        public string MqttAlarmStateTopic => $"{MqttTopicPrefix}/alarm";

        /// <summary>
        /// Gets the MQTT alarm command topic.
        /// </summary>
        public string MqttAlarmCommandTopic => $"{MqttAlarmStateTopic}/set";

        /// <summary>
        /// Gets the MQTT availability topic.
        /// </summary>
        public string MqttAvailabilityTopic => $"{MqttTopicPrefix}/lwt";

        /// <summary>
        /// Gets the MQTT zone sensor topic.
        /// </summary>
        /// <param name="zoneId">The zone identifier.</param>
        /// <param name="topic">The topic.</param>
        /// <returns></returns>
        public string GetMqttZoneTopic(int zoneId, string topic = StateTopicName)
        {
            return $"{MqttTopicPrefix}/zone{zoneId:000}/{topic}";
        }

        /// <summary>
        /// Gets the MQTT alarm sensor topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns></returns>
        public string GetMqttAlarmTopic(string topic)
        {
            return $"{MqttAlarmStateTopic}/{topic}";
        }

        /// <summary>
        /// Gets the discovery configurations.
        /// </summary>
        /// <returns>List of MqttDiscoveryConfig</returns>
        public List<MqttDiscoveryConfig> GetDiscoveryConfigs()
        {
            var configs = new List<MqttDiscoveryConfig>();

            // Add the Alarm Panel
            var alarmPanel = new MqttAlarmControlPanelDiscoveryConfig
            {
                Device = new MqttDiscoveryDevice
                {
                    Identifiers = new List<string> { this.Identifier },
                    Manufacturer = nameof(Paradox),
                    Model = ParadoxPrtModelName,
                    Name = HomeAssistantIntegration.Instance.Configuration.Label,
                },
                UniqueId = $"{this.Identifier}_alarm_panel",
                ObjectId = $"{this.Identifier}_alarm_panel",
                Name = HomeAssistantIntegration.Instance.Configuration.Label,
                Code = RemoteCode,
                CommandTemplate = CommandTemplate,
                StateTopic = MqttAlarmStateTopic,
                CommandTopic = MqttAlarmCommandTopic,
                AvailabilityTopic = MqttAvailabilityTopic,
                PayloadArmAway = HomeAssistantAlarmCommand.ArmAway,
                PayloadArmHome = HomeAssistantAlarmCommand.ArmHome,
                PayloadArmNight = HomeAssistantAlarmCommand.ArmHome,
                PayloadArmVacation = HomeAssistantAlarmCommand.ArmAway,
                PayloadArmCustomBypass = HomeAssistantAlarmCommand.ArmBypass,
                PayloadDisarm = HomeAssistantAlarmCommand.Disarm,
                Qos = 1
            };
            configs.Add(alarmPanel);
            // Add the alarm's sensors
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.Trouble, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.Area, HomeAssistantBinarySensorClass.Safety, HomeAssistantEntityCategory.Diagnostic, icon: "mdi:shield-home"));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.AlarmInMemory, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic, label: "Alarm in memory"));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.Strobe, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.Programming, HomeAssistantBinarySensorClass.Lock, HomeAssistantEntityCategory.Diagnostic));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.ACFailure, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic, label: "AC", icon: "mdi:power-plug-outline"));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.BatteryFailure, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic, label: "Battery", icon: "mdi:battery-charging"));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.ClockTrouble, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic, label: "Clock", icon: "mdi:clock-check"));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.BellAbsent, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic, label: "Bell", icon: "mdi:bell-check"));
            configs.Add(CreateAlarmBinarySensorConfig(alarmPanel, HomeAssistantAlarmSensor.CombusFault, HomeAssistantBinarySensorClass.Problem, HomeAssistantEntityCategory.Diagnostic, label: "Combus", icon: "mdi:connection"));

            // Add Zones
            foreach (var zone in HomeAssistantIntegration.Instance.Configuration.Zones)
            {
                if (zone.Id > 0 && zone.Id <= 192 && !string.IsNullOrEmpty(zone.Label))
                {
                    var zoneDevice = new MqttDiscoveryDevice
                    {
                        Identifiers = new List<string> { $"{this.Identifier}:Zone{zone.Id:000}" },
                        Manufacturer = zone.Manufacturer ?? nameof(Paradox),
                        Model = zone.Model,
                        Name = zone.Label ?? (PackageHost.Package as Program)?.GetItem<ZoneInfo>(zone.Id)?.Name ?? $"Zone {zone.Id:000}",
                        ViaDevice = alarmPanel.Device.Identifiers.FirstOrDefault(),
                        SuggestedArea = zone.Area
                    };
                    configs.Add(CreateZoneBinarySensorConfig(zone, zoneDevice, HomeAssistantZoneSensor.Status, zone.Type, topic: StateTopicName));
                    configs.Add(CreateZoneBinarySensorConfig(zone, zoneDevice, HomeAssistantZoneSensor.Tamper, HomeAssistantBinarySensorClass.Tamper, topic: StateTopicName));
                    configs.Add(CreateZoneBinarySensorConfig(zone, zoneDevice, HomeAssistantZoneSensor.Alarm, HomeAssistantBinarySensorClass.Problem));
                    configs.Add(CreateZoneBinarySensorConfig(zone, zoneDevice, HomeAssistantZoneSensor.Supervision, HomeAssistantBinarySensorClass.Connectivity, HomeAssistantEntityCategory.Diagnostic));

                    if (zone.Fire)
                    {
                        configs.Add(CreateZoneBinarySensorConfig(zone, zoneDevice, HomeAssistantZoneSensor.FireAlarm, HomeAssistantBinarySensorClass.Smoke));
                    }
                    if (zone.Battery)
                    {
                        configs.Add(CreateZoneBinarySensorConfig(zone, zoneDevice, HomeAssistantZoneSensor.BatteryLow, HomeAssistantBinarySensorClass.Battery, HomeAssistantEntityCategory.Diagnostic));
                    }
                }
            }

            // Return the list
            return configs;
        }

        private MqttBinarySensorDiscoveryConfig CreateZoneBinarySensorConfig(HomeAssistantConfiguration.ZoneConfiguration zone, MqttDiscoveryDevice zoneDevice, string id, string className, string entityCategory = null, string label = null, string topic = null)
        {
            return new MqttBinarySensorDiscoveryConfig
            {
                UniqueId = $"{this.Identifier}_Zone{zone.Id:000}_{id}",
                ObjectId = $"{this.Identifier}_Zone{zone.Id:000}_{id}",
                Device = zoneDevice,
                Name = $"{zone.Label} {label ?? id}",
                EntityCategory = entityCategory,
                DeviceClass = className,
                StateTopic = this.GetMqttZoneTopic(zone.Id, topic ?? id),
                ValueTemplate = topic != null ? $"{{{{ value_json.{id}}}}}" : null,
                AvailabilityTopic = MqttAvailabilityTopic
            };
        }

        private MqttBinarySensorDiscoveryConfig CreateAlarmBinarySensorConfig(MqttAlarmControlPanelDiscoveryConfig alarmPanel, string id, string className, string entityCategory = null, string label = null, string topic = null, string icon = null)
        {
            return new MqttBinarySensorDiscoveryConfig
            {
                UniqueId = $"{alarmPanel.UniqueId}_{id}",
                ObjectId = $"{alarmPanel.ObjectId}_{id}",
                Device = alarmPanel.Device,
                Name = $"{alarmPanel.Name} {label ?? id}",
                EntityCategory = entityCategory,
                DeviceClass = className,
                Icon = icon,
                StateTopic = this.GetMqttAlarmTopic(topic ?? id),
                ValueTemplate = topic != null ? $"{{{{ value_json.{id}}}}}" : null,
                AvailabilityTopic = MqttAvailabilityTopic
            };
        }
    }
}

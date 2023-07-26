/*
 *	 Paradox connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
 *	
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *	
 *	 Unless required by applicable law or agreed to in writing,
 *	 software distributed under the License is distributed on an
 *	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *	 KIND, either express or implied. See the License for the
 *	 specific language governing permissions and limitations
 *	 under the License.
 */

namespace Paradox
{
    using Constellation;
    using Constellation.Package;
    using Paradox.Events;
    using Paradox.HomeAssistant;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Paradox Constellation package 
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private static object syncLock = new object();

        private ParadoxManager paradox = null;
        private MessageScope scopeParadox = null;
        private Dictionary<string, BaseParadoxItem> items = new Dictionary<string, BaseParadoxItem>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called before Shutdown the package (the Constellation connection is still open).
        /// </summary>
        public override void OnPreShutdown()
        {
            if (this.paradox != null && this.paradox.Interface.IsConnected)
            {
                this.paradox.Interface.Disconnect();
            }
        }

        /// <summary>
        /// Called when start the package.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Initializing Paradox package");

            // The group where send AlarmEvent message is the name of the package's instance
            this.scopeParadox = MessageScope.Create(MessageScope.ScopeType.Group, PackageHost.PackageInstanceName);

            // Create the Paradox API manager
            this.paradox = new ParadoxManager(PackageHost.GetSettingValue("PortCom"), PackageHost.GetSettingValue<int>("PortBaudRate"));

            // When the PRT3 connection changed
            bool connectionFail = false;
            this.paradox.Interface.ConnectionStateChanged += (s, e) =>
            {
                // Disconnection
                if (!this.paradox.Interface.IsConnected)
                {
                    if (connectionFail) // already fail ?
                    {
                        PackageHost.WriteInfo("Error unable to reconnect to the PRT3 Serial interface : shutdown package !");
                        PackageHost.Shutdown();
                    }
                    else // first disconnection, try to reconnect if still running
                    {
                        PackageHost.WriteWarn("Serial interface is disconnected" + (PackageHost.IsRunning ? " : trying to reconnect" : "."));
                        if (PackageHost.IsRunning)
                        {
                            connectionFail = true;
                            this.paradox.Interface.Connect();
                        }
                    }
                }
                // Reconnection
                else
                {
                    connectionFail = false;
                    PackageHost.WriteInfo("PRT3 Serial interface is reconnected : RefreshAll");
                    this.RefreshAll();
                }
                // Raise Constellation event
                this.scopeParadox.GetProxy().AlarmEvent(new { Type = "ConnectionStateChanged", State = this.paradox.Interface.IsConnected });
            };
            
            // Raise initial Constellation event
            this.scopeParadox.GetProxy().AlarmEvent(new { Type = "ConnectionStateChanged", State = this.paradox.Interface.IsConnected });

            // Refresh all when the package is reconnected to Constellation
            PackageHost.ConnectionStateChanged += (s, e) =>
            {
                if (PackageHost.IsConnected)
                {
                    PackageHost.WriteInfo("Package is reconnected : RefreshAll");
                    this.RefreshAll();
                }
            };

            // Interface error
            this.paradox.Interface.InterfaceError += (s, e) =>
            {
                // Write error
                PackageHost.WriteError("PRT3 Serial interface error : {0}", e.Exception.ToString());
                // Raise Constellation event
                this.scopeParadox.GetProxy().AlarmEvent(new { Type = "InterfaceError", Exception = e.Exception.Message });
            };

            // Message logging
            try
            {
                if (!string.IsNullOrEmpty(PackageHost.GetSettingValue("MessagesLogFilePath")) &&
                    !string.IsNullOrEmpty(Path.GetFileName(PackageHost.GetSettingValue("MessagesLogFilePath"))))
                {
                    BlockingCollection<string> logMessages = new BlockingCollection<string>();
                    this.paradox.Interface.MessageReceived += (s, e) => logMessages.Add(e.Date.ToString("dd/MM/yyyy HH:mm:ss.fff") + " < " + e.Message);
                    this.paradox.Interface.MessageSent += (s, e) => logMessages.Add(e.Date.ToString("dd/MM/yyyy HH:mm:ss.fff") + " > " + e.Message);
                    Task.Factory.StartNew(() =>
                    {
                        while (PackageHost.IsRunning)
                        {
                            foreach (var msg in logMessages.GetConsumingEnumerable())
                            {
                                try
                                {
                                    File.AppendAllText(
                                        PackageHost.GetSettingValue("MessagesLogFilePath").Replace(".log", "." + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                                        msg + Environment.NewLine);
                                }
                                catch { }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error with the Paradox message logging : {0}", ex.Message);
            }

            // Label received
            this.paradox.UserLabelReceived += (s, e) => this.PushItem<UserInfo>(e.UserId, o => o.UpdateName(e.Label));
            this.paradox.ZoneLabelReceived += (s, e) => this.PushItem<ZoneInfo>((int)e.Zone, o => o.UpdateName(e.Label));
            this.paradox.AreaLabelReceived += (s, e) => this.PushItem<AreaInfo>((int)e.Area, o => o.UpdateName(e.Label));

            // Area Status changed
            this.paradox.AreaStatusChanged += (s, e) => this.PushItem<AreaInfo>((int)e.Area, o =>
            {
                o.HasTrouble = e.HasTrouble;
                o.InAlarm = e.InAlarm;
                o.IsInProgramming = e.IsInProgramming;
                o.IsFullArmed = (e.Status == AreaStatus.Armed) || (e.Status == AreaStatus.ForceArmed);
                o.IsReady = e.IsReady;
                o.IsStayArmed = e.Status == AreaStatus.StayArmed;
                o.LastActivity = e.MessageDate;
                o.Strobe = e.Strobe;
            });

            // Zone Status changed
            this.paradox.ZoneStatusChanged += (s, e) => this.PushItem<ZoneInfo>((int)e.Zone, o =>
            {
                o.InAlarm = e.InAlarm;
                o.InFireAlarm = e.InFireAlarm;
                o.IsOpen = e.Status == ZoneStatus.Open;
                o.IsTamper = e.Status == ZoneStatus.Tampered;
                o.LowBattery = e.LowBattery;
                o.SupervisionLost = e.SupervisionLost;
                o.LastActivity = e.MessageDate;
            });

            // Zone changed
            this.paradox.EventManager.ZoneChanged += (s, e) => this.PushItem<ZoneInfo>((int)e.Zone, o =>
            {
                o.IsOpen = e.EventGroup == EventGroup.ZoneOpen;
                o.IsTamper = e.EventGroup == EventGroup.ZoneTampered;
                o.LastActivity = e.MessageDate;
            });

            // Zone in alarm
            this.paradox.EventManager.ZoneInAlarm += (s, e) => this.PushItem<ZoneInfo>((int)e.Zone, o =>
            {
                o.InAlarm = true;
                o.LastActivity = e.MessageDate;
            });
            this.paradox.EventManager.ZoneAlarmRestored += (s, e) => this.PushItem<ZoneInfo>((int)e.Zone, o =>
            {
                o.InAlarm = false;
                o.LastActivity = e.MessageDate;
            });

            // Log all Paradox system events to Constellation
            this.paradox.SystemEventReceived += (s, e) => PackageHost.WriteInfo("SystemEventReceived: {0}", e.ToString());

            // User enter code on keypad
            this.paradox.EventManager.UserCodeEnteredOnKeypad += (s, e) =>
            {
                string userId = "UserInfo" + e.UserId;
                if (items.ContainsKey(userId))
                {
                    PackageHost.WriteInfo(items[userId].Name + " entered code on keyboard ");
                }
                this.PushItem<UserInfo>(e.UserId, o => o.LastActivity = e.MessageDate);
            };

            // Status changed
            this.paradox.EventManager.Status1Changed += (s, e) =>
            {
                if (e.StatusType == Status1EventType.Armed || e.StatusType == Status1EventType.ForceArmed || e.StatusType == Status1EventType.InstantArmed || e.StatusType == Status1EventType.StayArmed)
                {
                    this.scopeParadox.GetProxy().AlarmEvent(new { Type = "Armed", Status = e.StatusType.ToString(), Date = e.MessageDate, Text = e.MessageText });
                }
                else if (e.StatusType == Status1EventType.AudibleAlarm || e.StatusType == Status1EventType.SilentAlarm || e.StatusType == Status1EventType.StrobeAlarm)
                {
                    this.RefreshArea(e.Area);
                    this.scopeParadox.GetProxy().AlarmEvent(new { Type = "Alarm", Status = e.StatusType.ToString(), Date = e.MessageDate, Text = e.MessageText });
                }
                PackageHost.PushStateObject("Status1", new { Status = e.StatusType.ToString() });
            };
            this.paradox.EventManager.Status2Changed += (s, e) => PackageHost.PushStateObject("Status2", new { Status = e.StatusType.ToString() });

            // Arming & disarming events
            this.paradox.EventManager.Arming += (s, e) =>
            {
                this.RefreshArea(e.Area);
                this.scopeParadox.GetProxy().AlarmEvent(new { Type = "Arming", User = items["UserInfo" + e.UserId], Date = e.MessageDate, Text = e.MessageText });
            };
            this.paradox.EventManager.Disarming += (s, e) =>
            {
                this.RefreshArea(e.Area);
                this.scopeParadox.GetProxy().AlarmEvent(new { Type = "Disarming", User = items["UserInfo" + e.UserId], Date = e.MessageDate, Text = e.MessageText });
            };

            // Other events
            this.paradox.EventManager.UserCodeEnteredOnKeypad += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "UserCodeEnteredOnKeypad", User = items["UserInfo" + e.UserId], Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.SpecialArming += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "SpecialArming", ArmingType = e.ArmingType.ToString(), Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.AccessDenied += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "AccessDenied", User = items["UserInfo" + e.UserId], Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.AccessGranted += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "AccessGranted", User = items["UserInfo" + e.UserId], Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.AlarmCancelled += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "AlarmCancelled", User = items["UserInfo" + e.UserId], Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.NonReportableEvent += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "NonReportableEvent", Event = e.Event.ToString(), Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.ZoneInAlarm += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "ZoneInAlarm", Zone = items["ZoneInfo" + (int)e.Zone], Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.ZoneAlarmRestored += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "ZoneAlarmRestored", Zone = items["ZoneInfo" + (int)e.Zone], Date = e.MessageDate, Text = e.MessageText });
            this.paradox.EventManager.ZoneChanged += (s, e) => this.scopeParadox.GetProxy().AlarmEvent(new { Type = "ZoneChanged", Zone = items["ZoneInfo" + (int)e.Zone], Date = e.MessageDate, Text = e.MessageText });

            // Refresh all
            this.RefreshAll();

            // HomeAssistant integration
            var config = PackageHost.GetSettingAsJsonObject<HomeAssistantConfiguration>("HomeAssistant", true);
            if (config != null && config.Enable && config.Mqtt != null)
            {
                HomeAssistantIntegration.Start(config);
            }

            // Initialization OK
            PackageHost.WriteInfo("Paradox System is loaded");
        }

        /// <summary>
        /// Arms the area.
        /// </summary>
        /// <param name="request">The arming request.</param>
        [MessageCallback]
        public bool AreaArm(ArmingRequestData request)
        {
            return this.DoSafeAction(() =>
            {
                PackageHost.WriteInfo("Arming area {0} with mode {1}", request.Area, request.Mode);
                return this.paradox.AreaArm((Area)request.Area, request.Mode, request.PinCode).Result;
            });
        }

        /// <summary>
        /// Disarms the area.
        /// </summary>
        /// <param name="request">The disarming request.</param>
        [MessageCallback]
        public bool AreaDisarm(ArmingRequestData request)
        {
            return this.DoSafeAction(() =>
            {
                PackageHost.WriteInfo("Disarming area {0}", request.Area);
                return this.paradox.AreaDisarm((Area)request.Area, request.PinCode).Result;
            });
        }

        /// <summary>
        /// Refreshes the specified area status.
        /// </summary>
        /// <param name="area">The area.</param>
        [MessageCallback]
        public void RefreshArea(Area area)
        {
            PackageHost.WriteInfo("Refreshing area " + area.ToString());
            Thread.Sleep(1000);
            ParadoxManager.Instance.RequestArea(area);
        }

        /// <summary>
        /// Refreshes all items.
        /// </summary>
        [MessageCallback]
        public void RefreshAll()
        {
            PackageHost.WriteInfo("Refreshing Paradox system");
            try
            {
                for (int i = 1; i <= PackageHost.GetSettingValue<int>("numberofAreas"); i++)
                {
                    PackageHost.WriteInfo("Refreshing area " + i.ToString());
                    ParadoxManager.Instance.RequestArea((Area)i);
                    Thread.Sleep(10);
                    ParadoxManager.Instance.RequestAreaLabel((Area)i);
                    Thread.Sleep(10);
                }

                for (int i = 1; i <= PackageHost.GetSettingValue<int>("numberofUsers"); i++)
                {
                    PackageHost.WriteInfo("Refreshing User " + i.ToString());
                    ParadoxManager.Instance.RequestUserLabel(i);
                    Thread.Sleep(10);
                }

                for (int i = 1; i <= PackageHost.GetSettingValue<int>("numberofZones"); i++)
                {
                    PackageHost.WriteInfo("Refreshing zone " + i.ToString());
                    ParadoxManager.Instance.RequestZone((Zone)i);
                    Thread.Sleep(10);
                    ParadoxManager.Instance.RequestZoneLabel((Zone)i);
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error to RefreshAll : " + ex.ToString());
            }
        }

        internal TItem GetItem<TItem>(int id) where TItem : BaseParadoxItem
        {
            string identifiant = typeof(TItem).Name + id;
            if (items.ContainsKey(identifiant))
            {
                return this.items[identifiant] as TItem;
            }
            return default;
        }

        private void PushItem<TItem>(int id, Action<TItem> updateAction = null) where TItem : BaseParadoxItem, new()
        {
            try
            {
                string identifiant = typeof(TItem).Name + id;
                TItem item = null;

                lock (syncLock)
                {
                    if (!items.ContainsKey(identifiant))
                    {
                        item = new TItem() { Id = id, LastActivity = DateTime.Now, Type = typeof(TItem).Name };
                        this.items.Add(identifiant, item);
                    }

                    item = this.items[identifiant] as TItem;
                    updateAction?.Invoke(item);
                }

                PackageHost.PushStateObject(identifiant, item, "Paradox." + typeof(TItem).Name, new Dictionary<string, object>()
                {
                    { "Id", id },
                    { "Type", typeof(TItem).Name }
                });
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error to push item : " + ex.ToString());
            }
        }

        #region Anti bruteforce

        private static int tries = 0;
        private static DateTime graceTime = DateTime.MinValue;

        private bool DoSafeAction(Func<bool> action)
        {
            try
            {
                if (DateTime.Now > graceTime)
                {
                    bool result = action();
                    if (result)
                    {
                        tries = 0;
                    }
                    else if(PackageHost.GetSettingValue<int>("AntiBruteForceMaxTries") > 0)
                    {
                        int maxTries = PackageHost.GetSettingValue<int>("AntiBruteForceMaxTries");
                        tries++;
                        if (tries - maxTries >= 0)
                        {
                            int lockTime = (tries - maxTries + 1) * 30;
                            graceTime = DateTime.Now.AddSeconds(lockTime);
                            PackageHost.WriteError("Action failed! Locked for {0} seconds", lockTime);
                        }
                        else
                        {
                            PackageHost.WriteWarn("Action failed! Remaining tries: {0}/{1}", maxTries - tries, maxTries);
                        }
                    }
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("An error has occurred : {0}", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }

        #endregion
    }
}

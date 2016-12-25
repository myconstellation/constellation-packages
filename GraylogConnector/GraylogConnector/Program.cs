/*
 *	 Graylog connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace GraylogConnector
{
    using Constellation.Control;
    using Constellation.Package;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Graylog connector
    /// </summary>
    public class Program : PackageBase
    {
        internal const int TRUE = 1, FALSE = 0;

        private List<Action<byte[]>> emitters = new List<Action<byte[]>>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Graylog connector starting ...");

            // Get configuration
            var configuration = PackageHost.GetSettingAsConfigurationSection<GraylogConfiguration>("graylogConfiguration", true);

            // Build the emitters
            foreach (GelfOutputElement output in configuration.GelfOutputs)
            {
                if (output.Enable)
                {
                    PackageHost.WriteInfo("Loading output '{0}' ({3}://{1}:{2})", output.Name, output.Host, output.Port, output.Protocol);

                    switch (output.Protocol)
                    {
                        case GelfOutputProtocol.Udp:
                            emitters.Add(new Action<byte[]>(async data =>
                            {
                                UdpClient udpClient = new UdpClient();
                                await udpClient.SendAsync(data, data.Length, output.Host, output.Port);
                                udpClient.Close();
                            }));
                            break;
                        case GelfOutputProtocol.Tcp:
                            // TCP not support !
                            PackageHost.WriteWarn("{0} will be ignored : GELF TCP output isn't supported in this version !", output.Name);
                            break;
                    }
                }
            }

            // Subscribe to StateObject update
            foreach (Subscription subscription in configuration.Subscriptions)
            {
                if (subscription.Enable)
                {
                    if (subscription.Aggregation != null && subscription.Aggregation.Count > 0)
                    {
                        new StateObjectAggregateSubscription((data) => this.SendData(data), subscription);
                    }
                    else
                    {
                        new StateObjectSubscription((data) => this.SendData(data), subscription);
                    }
                }
            }

            // ControlHub access ?
            if (PackageHost.HasControlManager)
            {
                // Attach to events
                PackageHost.ControlManager.PackageStateUpdated += ControlManager_PackageStateUpdated;
                PackageHost.ControlManager.SentinelUpdated += ControlManager_SentinelUpdated;
                PackageHost.ControlManager.LogEntryReceived += ControlManager_LogEntryReceived;
                
                // Subscribing to control groups
                if (configuration.SendPackageLogs)
                {
                    PackageHost.WriteInfo("Subscribing to package logs");
                    PackageHost.ControlManager.ReceivePackageLog = true;
                }
                if (configuration.SendSentinelUpdates)
                {
                    PackageHost.WriteInfo("Subscribing to sentinel updates");
                    PackageHost.ControlManager.ReceiveSentinelUpdates = true;
                }
                if (configuration.SendPackageStates)
                {
                    PackageHost.WriteInfo("Subscribing to package states");
                    PackageHost.ControlManager.ReceivePackageState = true;
                }
            }
            else if(configuration.SendPackageLogs || configuration.SendSentinelUpdates || configuration.SendPackageStates)
            {
                PackageHost.WriteError("Unable to access to the control manager ! The package can't subscribe to package logs, sentinel updates and package states. Please the credential used for this package.");
            }

            // Ready !
            PackageHost.WriteInfo("Graylog connector started!");
        }

        private void ControlManager_PackageStateUpdated(object sender, PackageStateEventArgs e)
        {
            // Prepare data
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "host", e.SentinelName },
                { "timestamp", Program.GetUnixTime(e.LastUpdate) },
                { "short_message", string.Format("{0}/{1} is {2}", e.SentinelName, e.PackageName, e.State.ToString()) },
                { "_package.isConnected", e.IsConnected ? TRUE : FALSE },
                { "_package.version",  e.PackageVersion },
                { "_package.state", e.State.ToString() },
                { "_package.name", e.PackageName },
                { "_package.connectionId", e.ConnectionId },
                { "_package.constellationClientVersion", e.ConstellationClientVersion }
            };
            // Send data
            this.SendData(data);
        }

        private void ControlManager_SentinelUpdated(object sender, SentinelEventArgs e)
        {
            // Prepare data
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "host", e.Sentinel.Description.SentinelName },
                { "timestamp", Program.GetUnixTime(e.Sentinel.RegistrationDate) },
                { "short_message",  string.Format("{0} is {1}connected", e.Sentinel.Description.SentinelName, e.Sentinel.IsConnected ? "" : "dis") },
                { "_sentinel.isConnected", e.Sentinel.IsConnected ? TRUE : FALSE },
                { "_sentinel.version", e.Sentinel.Description.Version },
                { "_sentinel.clrVersion", e.Sentinel.Description.CLRVersion },
                { "_sentinel.dnsHostname", e.Sentinel.Description.DnsHostName },
                { "_sentinel.machineName", e.Sentinel.Description.MachineName },
                { "_sentinel.osVersion", e.Sentinel.Description.OSVersion },
                { "_sentinel.platform", e.Sentinel.Description.Platform }
            };
            // Send data
            this.SendData(data);
        }

        private void ControlManager_LogEntryReceived(object sender, LogEntryEventArgs e)
        {
            int level = 6;
            // Get Syslog level code
            switch (e.LogEntry.Level)
            {
                case Constellation.LogLevel.Debug:
                    level = 7;
                    break;
                case Constellation.LogLevel.Error:
                    level = 3;
                    break;
                case Constellation.LogLevel.Fatal:
                    level = 0;
                    break;
                case Constellation.LogLevel.Info:
                    level = 6;
                    break;
                case Constellation.LogLevel.Warn:
                    level = 4;
                    break;
            }
            // Prepare log's data
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "host", e.LogEntry.SentinelName },
                { "timestamp", Program.GetUnixTime(e.LogEntry.Date) },
                { "short_message", e.LogEntry.Message },
                { "level", level },
                { "_package.name", e.LogEntry.PackageName}
            };
            // Send data
            this.SendData(data);
        }

        private void SendData(object data)
        {
            try
            {
                // Serialize data to JSON
                string json = JsonConvert.SerializeObject(data);
                // Get UTF8 bytes
                byte[] sendBytes = Encoding.UTF8.GetBytes(json + Environment.NewLine);
                // Send to emitters
                this.emitters.ForEach(emiter =>
                {
                    try
                    {
                        emiter(sendBytes);
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteDebug(ex.ToString());
                    }
                });
            }
            catch (Exception e)
            {
                PackageHost.WriteError(e.ToString());
            }
        }

        internal static double GetUnixTime(DateTime dt)
        {
            return (dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
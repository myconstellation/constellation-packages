/*
 *	 RFXCom Package for Constellation
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

namespace RfxCom
{
    using Constellation;
    using Constellation.Package;
    using Core;
    using Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Program : PackageBase
    {
        private Dictionary<string, string> stateObjectCustomNames = new Dictionary<string, string>();
        private RfxManager rfx = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Check PortName settings
            if (!PackageHost.ContainsSetting("PortName"))
            {
                PackageHost.WriteError("The setting 'PortName' is requiered to start the package !");
                return;
            }

            // Get the custom names
            this.stateObjectCustomNames = PackageHost.GetSettingAsJsonObject<Dictionary<string, string>>("StateObjectCustomNames");
            PackageHost.SettingsUpdated += (s, e) =>
            {
                // Refresh the dictionary on settings's update!
                this.stateObjectCustomNames = PackageHost.GetSettingAsJsonObject<Dictionary<string, string>>("StateObjectCustomNames");
            };

            // Init the RFX manager
            this.rfx = new RfxManager(PackageHost.GetSettingValue("PortName"));

            // Message handler for verbose mode
            this.rfx.OnMessage += (s, e) =>
            {
                if (PackageHost.GetSettingValue<bool>("Verbose"))
                {
                    PackageHost.WriteInfo(e.Message);
                }
            };

            // Forward message to group ?
            this.rfx.OnPacketReceived += (s, e) =>
            {
                if (PackageHost.ContainsSetting("ForwardRawMessageToGroup") && !string.IsNullOrEmpty(PackageHost.GetSettingValue("ForwardRawMessageToGroup")))
                {
                    PackageHost.CreateMessageProxy(MessageScope.ScopeType.Group, PackageHost.GetSettingValue("ForwardRawMessageToGroup")).MessageReceived(e.Packet);
                }
            };

            // Attach handler on receive packet
            this.rfx.Subscribe<InterfaceMessage>(p =>
            {
                PackageHost.WriteInfo($"RFXCOM device ({p.TransceiverName}) connected on port '{this.rfx.RfxInterface.SerialPort.PortName}' with {p.Protocols.Count(i => i.Enabled)} protocol(s) enabled ({string.Join(", ", p.Protocols.Where(i => i.Enabled).Select(i => i.Name))})");
                PackageHost.PushStateObject("RFXCOM", new { p.TransceiverName, p.TypeName, p.Protocols, p.FirmwareVersion }, "RfxCom.Device");
            });
            this.rfx.Subscribe<TemperatureSensor>(p =>
            {
                // Push StateObject
                PackageHost.PushStateObject(this.GetStateObjectName("TemperatureSensor_" + p.SensorID), new
                {
                    p.SensorID,
                    p.Channel,
                    p.SequenceNumber,
                    p.BatteryLevel,
                    p.SignalLevel,
                    p.Temperature,
                    p.SubTypeName
                }, "RfxCom.TemperatureSensor", new Dictionary<string, object>() { ["Type"] = p.TypeName, ["RawData"] = BitConverter.ToString(p.RawData) }, PackageHost.GetSettingValue<int>("SensorStateObjectLifetime"));
            });
            this.rfx.Subscribe<TemperatureHumiditySensor>(p =>
            {
                // Push StateObject
                PackageHost.PushStateObject(this.GetStateObjectName("TemperatureHumiditySensor_" + p.SensorID), new
                {
                    p.SensorID,
                    p.Channel,
                    p.SequenceNumber,
                    p.BatteryLevel,
                    p.SignalLevel,
                    p.Temperature,
                    p.Humidity,
                    p.Status,
                    p.SubTypeName,
                }, "RfxCom.TemperatureHumiditySensor", new Dictionary<string, object>() { ["Type"] = p.TypeName, ["RawData"] = BitConverter.ToString(p.RawData) }, PackageHost.GetSettingValue<int>("SensorStateObjectLifetime"));
            });

            // Starting !
            var protocolsEnabled = PackageHost.ContainsSetting("ProtocolsEnabled") ? PackageHost.GetSettingValue("ProtocolsEnabled").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray() : null;
            this.rfx.Connect(protocolsEnabled);
            PackageHost.WriteInfo("The RFXcom package is started!");
        }

        /// <summary>
        /// Refresh the RFXCOM device status.
        /// </summary>
        [MessageCallback]
        public async void RefreshStatus()
        {
            await this.rfx.GetStatus();
        }

        /// <summary>
        /// Sets the protocols enabled (the other will be disabled)
        /// </summary>
        /// <param name="protocolsEnabled">The protocols enabled.</param>
        [MessageCallback]
        public async void SetProtocols(string[] protocolsEnabled)
        {
            await this.rfx.SetProtocols(protocolsEnabled);
        }

        /// <summary>
        /// Sends the message with the RFXCOM device.
        /// </summary>
        /// <param name="hexMessage">The hexadecimal message.</param>
        [MessageCallback]
        public async void SendMessage(string hexMessage)
        {
            var data = Enumerable.Range(0, hexMessage.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(hexMessage.Substring(x, 2), 16))
                                 .ToArray();
            await this.rfx.SendMessage(data);
        }

        /// <summary>
        /// Sends the RFY command (for Somfy device).
        /// </summary>
        /// <param name="command">The RFY command.</param>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        /// <param name="id3">The id3.</param>
        /// <param name="unitcode">The unitcode.</param>
        [MessageCallback]
        public async void SendRFYCommand(RFYCommand command, int id1, int id2, int id3, int unitcode)
        {
            await this.rfx.SendMessage(new byte[] { 0x0c, 0x1A, 0x0, 0x02, (byte)id1, (byte)id2, (byte)id3, (byte)unitcode, (byte)command, 0x0, 0x0, 0x0, 0x0 });
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            PackageHost.WriteInfo("Disconnecting to rfxcom device ...");
            this.rfx?.Disconnect();
        }

        /// <summary>
        /// Gets the custom name of the state object if set in the settings package.
        /// </summary>
        /// <param name="soName">Name of the  state object.</param>
        /// <returns></returns>
        private string GetStateObjectName(string soName)
        {
            return (this.stateObjectCustomNames != null && this.stateObjectCustomNames.ContainsKey(soName)) ? this.stateObjectCustomNames[soName] : soName;
        }

    }
}

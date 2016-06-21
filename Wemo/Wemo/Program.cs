/*
 *	 Wemo Package for Constellation
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

namespace Wemo
{
    using Constellation.Package;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Wemo Package for Constellation
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private WemoScanner scanner = new WemoScanner();
        private Dictionary<string, WemoSwitch> switchDevices = new Dictionary<string, WemoSwitch>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Init the Wemo scanner           
            this.scanner.WemoDeviceFound += Scanner_WemoDeviceFound;
            // Discovery loop
            Thread discoveryThread = new Thread(new ThreadStart(() =>
            {
                int top = PackageHost.GetSettingValue<int>("DiscoveryInterval");
                while (PackageHost.IsRunning)
                {
                    if (top++ >= PackageHost.GetSettingValue<int>("DiscoveryInterval"))
                    {
                        top = 1;
                        this.Discover();
                    }
                    Thread.Sleep(1000);
                }
            }));
            // DeviceQuery loop
            Thread queryDeviceThread = new Thread(new ThreadStart(() =>
            {
                int top = PackageHost.GetSettingValue<int>("DeviceQueryInterval");
                while (PackageHost.IsRunning)
                {
                    if (top++ >= PackageHost.GetSettingValue<int>("DeviceQueryInterval"))
                    {
                        top = 1;
                        foreach (var sw in switchDevices)
                        {
                            try
                            {
                                // Query device
                                sw.Value.QuerySwitchState();
                                // Push StateObject
                                PackageHost.PushStateObject<WemoDevice>(sw.Value.SerialNumber, sw.Value);
                            }
                            catch { }
                        }
                    }
                    Thread.Sleep(1000);
                }
            }));
            // Start background threads
            discoveryThread.Start();
            queryDeviceThread.Start();
            // Package started !
            PackageHost.WriteInfo("Wemo connector is started !");
        }

        /// <summary>
        /// Sets the state of the switch.
        /// </summary>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="state">The new state of the switch.</param>
        [MessageCallback]
        public void SetSwitchState(string serialNumber, bool state)
        {
            serialNumber = serialNumber.ToUpper();
            if (switchDevices.ContainsKey(serialNumber))
            {
                switchDevices[serialNumber].SetSwitchState(state);
                PackageHost.WriteInfo($"Device {switchDevices[serialNumber].FriendlyName} ({serialNumber}) is switch to {state}");
            }
            else
            {
                PackageHost.WriteWarn($"Device {serialNumber} not found !");
            }
        }

        /// <summary>
        /// Discover Wemo devices.
        /// </summary>
        [MessageCallback]
        public void Discover()
        {
            PackageHost.WriteInfo("Discovering network for Wemo devices ...");
            this.scanner.DiscoverWemoDevices();
        }

        private void Scanner_WemoDeviceFound(object sender, WemoScanner.WemoDeviceFoundEventArgs e)
        {
            // Wemo device found !
            PackageHost.WriteInfo("Found {0} ({1}) on {2}", e.Device.FriendlyName, e.Device.ModelDescription, e.Device.Location.Host);
            // Just support Wemo Switch & Insight for now - Bridge, Motion, LightSwitch or Maker devices are not currently supported !
            if (e.Device.DeviceType == "urn:Belkin:device:insight:1" || e.Device.DeviceType == "urn:Belkin:device:controllee:1")
            {
                WemoSwitch swDevice = e.Device.ChangeDeviceType<WemoSwitch>();
                try
                {
                    // Initial query state & icon
                    swDevice.IconUrl = e.Device.InvokeServiceAction("basicevent", "GetIconURL").InnerText;
                    swDevice.QuerySwitchState();
                }
                catch { }
                lock (switchDevices)
                {
                    switchDevices[e.Device.SerialNumber.ToUpper()] = swDevice;
                }
            }
            // Push any Wemo devices
            PackageHost.PushStateObject<WemoDevice>(e.Device.SerialNumber, e.Device);
        }
    }
}

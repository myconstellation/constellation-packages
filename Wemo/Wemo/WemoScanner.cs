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
    using OpenSource.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Xml;

    /// <summary>
    /// Represent Wemo network scanner
    /// </summary>
    public class WemoScanner
    {
        /// <summary>
        /// The belkin device URN
        /// </summary>
        private const string BELKIN_DEVICE_URN = "urn:Belkin:device-1-0";

        /// <summary>
        /// The UPnP sniffer
        /// </summary>
        private UPnPSearchSniffer sniffer = new UPnPSearchSniffer();

        /// <summary>
        /// The know devices
        /// </summary>
        private List<string> knowDevices = new List<string>();

        /// <summary>
        /// Occurs when Wemo device found.
        /// </summary>
        public event EventHandler<WemoDeviceFoundEventArgs> WemoDeviceFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="WemoScanner"/> class.
        /// </summary>
        public WemoScanner()
        {
            this.sniffer.OnPacket += this.sniffer_OnPacket;
        }

        /// <summary>
        /// Discovers the Wemo devices.
        /// </summary>
        public void DiscoverWemoDevices()
        {
            this.sniffer.Search("upnp:rootdevice");
        }
        
        private void sniffer_OnPacket(object sender, string Packet, IPEndPoint Local, IPEndPoint From)
        {
            Debug.WriteLine("* UPnP device on [{0}]", From);
            if (Packet.StartsWith("HTTP/1.1"))
            {
                // Get the location uri
                string location = "";
                int idx = Packet.IndexOf("\r\nLOCATION:");
                if (idx == -1) idx = Packet.IndexOf("\r\nusn:");
                if (idx > 0)
                {
                    int pos2 = Packet.IndexOf("\r\n", idx + 11);
                    location = Packet.Substring(idx + 11, pos2 - (idx + 11));
                }
                location = location.Trim();
                // Check device
                try
                {
                    var locationUri = new Uri(location);
                    using (var wc = new WebClient())
                    {
                        wc.DownloadStringCompleted += (s, e) =>
                        {
                            if (!e.Cancelled && e.Error == null && e.Result.Contains(BELKIN_DEVICE_URN)) // TODO: Parse upnp device data and determine device
                            {
                                lock (knowDevices)
                                {
                                    if (!knowDevices.Contains(location))
                                    {
                                        knowDevices.Add(location);
                                        // Reading the setup.xml
                                        XmlDocument deviceInfo = new XmlDocument();
                                        XmlNamespaceManager ns = new XmlNamespaceManager(deviceInfo.NameTable);
                                        ns.AddNamespace("x", BELKIN_DEVICE_URN);
                                        deviceInfo.LoadXml(e.Result);
                                        var device = new WemoDevice()
                                        {
                                            Location = locationUri,
                                            FriendlyName = deviceInfo.SelectSingleNode("//x:device/x:friendlyName", ns).InnerText,
                                            DeviceType = deviceInfo.SelectSingleNode("//x:device/x:deviceType", ns).InnerText,
                                            Manufacturer = deviceInfo.SelectSingleNode("//x:device/x:manufacturer", ns).InnerText,
                                            ModelDescription = deviceInfo.SelectSingleNode("//x:device/x:modelDescription", ns).InnerText,
                                            ModelName = deviceInfo.SelectSingleNode("//x:device/x:modelName", ns).InnerText,
                                            ModelNumber = deviceInfo.SelectSingleNode("//x:device/x:modelNumber", ns).InnerText,
                                            SerialNumber = deviceInfo.SelectSingleNode("//x:device/x:serialNumber", ns).InnerText,
                                            UDN = deviceInfo.SelectSingleNode("//x:device/x:UDN", ns).InnerText,
                                            UPC = deviceInfo.SelectSingleNode("//x:device/x:UPC", ns).InnerText,
                                            MacAddress = deviceInfo.SelectSingleNode("//x:device/x:macAddress", ns).InnerText,
                                            FirmwareVersion = deviceInfo.SelectSingleNode("//x:device/x:firmwareVersion", ns).InnerText,
                                        };
                                        // Raise event
                                        WemoDeviceFound?.Invoke(this, new WemoDeviceFoundEventArgs() {  Device = device });
                                    }
                                }
                            }
                        };
                        wc.DownloadStringAsync(locationUri);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Represent Wemo device
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class WemoDeviceFoundEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the Wemo device.
            /// </summary>
            /// <value>
            /// The Wemo device.
            /// </value>
            public WemoDevice Device { get; set; }
        }
    }
}

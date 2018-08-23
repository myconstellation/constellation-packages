/*
 *	 Huawei Mobile Router Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace HuaweiMobileRouter.Models
{
    using Constellation.Package;
    using System.Xml.Serialization;

    /// <summary>
    /// Router status
    /// </summary>
    [StateObject, XmlRoot(ElementName = "response")] //api/monitoring/status
    public class MonitoringStatus
    {
        [XmlElement(ElementName = "ConnectionStatus")]
        public NetworkStatus ConnectionStatus { get; set; }

        [XmlElement(ElementName = "WifiConnectionStatus")]
        public string WifiConnectionStatus { get; set; }

        [XmlElement(ElementName = "SignalStrength")]
        public string SignalStrength { get; set; }

        [XmlElement(ElementName = "SignalIcon")]
        public int SignalIcon { get; set; }

        [XmlElement(ElementName = "CurrentNetworkType")]
        public NetworkType CurrentNetworkType { get; set; }

        [XmlElement(ElementName = "CurrentServiceDomain")]
        public int CurrentServiceDomain { get; set; }

        [XmlElement(ElementName = "RoamingStatus")]
        public int RoamingStatus { get; set; }

        [XmlElement(ElementName = "BatteryStatus")]
        public string BatteryStatus { get; set; }

        [XmlElement(ElementName = "BatteryLevel")]
        public string BatteryLevel { get; set; }

        [XmlElement(ElementName = "BatteryPercent")]
        public string BatteryPercent { get; set; }

        [XmlElement(ElementName = "simlockStatus")]
        public int SimlockStatus { get; set; }

        [XmlElement(ElementName = "WanIPAddress")]
        public string WanIPAddress { get; set; }

        [XmlElement(ElementName = "WanIPv6Address")]
        public string WanIPv6Address { get; set; }

        [XmlElement(ElementName = "PrimaryDns")]
        public string PrimaryDns { get; set; }

        [XmlElement(ElementName = "SecondaryDns")]
        public string SecondaryDns { get; set; }

        [XmlElement(ElementName = "PrimaryIPv6Dns")]
        public string PrimaryIPv6Dns { get; set; }

        [XmlElement(ElementName = "SecondaryIPv6Dns")]
        public string SecondaryIPv6Dns { get; set; }

        [XmlElement(ElementName = "CurrentWifiUser")]
        public int CurrentWifiUser { get; set; }

        [XmlElement(ElementName = "TotalWifiUser")]
        public int TotalWifiUser { get; set; }

        [XmlElement(ElementName = "currenttotalwifiuser")]
        public int CurrentTotalwifiuser { get; set; }

        [XmlElement(ElementName = "ServiceStatus")]
        public int ServiceStatus { get; set; }

        [XmlElement(ElementName = "SimStatus")]
        public int SimStatus { get; set; }

        [XmlElement(ElementName = "WifiStatus")]
        public int WifiStatus { get; set; }

        [XmlElement(ElementName = "CurrentNetworkTypeEx")]
        public int CurrentNetworkTypeEx { get; set; }

        [XmlElement(ElementName = "maxsignal")]
        public int MaxSignal { get; set; }

        [XmlElement(ElementName = "wifiindooronly")]
        public int WifiIndoorOnly { get; set; }

        [XmlElement(ElementName = "wififrequence")]
        public int WifiFrequence { get; set; }

        [XmlElement(ElementName = "classify")]
        public string Classify { get; set; }

        [XmlElement(ElementName = "flymode")]
        public int Flymode { get; set; }

        [XmlElement(ElementName = "cellroam")]
        public int CellRoam { get; set; }

        [XmlElement(ElementName = "voice_busy")]
        public int VoiceBusy { get; set; }
    }


}

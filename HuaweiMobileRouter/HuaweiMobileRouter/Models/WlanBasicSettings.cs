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
    /// WiFi setup information
    /// </summary>
    [StateObject, XmlRoot(ElementName = "response")] //api/wlan/basic-settings
    public class WlanBasicSettings
    {
        [XmlElement(ElementName = "WifiSsid")]
        public string WifiSsid { get; set; }

        [XmlElement(ElementName = "WifiChannel")]
        public int WifiChannel { get; set; }

        [XmlElement(ElementName = "WifiHide")]
        public int WifiHide { get; set; }

        [XmlElement(ElementName = "WifiCountry")]
        public string WifiCountry { get; set; }

        [XmlElement(ElementName = "WifiMode")]
        public string WifiMode { get; set; }

        [XmlElement(ElementName = "WifiRate")]
        public int WifiRate { get; set; }

        [XmlElement(ElementName = "WifiTxPwrPcnt")]
        public int WifiTxPwrPcnt { get; set; }

        [XmlElement(ElementName = "WifiMaxAssoc")]
        public int WifiMaxAssoc { get; set; }

        [XmlElement(ElementName = "WifiEnable")]
        public int WifiEnable { get; set; }

        [XmlElement(ElementName = "WifiFrgThrshld")]
        public int WifiFrgThreshold { get; set; }

        [XmlElement(ElementName = "WifiRtsThrshld")]
        public int WifiRtsThreshold { get; set; }

        [XmlElement(ElementName = "WifiDtmIntvl")]
        public int WifiDtmInterval { get; set; }

        [XmlElement(ElementName = "WifiBcnIntvl")]
        public int WifiBcnInterval { get; set; }

        [XmlElement(ElementName = "WifiWme")]
        public int WifiWme { get; set; }

        [XmlElement(ElementName = "WifiPamode")]
        public int WifiPaMode { get; set; }

        [XmlElement(ElementName = "WifiIsolate")]
        public int WifiIsolate { get; set; }

        [XmlElement(ElementName = "WifiProtectionmode")]
        public int WifiProtectionMode { get; set; }

        [XmlElement(ElementName = "Wifioffenable")]
        public int WifiOffEnable { get; set; }

        [XmlElement(ElementName = "Wifiofftime")]
        public int WifiOffTime { get; set; }

        [XmlElement(ElementName = "wifibandwidth")]
        public int WifiBandwidth { get; set; }

        [XmlElement(ElementName = "wifiautocountryswitch")]
        public int WifiAutoCountrySwitch { get; set; }

        [XmlElement(ElementName = "wifiantennanum")]
        public int WifiAntennaNum { get; set; }

        [XmlElement(ElementName = "wifiguestofftime")]
        public int WifiGuestOffTime { get; set; }

        [XmlElement(ElementName = "WifiRestart")]
        public int WifiRestart { get; set; }
    }
}

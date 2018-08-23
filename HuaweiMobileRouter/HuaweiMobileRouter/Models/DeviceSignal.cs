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
    /// Cell ID and signal quality parameters: RSRQ, RSRP, RSSI, SINR, RSCP, Ec/Io
    /// </summary>
    [StateObject, XmlRoot(ElementName = "response")] //api/device/signal
    public class DeviceSignal
    {
        [XmlElement(ElementName = "pci")]
        public string Pci { get; set; }

        [XmlElement(ElementName = "sc")]
        public string Sc { get; set; }

        [XmlElement(ElementName = "cell_id")]
        public string CellId { get; set; }

        [XmlElement(ElementName = "rsrq")]
        public string Rsrq { get; set; }

        [XmlElement(ElementName = "rsrp")]
        public string Rsrp { get; set; }

        [XmlElement(ElementName = "rssi")]
        public string Rssi { get; set; }

        [XmlElement(ElementName = "sinr")]
        public string Sinr { get; set; }

        [XmlElement(ElementName = "rscp")]
        public string Rscp { get; set; }

        [XmlElement(ElementName = "ecio")]
        public string Ecio { get; set; }

        [XmlElement(ElementName = "mode")]
        public int Mode { get; set; }
    }
}

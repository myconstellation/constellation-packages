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
    /// Huawei device information
    /// </summary>
    [StateObject, XmlRoot(ElementName = "response")] //api/device/information
    public class DeviceInformation
    {
        [XmlElement(ElementName = "DeviceName")]
        public string DeviceName { get; set; }

        [XmlElement(ElementName = "SerialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// International Mobile Equipment Identity
        /// </summary>
        [XmlElement(ElementName = "Imei")]
        public string Imei { get; set; }

        /// <summary>
        /// International Mobile Subscriber Identity (IMSI)
        /// </summary>
        [XmlElement(ElementName = "Imsi")]
        public string Imsi { get; set; }

        /// <summary>
        /// Unique identifier of SIM Card.
        /// </summary>
        [XmlElement(ElementName = "Iccid")]
        public string Iccid { get; set; }

        /// <summary>
        /// Mobile Station ISDN Number
        /// </summary>
        [XmlElement(ElementName = "Msisdn")]
        public string Msisdn { get; set; }

        [XmlElement(ElementName = "HardwareVersion")]
        public string HardwareVersion { get; set; }

        [XmlElement(ElementName = "SoftwareVersion")]
        public string SoftwareVersion { get; set; }

        [XmlElement(ElementName = "WebUIVersion")]
        public string WebUIVersion { get; set; }

        [XmlElement(ElementName = "MacAddress1")]
        public string MacAddress1 { get; set; }

        [XmlElement(ElementName = "MacAddress2")]
        public string MacAddress2 { get; set; }

        [XmlElement(ElementName = "ProductFamily")]
        public string ProductFamily { get; set; }

        [XmlElement(ElementName = "Classify")]
        public string Classify { get; set; }

        [XmlElement(ElementName = "supportmode")]
        public string Supportmode { get; set; }

        [XmlElement(ElementName = "workmode")]
        public string Workmode { get; set; }
    }
}

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
    using System.Xml.Serialization;

    public enum NetworkStatus
    {
        Unknown = 0,
        [XmlEnum(Name = "2")]
        InvalidProfile,
        [XmlEnum(Name = "37")]
        NotAllowed,
        [XmlEnum(Name = "201")]
        BandwidthExceeded,
        [XmlEnum(Name = "900")]
        Connecting,
        [XmlEnum(Name = "901")]
        Connected,
        [XmlEnum(Name = "903")]
        Diconnecting,
        [XmlEnum(Name = "902")]
        Disconnected,
        [XmlEnum(Name = "905")]
        SignalPoor,

        //    2: "Connection failed, the profile is invalid",
        //    3: "Connection failed, the profile is invalid",
        //    5: "Connection failed, the profile is invalid",
        //    7: "Network access not allowed",
        //    8: "Connection failed, the profile is invalid",
        //    11: "Network access not allowed",
        //    12: "Connection failed, roaming not allowed",
        //    13: "Connection failed, roaming not allowed",
        //    14: "Network access not allowed",
        //    20: "Connection failed, the profile is invalid",
        //    21: "Connection failed, the profile is invalid",
        //    23: "Connection failed, the profile is invalid",
        //    27: "Connection failed, the profile is invalid",
        //    28: "Connection failed, the profile is invalid",
        //    29: "Connection failed, the profile is invalid",
        //    30: "Connection failed, the profile is invalid",
        //    31: "Connection failed, the profile is invalid",
        //    32: "Connection failed, the profile is invalid",
        //    33: "Connection failed, the profile is invalid",
    }
    public enum NetworkType
    {
        Unknown = 0,
        /// <summary>
        /// No service
        /// </summary>
        [XmlEnum(Name = "0")]
        NoService,
        /// <summary>
        /// Global System for Mobile Communications
        /// </summary>
        [XmlEnum(Name = "1")]
        GSM,
        /// <summary>
        /// General Packet Radio Service
        /// </summary>
        [XmlEnum(Name = "2")]
        GPRS,
        /// <summary>
        /// Enhanced Data Rates for GSM Evolution
        /// </summary>
        [XmlEnum(Name = "3")]
        EDGE,
        /// <summary>
        /// Wideband Code Division Multiple Access
        /// </summary>
        [XmlEnum(Name = "4")]
        WCDMA,
        /// <summary>
        /// High Speed Downlink Packet Access (3.5G, 3G+, H or turbo 3G)
        /// </summary>
        [XmlEnum(Name = "5")]
        HSDPA,
        /// <summary>
        /// High Speed Uplink Packet Access (3G)
        /// </summary>
        [XmlEnum(Name = "6")]
        HSUPA,
        /// <summary>
        /// High Speed Packet Access (3G+)
        /// </summary>
        [XmlEnum(Name = "7")]
        HSPA,
        /// <summary>
        /// TD-SCDMA
        /// </summary>
        [XmlEnum(Name = "8")]
        TDSCDMA,
        /// <summary>
        /// HSPA+ (H+, 3G++, HSPAP or 3G Dual Carrier)
        /// </summary>
        [XmlEnum(Name = "9")]
        HSPAPlus,
        /// <summary>
        /// Evolution-Data Optimized (EVDO) Rev 0
        /// </summary>
        [XmlEnum(Name = "10")]
        EVDORev0,
        /// <summary>
        /// Evolution-Data Optimized (EVDO) Rev A
        /// </summary>
        [XmlEnum(Name = "11")]
        EVDORevA,
        /// <summary>
        /// Evolution-Data Optimized (EVDO) Rev B
        /// </summary>
        [XmlEnum(Name = "12")]
        EVDORevB,
        /// <summary>
        /// CDMA2000 1X (IS-2000), also known as 1x and 1xRTT
        /// </summary>
        [XmlEnum(Name = "13")]
        CDMA2000_1X,
        /// <summary>
        /// UMB (Ultra Mobile Broadband) 
        /// </summary>
        [XmlEnum(Name = "14")]
        UMB,
        /// <summary>
        /// CDMA2000 1xEV-DV
        /// </summary>
        [XmlEnum(Name = "15")]
        CDMA2000_1xEVDV,
        /// <summary>
        /// CDMA2000 3X (3XRTT)
        /// </summary>
        [XmlEnum(Name = "16")]
        CDMA2000_3X,
        /// <summary>
        /// HSPA+ (H+) 64QAM
        /// </summary>
        [XmlEnum(Name = "17")]
        HSPAPlus64QAM,
        /// <summary>
        /// HSPA+ (H+) MIMO
        /// </summary>
        [XmlEnum(Name = "18")]
        HSPAPlusMIMO,
        /// <summary>
        /// LTE (4G)
        /// </summary>
        [XmlEnum(Name = "19")]
        LTE,
        /// <summary>
        /// UMTS (3G)
        /// </summary>
        [XmlEnum(Name = "41")]
        UMTS
    }
}

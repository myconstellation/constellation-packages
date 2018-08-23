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
    /// Traffic transferred
    /// </summary>
    [StateObject, XmlRoot(ElementName = "response")] //api/monitoring/traffic-statistics
    public class TrafficStatistics
    {
        [XmlElement(ElementName = "CurrentConnectTime")]
        public long CurrentConnectTime { get; set; }

        [XmlElement(ElementName = "CurrentUpload")]
        public long CurrentUpload { get; set; }

        [XmlElement(ElementName = "CurrentDownload")]
        public long CurrentDownload { get; set; }

        [XmlElement(ElementName = "CurrentDownloadRate")]
        public long CurrentDownloadRate { get; set; }

        [XmlElement(ElementName = "CurrentUploadRate")]
        public long CurrentUploadRate { get; set; }

        [XmlElement(ElementName = "TotalUpload")]
        public long TotalUpload { get; set; }

        [XmlElement(ElementName = "TotalDownload")]
        public long TotalDownload { get; set; }

        [XmlElement(ElementName = "TotalConnectTime")]
        public long TotalConnectTime { get; set; }

        [XmlElement(ElementName = "showtraffic")]
        public int ShowTraffic { get; set; }
    }
}

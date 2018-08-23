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
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Router monthly statistics
    /// </summary>
    [StateObject, XmlRoot("response")]
    public class MonthlyStatistics
    {
        [XmlElement("CurrentMonthDownload")]
        public long CurrentMonthDownload { get; set; }

        [XmlElement("CurrentMonthUpload")]
        public long CurrentMonthUpload { get; set; }

        [XmlElement("roam_month_download")]
        public long RoamMonthDownload { get; set; }

        [XmlElement("roam_month_upload")]
        public long RoamMonthUpload { get; set; }

        [XmlElement("MonthDuration"), JsonIgnore]
        public long MonthDurationRaw { get; set; }

        [XmlElement("MonthLastClearTime"), JsonIgnore]
        public string MonthLastClearTime { get; set; }

        public TimeSpan MonthDuration => TimeSpan.FromSeconds(this.MonthDurationRaw);
        public DateTime MonthLastClearDate => DateTime.ParseExact(this.MonthLastClearTime, "yyyy-M-d", CultureInfo.InvariantCulture);
    }
}

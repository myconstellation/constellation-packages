/*
 *	 Vorwerk connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2020 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Vorwerk.Models
{
    using System;

    /// <summary>
    /// Represents a firmware information
    /// </summary>
    public partial class FirmwareInfo
    {
        [VorwerkProperty("version")]
        public string Version { get; set; }

        [VorwerkProperty("url")]
        public Uri Url { get; set; }

        [VorwerkProperty("manual_update_info_url")]
        public string ManualUpdateInfoUrl { get; set; }

        [VorwerkProperty("filesize")]
        public long Filesize { get; set; }

        [VorwerkProperty("min_required_version")]
        public string MinRequiredVersion { get; set; }
    }
}

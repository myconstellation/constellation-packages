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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the Dashboard
    /// </summary>
    public partial class Dashboard
    {
        [VorwerkProperty("id")]
        public string Id { get; set; }

        [VorwerkProperty("email")]
        public string Email { get; set; }

        [VorwerkProperty("first_name")]
        public string FirstName { get; set; }

        [VorwerkProperty("last_name")]
        public string LastName { get; set; }

        [VorwerkProperty("locale")]
        public string Locale { get; set; }

        [VorwerkProperty("country_code")]
        public string CountryCode { get; set; }

        [VorwerkProperty("developer")]
        public bool Developer { get; set; }

        [VorwerkProperty("newsletter")]
        public bool Newsletter { get; set; }

        [VorwerkProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [VorwerkProperty("verified_at")]
        public DateTimeOffset VerifiedAt { get; set; }

        [VorwerkProperty("robots")]
        public List<Robot> Robots { get; set; }

        [VorwerkProperty("recent_firmwares")]
        public Dictionary<string, FirmwareInfo> RecentFirmwares { get; set; }

        /// <summary>
        /// Create Dashboard from the json
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static Dashboard FromJson(string json) => JsonConvert.DeserializeObject<Dashboard>(json, VorwerkContractResolver.Settings);
    }    
}


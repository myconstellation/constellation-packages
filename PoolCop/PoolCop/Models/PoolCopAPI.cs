/*
 *	 PoolCop connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018-2019 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace PoolCop.Models
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// PoolCop API informations
    /// </summary>
    public partial class PoolCopAPI
    {
        /// <summary>
        /// The API identifier
        /// </summary>
        [PoolCopilotProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int Id { get; set; }

        /// <summary>
        /// The API name
        /// </summary>
        [PoolCopilotProperty("apiname")]
        public string Name { get; set; }

        /// <summary>
        /// The API level
        /// </summary>
        [PoolCopilotProperty("level")]
        public string Level { get; set; }

        /// <summary>
        /// The API token value
        /// </summary>
        [PoolCopilotProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// The API token expiration date
        /// </summary>
        [PoolCopilotProperty("expire")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// The API secret key
        /// </summary>
        [PoolCopilotProperty("apikey")]
        public string SecretKey { get; set; }

        /// <summary>
        /// The PoolCop API identifier
        /// </summary>
        [PoolCopilotProperty("poolcop_api_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int PoolcopApiId { get; set; }

        /// <summary>
        /// The PoolCop identifier
        /// </summary>
        [PoolCopilotProperty("poolcop_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int PoolcopId { get; set; }
        
        /// <summary>
        /// Remote IP address
        /// </summary>
        [PoolCopilotProperty("ip")]
        public string RemoteIP { get; set; }

        /// <summary>
        /// Number of remaining API calls
        /// </summary>
        [PoolCopilotProperty("max_limit")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int RemainingCalls { get; set; }
    }
}

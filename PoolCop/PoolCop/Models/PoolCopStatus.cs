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

    /// <summary>
    /// All in one PoolCop data, configuration, settings, alarms and Pool related infos...
    /// </summary>
    public partial class PoolCopStatus
    {
        /// <summary>
        /// The API access informations
        /// </summary>
        [PoolCopilotProperty("api_token")]
        public PoolCopAPI API { get; set; }

        /// <summary>
        /// The Pool
        /// </summary>
        [PoolCopilotProperty("Pool")]
        public Pool Pool { get; set; }

        /// <summary>
        /// The PoolCop
        /// </summary>
        [PoolCopilotProperty("PoolCop")]
        public PoolCop PoolCop { get; set; }
        
        /// <summary>
        /// Create StatusResponse from the json
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static PoolCopStatus FromJson(string json) => JsonConvert.DeserializeObject<PoolCopStatus>(json, PoolCopilotContractResolver.Settings);
    }    
}


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
    /// Command result
    /// </summary>
    public partial class CommandResult
    {
        /// <summary>
        /// The API access informations
        /// </summary>
        [PoolCopilotProperty("api_token")]
        public PoolCopAPI API { get; set; }

        /// <summary>
        /// The Pool
        /// </summary>
        [PoolCopilotProperty("Command")]
        public string Command { get; set; }

        /// <summary>
        /// The PoolCop
        /// </summary>
        [PoolCopilotProperty("Result")]
        public string Result { get; set; }

        /// <summary>
        /// Create CommandResult from the json
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static CommandResult FromJson(string json) => JsonConvert.DeserializeObject<CommandResult>(json, PoolCopilotContractResolver.Settings);
    }    
}


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
    using System;

    /// <summary>
    /// Represents the Pool
    /// </summary>
    public partial class Pool
    {
        /// <summary>
        /// The Pool nickname
        /// </summary>
        [PoolCopilotProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// The Poolcop name
        /// </summary>
        [PoolCopilotProperty("poolcop")]
        public string Poolcop { get; set; }

        /// <summary>
        /// The Pool image (optional)
        /// </summary>
        [PoolCopilotProperty("image")]
        public Uri Image { get; set; }

        /// <summary>
        /// The Pool latitude location
        /// </summary>
        [PoolCopilotProperty("latitude")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// The Pool longitude location
        /// </summary>
        [PoolCopilotProperty("longitude")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// The Pool timezone
        /// </summary>
        [PoolCopilotProperty("timezone")]
        public string Timezone { get; set; }
    }
}

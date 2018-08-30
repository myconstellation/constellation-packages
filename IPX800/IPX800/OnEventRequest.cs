/*
 *	 GCE Electronics IPX800 Package for Constellation
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

namespace IPX800
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides data when IPX push "OnEvent" request
    /// </summary>
    internal class OnEventRequest
    {
        /// <summary>
        /// Gets or sets a value indicating the type of IPX push (ON or OFF)
        /// </summary>
        /// <value>
        ///   <c>true</c> if URL ON; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("S")]
        public bool State { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        [JsonProperty("V")]
        public string Values { get; set; }

        /// <summary>
        /// Gets or sets the IPX element type (R, D, VO or VI).
        /// </summary>
        /// <value>
        /// The IPX element type.
        /// </value>
        [JsonProperty("T")]
        public string Type { get; set; }
    }
}

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

namespace IPX800.Enumerations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.ComponentModel;

    /// <summary>
    /// Wire Pilot (FP) mode
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WirePilotMode
    {
        /// <summary>
        /// Comfort (normal mode)
        /// </summary>
        [Description("Confort")]
        Comfort = 0,
        /// <summary>
        /// Reduced mode
        /// </summary>
        [Description("Eco")]
        Eco = 1,
        /// <summary>
        /// Frost free
        /// </summary>
        [Description("Hors Gel")]
        FrostFree = 2,
        /// <summary>
        /// Off
        /// </summary>
        [Description("Arret")]
        Off = 3,
        /// <summary>
        /// Comfort -1°C
        /// </summary>
        [Description("Confort -1")]
        ComfortS1 = 4,
        /// <summary>
        /// Comfort -2°C
        /// </summary>
        [Description("Confort -2")]
        ComfortS2 = 5,
    };
}

/*
 *	 SNMP Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Snmp
{
    using Newtonsoft.Json;
    using SnmpSharpNet;

    /// <summary>
    /// A (conceptual) entry for one processor contained by the host.
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// The product ID of the firmware associated with the processor.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.3.3.1.1")]
        [JsonConverter(typeof(OidConverter))]
        public Oid FirmwareID { get; set; }

        /// <summary>
        /// The average, over the last minute, of the percentage of time that this processor was not idle.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.3.3.1.2")]
        public int Load { get; set; }
    }
}

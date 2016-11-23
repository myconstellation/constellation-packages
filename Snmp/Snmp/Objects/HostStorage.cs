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
    /// A (conceptual) entry for one logical storage area on the host.
    /// </summary>
    public class Storage
    {
        /// <summary>
        /// A unique value for each logical storage area contained by the host.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.1")]
        public int Index { get; set; }

        /// <summary>
        ///  The type of storage represented by this entry.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.2")]
        [JsonConverter(typeof(OidConverter))]
        public Oid Type { get; set; }

        /// <summary>
        /// A description of the type and instance of the storage described by this entry.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.3")]
        public string Description { get; set; }

        /// <summary>
        /// The size, in bytes, of the data objects allocated from this pool.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.4")]
        public int AllocationUnits { get; set; }

        /// <summary>
        /// The size of the storage represented by this entry, in units of hrStorageAllocationUnits.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.5")]
        public int Size { get; set; }

        /// <summary>
        /// The amount of the storage represented by this entry that is allocated, in units of AllocationUnits.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.6")]
        public int Used { get; set; }

        /// <summary>
        ///  The number of requests for storage represented by this entry that could not be honored due to not enough storage.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1.7")]
        public uint AllocationFailures { get; set; }
    }
}

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
    using System;

    /// <summary>
    /// Description of the system's hardware type, software operating-system, and networking software.
    /// </summary>
    [SnmpObject, OID(".1.3.6.1.2.1.1")]
    public class SystemDescription
    {
        /// <summary>
        /// A textual description of the entity. 
        /// </summary>
        [OID(".1.3.6.1.2.1.1.1")]
        public string Description { get; set; }

        /// <summary>
        /// The vendor's authoritative identification of the network management subsystem contained in the entity.
        /// </summary>
        [OID(".1.3.6.1.2.1.1.2")]
        [JsonConverter(typeof(OidConverter))]
        public Oid ObjectID { get; set; }

        /// <summary>
        /// The time since the network management portion of the system was last re-initialized.
        /// </summary>
        [OID(".1.3.6.1.2.1.1.3")]
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// The textual identification of the contact person for this managed node, together with information on how to contact this person.
        /// </summary>
        [OID(".1.3.6.1.2.1.1.4")]
        public string Contact { get; set; }

        /// <summary>
        /// An administratively-assigned name for this managed node.
        /// </summary>
        [OID(".1.3.6.1.2.1.1.5")]
        public string Name { get; set; }

        /// <summary>
        /// The physical location of this node.
        /// </summary>
        [OID(".1.3.6.1.2.1.1.6")]
        public string Location { get; set; }

        /// <summary>
        /// A value which indicates the set of services that this entity primarily offers.
        /// </summary>
        [OID(".1.3.6.1.2.1.1.7")]
        public int Services { get; set; }
    }
}

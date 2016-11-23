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
    using Constellation.Package;

    /// <summary>
    /// Represent a SNMP device.
    /// </summary>
    [StateObject]
    public class SnmpDevice
    {
        /// <summary>
        /// The system's description.
        /// </summary>
        [OID(".1.3.6.1.2.1.1"), Required]
        public SystemDescription Description { get; set; }

        /// <summary>
        /// Network interfaces of this managed node.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1"), Required]
        public Sequence<NetworkInterface> Interfaces { get; set; }

        /// <summary>
        /// IP addresse of this managed node.
        /// </summary>
        [OID(".1.3.6.1.2.1.4.20.1"), Required]
        public Sequence<Address> Addresses { get; set; }

        /// <summary>
        /// The host resources's description.
        /// </summary>
        [OID(".1.3.6.1.2.1.25")]
        public Host Host { get; set; }

        /// <summary>
        /// The (conceptual) table of logical storage areas on the host.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.3.1")]
        public Sequence<Storage> Storages { get; set; }

        /// <summary>
        /// The (conceptual) table of processors contained by the host.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.3.3.1")]
        public Sequence<Processor> ProcessorsLoad { get; set; }
    }
}

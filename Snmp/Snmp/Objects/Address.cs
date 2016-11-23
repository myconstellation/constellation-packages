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
    using System.Net;

    /// <summary>
    /// The addressing information for one of this entity's IP addresses
    /// </summary>
    [SnmpObject, OID(".1.3.6.1.2.1.4.20.1")]
    public class Address
    {
        /// <summary>
        /// The IP address to which this entry's addressing information pertains.
        /// </summary>
        [OID(".1.3.6.1.2.1.4.20.1.1")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IP { get; set; }

        /// <summary>
        /// The index value which uniquely identifies the interface to which this entry is applicable.
        /// </summary>
        [OID(".1.3.6.1.2.1.4.20.1.2")]
        public int Index { get; set; }

        /// <summary>
        /// The subnet mask associated with the IP address of this entry.
        /// </summary>
        [OID(".1.3.6.1.2.1.4.20.1.3")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress Mask { get; set; }

        /// <summary>
        ///  The value of the least-significant bit in the IP broadcast address used for sending datagrams on the(logical) interface associated with the IP address of this entry.
        /// </summary>
        [OID(".1.3.6.1.2.1.4.20.1.4")]
        public int Broadcast { get; set; }

        /// <summary>
        /// The size of the largest IP datagram which this entity can re-assemble from incoming IP fragmented datagrams received on this interface.
        /// </summary>
        [OID(".1.3.6.1.2.1.4.20.1.5")]
        public int ReassembleMaxSize { get; set; }
    }
}

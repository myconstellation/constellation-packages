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
    /// An interface entry containing objects at the subnetwork layer and below for a particular interface.
    /// </summary>
    [SnmpObject, OID(".1.3.6.1.2.1.2.2.1")]
    public class NetworkInterface
    {
        /// <summary>
        /// A unique value for each interface.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.1")]
        public int Id { get; set; }

        /// <summary>
        /// A textual string containing information about the interface.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.2")]
        public string Description { get; set; }

        /// <summary>
        /// The type of interface, distinguished according to the physical/link protocol(s) immediately `below' the network layer in the protocol stack.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.3")]
        public int Type { get; set; }

        /// <summary>
        ///  The size of the largest datagram which can be sent/received on the interface, specified in octets.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.4")]
        public int MTU { get; set; }

        /// <summary>
        ///  An estimate of the interface's current bandwidth in bits per second.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.5")]
        public uint Speed { get; set; }

        /// <summary>
        /// The interface's address at the protocol layer immediately `below' the network layer in the protocol stack.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.6")]
        public string PhysAddress { get; set; }

        /// <summary>
        /// The desired state of the interface.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.7")]
        public AdminStatusType AdminStatus { get; set; }

        /// <summary>
        /// The current operational state of the interface.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.8")]
        public OpenStatusType OpenStatus { get; set; }

        /// <summary>
        /// The value of sysUpTime at the time the interface entered its current operational state.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.9")]
        public TimeSpan LastChange { get; set; }

        /// <summary>
        /// The total number of octets received on the interface, including framing characters.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.10")]
        public uint InOctects { get; set; }

        /// <summary>
        /// The number of subnetwork-unicast packets delivered to a higher-layer protocol.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.11")]
        public uint InUcastPkts { get; set; }

        /// <summary>
        /// The number of non-unicast (i.e., subnetwork-broadcast or subnetwork-multicast) packets delivered to a higher-layer protocol.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.12")]
        public uint InNUcastPkts { get; set; }

        /// <summary>
        /// The number of inbound packets which were chosen to be discarded even though no errors had been detected to prevent their being deliverable to a higher-layer protocol.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.13")]
        public uint InDiscards { get; set; }

        /// <summary>
        /// The number of inbound packets that contained errors preventing them from being deliverable to a higher-layer protocol.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.14")]
        public uint InErrors { get; set; }

        /// <summary>
        /// The number of packets received via the interface which were discarded because of an unknown or unsupported protocol.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.15")]
        public uint InUnknownProtos { get; set; }

        /// <summary>
        /// The total number of octets transmitted out of the interface, including framing characters.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.16")]
        public uint OutOctets { get; set; }

        /// <summary>
        /// The total number of packets that higher-level protocols requested be transmitted to a subnetwork-unicast address, including those that were discarded or not sent.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.17")]
        public uint OutUcastPkts { get; set; }

        /// <summary>
        /// The total number of packets that higher-level protocols requested be transmitted to a non-unicast address, including those that were discarded or not sent.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.18")]
        public uint OutNUcastPkts { get; set; }

        /// <summary>
        /// The number of outbound packets which were chosen to be discarded even though no errors had been detected to prevent their being transmitted.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.19")]
        public uint OutDiscards { get; set; }

        /// <summary>
        /// The number of outbound packets that could not be transmitted because of errors.
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.20")]
        public uint OutErrors { get; set; }

        /// <summary>
        /// The length of the output packet queue (in packets).
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.21")]
        public uint OutQLen { get; set; }

        /// <summary>
        /// A reference to MIB definitions specific to the particular media being used to realize the interface. 
        /// </summary>
        [OID(".1.3.6.1.2.1.2.2.1.22")]
        [JsonConverter(typeof(OidConverter))]
        public Oid Specific { get; set; }

        /// <summary>
        /// Admin status type
        /// </summary>
        public enum AdminStatusType
        {
            NotSet = 0,
            Up = 1,
            Down = 2,
            Testing = 3
        }

        /// <summary>
        /// Open Status type
        /// </summary>
        public enum OpenStatusType
        {
            NotSet = 0,
            Up = 1,
            Down = 2,
            Testing = 3,
            Unknow = 4,
            Dormant = 5,
            NotPresent = 6,
            LowerLayerDown = 7
        }
    }
}

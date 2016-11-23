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
    using System;

    /// <summary>
    /// Description of the host.
    /// </summary>
    [SnmpObject, OID(".1.3.6.1.2.1.25")]
    public class Host
    {
        /// <summary>
        /// The amount of time since this host was last initialized.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.1")]
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// The host's notion of the local date and time of day.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.2")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The index of the hrDeviceEntry for the device from which this host is configured to load its initial operating system configuration.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.3")]
        public int InitialLoadDevice { get; set; }

        /// <summary>
        /// This object contains the parameters supplied to the load device when requesting the initial operating system configuration from that device.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.4")]
        public string InitialLoadParameters { get; set; }

        /// <summary>
        /// The number of user sessions for which this host is storing state information.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.5")]
        public uint NumUsers { get; set; }

        /// <summary>
        ///  The number of process contexts currently loaded or running on this system.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.6")]
        public uint Processes { get; set; }

        /// <summary>
        ///  The maximum number of process contexts this system can support.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.1.7")]
        public int MaxProcesses { get; set; }

        /// <summary>
        /// The amount of physical read-write main memory, typically RAM, contained by the host.
        /// </summary>
        [OID(".1.3.6.1.2.1.25.2.2")]
        public int MemorySize { get; set; }
    }
}

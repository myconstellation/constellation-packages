/*
 *	 Paradox connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Paradox
{
    using Constellation.Package;

    /// <summary>
    /// Paradox Area information
    /// </summary>
    [StateObject]
    public class AreaInfo : BaseParadoxItem
    {
        /// <summary>
        /// A value indicating whether the area is full armed.
        /// </summary>
        /// <value>
        /// <c>true</c> if full armed; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullArmed { get; set; }
        /// <summary>
        /// A value indicating whether the area is stay armed.
        /// </summary>
        /// <value>
        /// <c>true</c> if stay armed; otherwise, <c>false</c>.
        /// </value>
        public bool IsStayArmed { get; set; }
        /// <summary>
        /// A value indicating whether the area has zone in memory.
        /// </summary>
        /// <value>
        ///   <c>true</c> if zone in memory; otherwise, <c>false</c>.
        /// </value>
        public bool ZoneInMemory { get; set; }
        /// <summary>
        /// A value indicating whether the area has trouble.
        /// </summary>
        /// <value>
        /// <c>true</c> if has trouble; otherwise, <c>false</c>.
        /// </value>
        public bool HasTrouble { get; set; }
        /// <summary>
        /// A value indicating whether the area is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ready; otherwise, <c>false</c>.
        /// </value>
        public bool IsReady { get; set; }
        /// <summary>
        /// A value indicating whether the area is in programming.
        /// </summary>
        /// <value>
        /// <c>true</c> if is in programming; otherwise, <c>false</c>.
        /// </value>
        public bool IsInProgramming { get; set; }
        /// <summary>
        /// A value indicating whether the area is in alarm.
        /// </summary>
        /// <value>
        ///   <c>true</c> if in alarm; otherwise, <c>false</c>.
        /// </value>
        public bool InAlarm { get; set; }
        /// <summary>
        /// A value indicating whether the area is strobe
        /// </summary>
        /// <value>
        ///   <c>true</c> if strobe; otherwise, <c>false</c>.
        /// </value>
        public bool Strobe { get; set; }
    }
}

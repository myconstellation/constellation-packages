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
    /// Represent Paradox Zone information
    /// </summary>
    [StateObject]
    public class ZoneInfo : BaseParadoxItem
    {
        /// <summary>
        /// A value indicating whether this zone is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get; set; }
        /// <summary>
        /// A value indicating whether this zone is tamper.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is tamper; otherwise, <c>false</c>.
        /// </value>
        public bool IsTamper { get; set; }
        /// <summary>
        /// A value indicating whether this zone is in alarm.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [in alarm]; otherwise, <c>false</c>.
        /// </value>
        public bool InAlarm { get; set; }
        /// <summary>
        /// A value indicating whether this zone is in fire alarm].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [in fire alarm]; otherwise, <c>false</c>.
        /// </value>
        public bool InFireAlarm { get; set; }
        /// <summary>
        /// A value indicating whether this zone has supervision lost.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supervision lost]; otherwise, <c>false</c>.
        /// </value>
        public bool SupervisionLost { get; set; }
        /// <summary>
        /// A value indicating whether this zone has low battery.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [low battery]; otherwise, <c>false</c>.
        /// </value>
        public bool LowBattery { get; set; }
    }
}

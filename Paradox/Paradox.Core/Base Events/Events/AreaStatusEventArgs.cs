/*
 *	 Paradox .NET library
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Sebastien Warin under one or more contributor
 *	 license agreements. Sebastien Warin licenses this file to you under
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
    /// <summary>
    /// Represent an Area change event
    /// </summary>
    public class AreaStatusEventArgs : ParadoxBaseEventArgs
    {
        /// <summary>
        /// Gets or sets the area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        public Area Area { get; set; }

        /// <summary>
        /// Gets or sets the area status.
        /// </summary>
        /// <value>
        /// The area status.
        /// </value>
        public AreaStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether zone in memory.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [zone in memory]; otherwise, <c>false</c>.
        /// </value>
        public bool ZoneInMemory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this area has trouble.
        /// </summary>
        /// <value>
        /// <c>true</c> if this area has trouble; otherwise, <c>false</c>.
        /// </value>
        public bool HasTrouble { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this area is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this area is ready; otherwise, <c>false</c>.
        /// </value>
        public bool IsReady { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this area is in programming.
        /// </summary>
        /// <value>
        /// <c>true</c> if this area is in programming; otherwise, <c>false</c>.
        /// </value>
        public bool IsInProgramming { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this area is in alarm.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this area is in alarm; otherwise, <c>false</c>.
        /// </value>
        public bool InAlarm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this area is strobe.
        /// </summary>
        /// <value>
        ///   <c>true</c> if area strobe; otherwise, <c>false</c>.
        /// </value>
        public bool Strobe { get; set; }

        /// <summary>
        /// Processes the raw message to extract the event data.
        /// </summary>
        /// <param name="message">The raw message.</param>
        internal override void ProcessMessage(string message)
        {
            this.Area = Utils.GetEnumValueFromStringId<Area>(message.Substring(2, 3));
            this.Status = Utils.GetEnumValueFromDescription<AreaStatus>(message[5].ToString());
            this.ZoneInMemory = message[6] != 'O';
            this.HasTrouble = message[7] != 'O';
            this.IsReady = message[8] == 'O';
            this.IsInProgramming = message[9] != 'O';
            this.InAlarm = message[10] != 'O';
            this.Strobe = message[11] != 'O';
        }
    }
}

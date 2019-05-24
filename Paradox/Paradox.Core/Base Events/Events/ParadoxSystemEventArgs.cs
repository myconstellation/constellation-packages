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
    using System;

    /// <summary>
    /// Represent a Paradox system event
    /// </summary>
    public class ParadoxSystemEventArgs : ParadoxBaseEventArgs
    {
        /// <summary>
        /// Gets or sets the area number.
        /// </summary>
        /// <value>
        /// The area number.
        /// </value>
        public int AreaNumber { get; set; }

        /// <summary>
        /// Gets or sets the event group.
        /// </summary>
        /// <value>
        /// The event group.
        /// </value>
        public EventGroup EventGroup { get; set; }

        /// <summary>
        /// Gets or sets the event number.
        /// </summary>
        /// <value>
        /// The event number.
        /// </value>
        public int EventNumber { get; set; }

        /// <summary>
        /// Gets the raw message text.
        /// </summary>
        /// <value>
        /// The raw message text.
        /// </value>
        public string MessageText { get { return this.ToString(); } }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public string MessageType { get { return this.GetType().Name; } }

        /// <summary>
        /// Copies event from the specified source.
        /// </summary>
        /// <param name="source">The <see cref="ParadoxSystemEventArgs"/> instance containing the source event data.</param>
        public void CopyFrom(ParadoxSystemEventArgs source)
        {
            this.AreaNumber = source.AreaNumber;
            this.EventGroup = source.EventGroup;
            this.EventNumber = source.EventNumber;
            this.MessageDate = source.MessageDate;
        }

        /// <summary>
        /// Processes the raw message to extract the event data.
        /// </summary>
        /// <param name="message">The raw message.</param>
        internal override void ProcessMessage(string message)
        {
            this.EventGroup = Utils.GetEnumValueFromStringId<EventGroup>(message.Substring(1, 3));
            this.EventNumber = Convert.ToInt32(message.Substring(5, 3));
            this.AreaNumber = Convert.ToInt32(message.Substring(9, 3));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this event.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this event.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} : {2} EventNumber={3} AreaNumber={4} ", this.MessageDate.ToShortDateString(), this.MessageDate.ToLongTimeString(), this.EventGroup, this.EventNumber, this.AreaNumber);
        }
    }
}

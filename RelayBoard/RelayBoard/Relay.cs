/*
 *	 RelayBoard Package for Constellation
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

namespace RelayBoard
{
    using System;

    /// <summary>
    /// The SainSmart relays
    /// </summary>
    public enum Relay
    {
        /// <summary>
        /// The relay #1
        /// </summary>
        [Relay(1, 0x01)]
        Relay1,
        /// <summary>
        /// The relay #2
        /// </summary>
        [Relay(2, 0x02)]
        Relay2,
        /// <summary>
        /// The relay #3
        /// </summary>
        [Relay(3, 0x04)]
        Relay3,
        /// <summary>
        /// The relay #4
        /// </summary>
        [Relay(4, 0x08)]
        Relay4,
        /// <summary>
        /// The relay #5
        /// </summary>
        [Relay(5, 0x10)]
        Relay5,
        /// <summary>
        /// The relay #6
        /// </summary>
        [Relay(6, 0x20)]
        Relay6,
        /// <summary>
        /// The relay #7
        /// </summary>
        [Relay(7, 0x40)]
        Relay7,
        /// <summary>
        /// The relay #8
        /// </summary>
        [Relay(8, 0x80)]
        Relay8,
        /// <summary>
        /// All relays
        /// </summary>
        [Relay(0xFF)]
        All
    }

    /// <summary>
    /// Specify the relay identifier
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class RelayAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the relay's code.
        /// </summary>
        /// <value>
        /// The relay's code.
        /// </value>
        public byte Code { get; set; }

        /// <summary>
        /// Gets or sets the relay's number.
        /// </summary>
        /// <value>
        /// The relay's number.
        /// </value>
        public int Number { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayAttribute" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public RelayAttribute(byte code)
        {
            this.Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayAttribute" /> class.
        /// </summary>
        /// <param name="number">The relay's number.</param>
        /// <param name="code">The relay's code.</param>
        public RelayAttribute(int number, byte code)
        {
            this.Number = number;
            this.Code = code;
        }
    }
}
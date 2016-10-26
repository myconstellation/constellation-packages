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
    using System.ComponentModel;

    /// <summary>
    /// The SainSmart relays
    /// </summary>
    [Flags]
    public enum Relay : byte
    {
        /// <summary>
        /// The relay #1
        /// </summary>
        [Description("1")]
        Relay1 = 0x01,
        /// <summary>
        /// The relay #2
        /// </summary>
        [Description("2")]
        Relay2 = 0x02,
        /// <summary>
        /// The relay #3
        /// </summary>
        [Description("3")]
        Relay3 = 0x04,
        /// <summary>
        /// The relay #4
        /// </summary>
        [Description("4")]
        Relay4 = 0x08,
        /// <summary>
        /// The relay #5
        /// </summary>
        [Description("5")]
        Relay5 = 0x10,
        /// <summary>
        /// The relay #6
        /// </summary>
        [Description("6")]
        Relay6 = 0x20,
        /// <summary>
        /// The relay #7
        /// </summary>
        [Description("7")]
        Relay7 = 0x40,
        /// <summary>
        /// The relay #8
        /// </summary>
        [Description("8")]
        Relay8 = 0x80,
        /// <summary>
        /// All relays
        /// </summary>
        [Description("All")]
        All = 0xFF
    }
}
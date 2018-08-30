/*
 *	 GCE Electronics IPX800 Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace IPX800.Enumerations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    /// <summary>
    /// Get arguments
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GetArgument
    {
        /// <summary>
        /// All (Get=all) except Analog (A), Virtual Analog (VA), Counters (C), PWM, DMX and SMS
        /// </summary>
        [EnumMember(Value = "all")]
        All,
        /// <summary>
        /// Output (Get=R)
        /// </summary>
        [EnumMember(Value = "R")]
        Output,
        /// <summary>
        /// Input (Get=D)
        /// </summary>
        [EnumMember(Value = "D")]
        Input,
        /// <summary>
        /// Virtual Output (Get=VO)
        /// </summary>
        [EnumMember(Value = "VO")]
        VirtualOutput,
        /// <summary>
        /// Virtual Input (Get=VI)
        /// </summary>
        [EnumMember(Value = "VI")]
        VirtualInput,
        /// <summary>
        /// Analog (Get=A)
        /// </summary>
        [EnumMember(Value = "A")]
        Analog,
        /// <summary>
        /// Counter (Get=C)
        /// </summary>
        [EnumMember(Value = "C")]
        Counter,
        /// <summary>
        /// Virtual Analog (Get=VA)
        /// </summary>
        [EnumMember(Value = "VA")]
        VirtualAnalog,
        /// <summary>
        /// Watch Dog (Get=PW)
        /// </summary>
        [EnumMember(Value = "PW")]
        WatchDog,
        /// <summary>
        /// EnOcean (Get=XENO)
        /// </summary>
        [EnumMember(Value = "XENO")]
        EnOcean,
        /// <summary>
        /// THL (Get=XTHL)
        /// </summary>
        [EnumMember(Value = "XTHL")]
        THL,
        /// <summary>
        /// PWM (Get=XPWM|1-24)
        /// </summary>
        [EnumMember(Value = "XPWM")]
        PWM,
        /// <summary>
        /// DMX (Get=XDMX|1-512)
        /// </summary>
        [EnumMember(Value = "XDMX")]
        DMX,
        /// <summary>
        /// Roller Shutter (Get=VR)
        /// </summary>
        [EnumMember(Value = "VR")]
        RollerShutter,
        /// <summary>
        /// Wire pilot (Get=FP)
        /// </summary>
        [EnumMember(Value = "FP")]
        WirePilot,
        /// <summary>
        /// X-Dimmer (Get=G)
        /// </summary>
        [EnumMember(Value = "G")]
        Dimmer,
        /// <summary>
        /// Thermostat (Get=T)
        /// </summary>
        [EnumMember(Value = "T")]
        Thermostat,
        /// <summary>
        /// SMS (Get=SMS)
        /// </summary>
        [EnumMember(Value = "SMS")]
        SMS
    }
}

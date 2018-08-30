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
    /// Set arguments
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SetArgument
    {
        /// <summary>
        /// Output (R)
        /// </summary>
        [EnumMember(Value = "R"), IPXIdentifier(IPXIdentifierFormats.Output)]
        Output,
        /// <summary>
        /// Virtual Output (VO)
        /// </summary>
        [EnumMember(Value = "VO"), IPXIdentifier(IPXIdentifierFormats.VirtualOutput)]
        VirtualOutput,
        /// <summary>
        /// Virtual Input (VI)
        /// </summary>
        [EnumMember(Value = "VI"), IPXIdentifier(IPXIdentifierFormats.VirtualInput)]
        VirtualInput,
        /// <summary>
        /// Counter (C)
        /// </summary>
        [EnumMember(Value = "C"), IPXIdentifier(IPXIdentifierFormats.Counter)]
        Counter,
        /// <summary>
        /// Virtual Analog (VA)
        /// </summary>
        [EnumMember(Value = "VA"), IPXIdentifier(IPXIdentifierFormats.VirtualAnalog)]
        VirtualAnalog,
        /// <summary>
        /// Roller shutter (VR)
        /// </summary>
        [EnumMember(Value = "VR"), IPXIdentifier(IPXIdentifierFormats.RollerShutter)]
        RollerShutter,
        /// <summary>
        /// Wire pilot (FP)
        /// </summary>
        [EnumMember(Value = "FP"), IPXIdentifier(IPXIdentifierFormats.WirePilot)]
        WirePilot,
        /// <summary>
        /// EnOcean (EnoPC)
        /// </summary>
        [EnumMember(Value = "EnoPC"), IPXIdentifier("^EnoPC(\\d{1,2})$")]
        EnOcean,
        /// <summary>
        /// Pulse UP
        /// </summary>
        [EnumMember(Value = "PulseUP"), IPXIdentifier("^PulseUP(\\d{1,2})$")]
        PulseUp,
        /// <summary>
        /// Pulse DOWN
        /// </summary>
        [EnumMember(Value = "PulseDOWN"), IPXIdentifier("^PulseDOWN(\\d{1,2})$")]
        PulseDown,
        /// <summary>
        /// SMS
        /// </summary>
        [EnumMember(Value = "SMS")]
        SMS,
        /// <summary>
        /// X-Dimmer (G)
        /// </summary>
        [EnumMember(Value = "G"), IPXIdentifier(IPXIdentifierFormats.Dimmer)]
        Dimmer,
        /// <summary>
        /// X-PWM (PWM)
        /// </summary>
        [EnumMember(Value = "PWM"), IPXIdentifier(IPXIdentifierFormats.PWM)]
        PWM,
    }
}

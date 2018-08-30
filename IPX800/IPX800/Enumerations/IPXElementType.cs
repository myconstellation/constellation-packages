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
    using System.ComponentModel;

    /// <summary>
    /// IPX element types
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IPXElementType
    {
        /// <summary>
        /// Undefined element type
        /// </summary>
        Undefined,
        /// <summary>
        /// Output / Relay element (R)
        /// </summary>
        [Description("R")]
        Output,
        /// <summary>
        /// Input element (D)
        /// </summary>
        [Description("D")]
        Input,
        /// <summary>
        /// Virtual Output element (VO)
        /// </summary>
        /// </summary>
        [Description("VO")]
        VirtualOutput,
        /// <summary>
        /// Virtual Input element (VI)
        /// </summary>
        [Description("VI")]
        VirtualInput,
        /// <summary>
        /// Analog element (A)
        /// </summary>
        [Description("A")]
        Analog,
        /// <summary>
        /// Counter element (C)
        /// </summary>
        [Description("C")]
        Counter,
        /// <summary>
        /// Virtual Analog element (VA)
        /// </summary>
        [Description("VA")]
        VirtualAnalog,
        /// <summary>
        /// Watch Dog element (PW)
        /// </summary>
        [Description("PW")]
        WatchDog,
        /// <summary>
        /// Roller Shutter element (VR)
        /// </summary>
        [Description("VR")]
        RollerShutter,
        /// <summary>
        /// Wire Pilot element (FP)
        /// </summary>
        [Description("FP")]
        WirePilot,
        /// <summary>
        /// X-Dimmer element (G)
        /// </summary>
        [Description("G")]
        Dimmer,
        /// <summary>
        /// Thermostat element (T)
        /// </summary>
        [Description("T")]
        Thermostat,
        /// <summary>
        /// X-THL element (XTHL)
        /// </summary>
        [Description("XTHL")]
        THL,
        /// <summary>
        /// X-PWM element (XPWM)
        /// </summary>
        [Description("XPWM")]
        PWM,
        /// <summary>
        /// X-DMX element (XDMX)
        /// </summary>
        [Description("XDMX")]
        DMX,
        /// <summary>
        /// EnOcean Switch element (XENO)
        /// </summary>
        [Description("XENO")]
        EnOceanSwitch,
        /// <summary>
        /// EnOcean Contact element (XENO)
        /// </summary>
        [Description("XENO")]
        EnOceanContact,
        /// <summary>
        /// EnOcean Actuator element (XENO)
        /// </summary>
        [Description("XENO")]
        EnOceanActuator,
        /// <summary>
        /// EnOcean Analog element (XENO)
        /// </summary>
        [Description("XENO")]
        EnOceanAnalog,
        /// <summary>
        /// EnOcean Roller Shutter element (XENO)
        /// </summary>
        [Description("XENO")]
        EnOceanRollerShutter,
    }
}

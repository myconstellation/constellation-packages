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
    /// <summary>
    /// IPX identifier regex patterns
    /// </summary>
    public static class IPXIdentifierFormats
    {
        /// <summary>
        /// Output / Relay (Rxx)
        /// </summary>
        public const string Output = "^R(\\d{1,2})$";
        /// <summary>
        /// Input (Dxx)
        /// </summary>
        public const string Input = "^D(\\d{1,2})$";
        /// <summary>
        /// Virtual Output (VOxxx)
        /// </summary>
        public const string VirtualOutput = "^VO(\\d{1,3})$";
        /// <summary>
        /// Virtual Input (VIxxx)
        /// </summary>
        public const string VirtualInput = "^VI(\\d{1,3})$";
        /// <summary>
        /// Counter (Cxx)
        /// </summary>
        public const string Counter = "^C(\\d{1,2})$";
        /// <summary>
        /// Analog (Ax)
        /// </summary>
        public const string Analog = "^A(\\d)$";
        /// <summary>
        /// Virtual Analog (VAxx)
        /// </summary>
        public const string VirtualAnalog = "^VA(\\d{1,2})$";
        /// <summary>
        /// Roller Shutter (VRxx)
        /// </summary>
        public const string RollerShutter = "^VR(\\d{1,2})$";
        /// <summary>
        /// Roller Shutter (VRx-y)
        /// </summary>
        public const string RollerShutterGetId = "^VR(\\d)-(\\d)$";
        /// <summary>
        /// Thermostat (Txx)
        /// </summary>
        public const string Thermostat = "^T(\\d{1,2})$";
        /// <summary>
        /// Wire Pilot (FPxx)
        /// </summary>
        public const string WirePilot = "^FP(\\d{1,2})$";
        /// <summary>
        /// Wire Pilot (FPx Zone y)
        /// </summary>
        public const string WirePilotGetId = "^FP(\\d) Zone (\\d)$"; 
        /// <summary>
        /// X-Dimmer (Gxx)
        /// </summary>
        public const string Dimmer = "^G(\\d{1,2})$";
        /// <summary>
        /// X-THL (THLx-YYY)
        /// </summary>
        public const string THL = "^(?<id>THL\\d)-(HUM|TEMP|LUM)$";
        /// <summary>
        /// PWM (PWMxx)
        /// </summary>
        public const string PWM = "^PWM(\\d{1,2})$";
        /// <summary>
        /// DMX (DMXxxx)
        /// </summary>
        public const string DMX = "^DMX(\\d{1,3})$";
        /// <summary>
        /// WatchDog (PWxx)
        /// </summary>
        public const string WatchDog = "^PW(\\d{1,2})$";
        /// <summary>
        /// EnOcean Switch (ENO SWITCHxx)
        /// </summary>
        public const string EnOceanSwitch = "^ENO SWITCH(\\d{1,2})$";
        /// <summary>
        /// EnOcean Contact (ENO CONTACTxx)
        /// </summary>
        public const string EnOceanContact = "^ENO CONTACT(\\d{1,2})$";
        /// <summary>
        /// EnOcean Actuator (ENO ACTIONNEURxx)
        /// </summary>
        public const string EnOceanActuator = "^ENO ACTIONNEUR(\\d{1,2})$";
        /// <summary>
        /// EnOcean Analog (ENO ANALOGxx)
        /// </summary>
        public const string EnOceanAnalog = "^ENO ANALOG(\\d{1,2})$";
        /// <summary>
        /// EnOcean Roller Shutter (ENO VOLET ROULANTxx)
        /// </summary>
        public const string EnOceanRollerShutter = "^ENO VOLET ROULANT(\\d{1,2})$";
    }
}

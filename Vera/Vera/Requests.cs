/*
 *	 Vera Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2012-2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Vera
{
    /// <summary>
    /// Z-Wave Device request
    /// </summary>
    public class DeviceRequest
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// The state of the device (True = On, False = Off)
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// The level (0 to 100%) for dimmable device 
        /// </summary>
        public int Level { get; set; }
    }


    /// <summary>
    /// Window Covering Action
    /// </summary>
    public enum WindowCoveringAction
    {
        /// <summary>
        /// UP Command
        /// </summary>
        UP = 0,

        /// <summary>
        /// Down Command
        /// </summary>
        DOWN = 1,

        /// <summary>
        /// Stop Command
        /// </summary>
        STOP = 2
    }

    /// <summary>
    /// Z-Wave Device request
    /// </summary>
    public class DeviceActionRequest
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// The command to action : Up (0), Down(1), Stop(2)
        /// </summary>
        public WindowCoveringAction Action { get; set; }
    }

    /// <summary>
    /// Mode target for themostats
    /// </summary>
    public enum ModeTarget
    {
        Off = 0,
        HeatOn = 1,
        CoolOn = 2,
        AutoChangeOver = 3
    }

    /// <summary>
    /// Z-Wave Device request
    /// </summary>
    public class DeviceTemperatureRequest
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// The temperature to set
        /// </summary>
        public double Temperature { get; set; }
    }

    /// <summary>
    /// Z-Wave Device request
    /// </summary>
    public class DeviceThermostatModeRequest
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// Thermostat Mode
        /// </summary>
        public ModeTarget ModeTarget { get; set; }
    }

    /// <summary>
    /// Z-Wave Device request
    /// </summary>
    public class DeviceDoorLockRequest
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// value for lock of door
        /// </summary>
        public bool Locked { get; set; }
    }
}

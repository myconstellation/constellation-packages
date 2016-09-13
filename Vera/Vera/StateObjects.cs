/*
 *	 Vera Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2012-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace VeraNet
{
    using Constellation.Package;
    using System;

    /// <summary>
    /// Vera Device
    /// </summary>
    public class VeraDevice
    {
        /// <summary>
        /// Gets or sets the current data version.
        /// </summary>
        /// <value>
        /// The current data version.
        /// </value>
        public long DataVersion { get; set; }
        /// <summary>
        /// Gets or sets the load time.
        /// </summary>
        /// <value>
        /// The load time.
        /// </value>
        public long LoadTime { get; set; }
        /// <summary>
        /// Gets or sets the Vera state.
        /// </summary>
        /// <value>
        /// The Vera state.
        /// </value>
        public string State { get; set; }
        /// <summary>
        /// Gets or sets the Vera comment.
        /// </summary>
        /// <value>
        /// The Vera comment.
        /// </value>
        public string Comment { get; set; }
        /// <summary>
        /// Gets or sets the Vera model.
        /// </summary>
        /// <value>
        /// The Vera model.
        /// </value>
        public string Model { get; set; }
        /// <summary>
        /// Gets or sets the Vera serial number.
        /// </summary>
        /// <value>
        /// The Vera serial number.
        /// </value>
        public string SerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the Vera version.
        /// </summary>
        /// <value>
        /// The Vera version.
        /// </value>
        public string Version { get; set; }
    }

    /// <summary>
    /// Vera Scena
    /// </summary>
    [StateObject]
    public class Scene
    {
        /// <summary>
        /// Gets or sets a value indicating whether this scene is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this scene is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the scene identifier.
        /// </summary>
        /// <value>
        /// The scene identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the room.
        /// </summary>
        /// <value>
        /// The room.
        /// </value>
        public string Room { get; set; }
        /// <summary>
        /// Gets or sets the last update.
        /// </summary>
        /// <value>
        /// The last update.
        /// </value>
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Gets or sets the device state.
        /// </summary>
        /// <value>
        /// The device state.
        /// </value>
        public VeraNet.Objects.VeraState State { get; set; }
    }

    /// <summary>
    /// Vera Temperature Sensor
    /// </summary>
    /// <seealso cref="VeraNet.Device" />
    [StateObject]
    public class TemperatureSensor : Device
    {
        /// <summary>
        /// Gets or sets the temperature.
        /// </summary>
        /// <value>
        /// The temperature.
        /// </value>
        public double Temperature { get; set; }
    }

    /// <summary>
    /// Vera Humidity Sensor
    /// </summary>
    /// <seealso cref="VeraNet.Device" />
    [StateObject]
    public class HumiditySensor : Device
    {
        /// <summary>
        /// Gets or sets the humidity.
        /// </summary>
        /// <value>
        /// The humidity.
        /// </value>
        public double Humidity { get; set; }
    }

    /// <summary>
    /// Vera Power Meter
    /// </summary>
    /// <seealso cref="VeraNet.Device" />
    [StateObject]
    public class PowerMeter : Device
    {
        /// <summary>
        /// Gets or sets the device consumption (Watts).
        /// </summary>
        /// <value>
        /// The device consumption (Watts).
        /// </value>
        public double Watts { get; set; }
        /// <summary>
        /// Gets or sets the device total consumption (KWh).
        /// </summary>
        /// <value>
        /// Rhe device total consumption (KWh).
        /// </value>
        public decimal KWh { get; set; }
    }

    [StateObject]
    public class Switch : PowerMeter
    {
        /// <summary>
        /// Gets or sets a device status (On / Off).
        /// </summary>
        /// <value>
        ///   <c>true</c> if Power On; <c>false</c> iff Power Off.
        /// </value>
        public bool Status { get; set; }
    }

    /// <summary>
    /// Vera Dimmable Light
    /// </summary>
    /// <seealso cref="VeraNet.Switch" />
    [StateObject]
    public class DimmableLight : Switch
    {
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level { get; set; }
    }

    /// <summary>
    /// Vera Window Covering
    /// </summary>
    /// <seealso cref="VeraNet.DimmableLight" />
    [StateObject]
    public class WindowCovering : DimmableLight
    {
    }

    /// <summary>
    /// Vera Security Sensor
    /// </summary>
    /// <seealso cref="VeraNet.Device" />
    [StateObject]
    public class SecuritySensor : Device
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SecuritySensor"/> is tripped.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tripped; otherwise, <c>false</c>.
        /// </value>
        public bool Tripped { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SecuritySensor"/> is armed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if armed; otherwise, <c>false</c>.
        /// </value>
        public bool Armed { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SecuritySensor"/> is tripped when armed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tripped when armed; otherwise, <c>false</c>.
        /// </value>
        public bool ArmedTripped { get; set; }
        /// <summary>
        /// Gets or sets the last trip.
        /// </summary>
        /// <value>
        /// The last trip.
        /// </value>
        public DateTime LastTrip { get; set; }
    }

    /// <summary>
    /// Base Vera device class
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the battery level.
        /// </summary>
        /// <value>
        /// The battery level.
        /// </value>
        public double BatteryLevel { get; set; }
        /// <summary>
        /// Gets or sets the room.
        /// </summary>
        /// <value>
        /// The room.
        /// </value>
        public string Room { get; set; }
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; set; }
        /// <summary>
        /// Gets or sets the last update.
        /// </summary>
        /// <value>
        /// The last update.
        /// </value>
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Gets or sets the device state.
        /// </summary>
        /// <value>
        /// The device state.
        /// </value>
        public VeraNet.Objects.VeraState State { get; set; }
    }
}

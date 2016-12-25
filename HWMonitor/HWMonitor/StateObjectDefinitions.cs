/*
 *	 HWMonitor Package for Constellation
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

namespace HWMonitor
{
    using Constellation.Package;
    using OpenHardwareMonitor.Hardware;

    /// <summary>
    /// Represent a Hardware device
    /// </summary>
    [StateObject]
    public class HardwareDevice
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets or sets the hardware type.
        /// </summary>
        /// <value>
        /// The hardware type.
        /// </value>
        public HardwareType Type { get; set; }
    }

    /// <summary>
    /// Represent a HDD
    /// </summary>
    [StateObject]
    public class DiskDrive
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the hardware.
        /// </summary>
        /// <value>
        /// The hardware.
        /// </value>
        public string Hardware { get; set; }
        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public string SerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public string Model { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        public string MediaType { get; set; }
        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer { get; set; }
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        public string Caption { get; set; }

    }

    /// <summary>
    /// Represent a Hardware sensor value
    /// </summary>
    [StateObject]
    public class SensorValue
    {
        /// <summary>
        /// Gets or sets the sensor name.
        /// </summary>
        /// <value>
        /// The sensor name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public float? Value { get; set; }
        /// <summary>
        /// Gets or sets the sensor type.
        /// </summary>
        /// <value>
        /// The sensor type.
        /// </value>
        public SensorType Type { get; set; }
        /// <summary>
        /// Gets the unit of the value.
        /// </summary>
        /// <value>
        /// The unit of the value.
        /// </value>
        public string Unit
        {
            get
            {
                switch (this.Type)
                {
                    case SensorType.Voltage:
                        return "V";
                    case SensorType.Clock:
                        return "MHz";
                    case SensorType.Temperature:
                        return "°C";
                    case SensorType.Load:
                        return "%";
                    case SensorType.Fan:
                        return "RPM";
                    case SensorType.Flow:
                        return "L/h";
                    case SensorType.Control:
                        return "%";
                    case SensorType.Level:
                        return "%";
                    case SensorType.Factor:
                        return "x1";
                    case SensorType.Power:
                        return "W";
                    case SensorType.Data:
                        return "GB";
                    case SensorType.DataRate:
                        return "Bytes/sec";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}

/*
 *	 Wemo Package for Constellation
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

namespace Wemo
{
    using Constellation.Package;
    using System;

    /// <summary>
    /// Represent Wemo Switch or Insight
    /// </summary>
    /// <seealso cref="Wemo.WemoDevice" />
    [StateObject]
    public class WemoSwitch : WemoDevice
    {
        /// <summary>
        /// Gets or sets the signal.
        /// </summary>
        /// <value>
        /// The signal.
        /// </value>
        public int Signal { get; set; }
        /// <summary>
        /// Gets or sets the icon URL.
        /// </summary>
        /// <value>
        /// The icon URL.
        /// </value>
        public string IconUrl { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WemoSwitch"/> is ON or OFF.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ON; otherwise, <c>false</c>.
        /// </value>
        public bool State { get; set; }
        /// <summary>
        /// Gets or sets the last change.
        /// </summary>
        /// <value>
        /// The last change.
        /// </value>
        public DateTime LastChange { get; set; }
        /// <summary>
        /// Gets or sets the time during which the switch is on since the last change.
        /// </summary>
        /// <value>
        /// The time during which the switch is on since the last change.
        /// </value>
        public TimeSpan OnFor { get; set; }
        /// <summary>
        /// Gets or sets the time during which the switch is on today.
        /// </summary>
        /// <value>
        /// The time during which the switch is on today.
        /// </value>
        public TimeSpan OnToday { get; set; }
        /// <summary>
        /// Gets or sets the time during which the switch is on .
        /// </summary>
        /// <value>
        /// The time during which the switch is on.
        /// </value>
        public TimeSpan OnTotal { get; set; }
        /// <summary>
        /// Gets or sets the time period over which averages are calculated.
        /// </summary>
        /// <value>
        /// The time period over which averages are calculated.
        /// </value>
        public TimeSpan TimePeriod { get; set; }
        /// <summary>
        /// Gets or sets the thresold in Watt.
        /// </summary>
        /// <value>
        /// The thresold in Watt.
        /// </value>
        public double Thresold { get; set; }
        /// <summary>
        /// Gets or sets the current power usage in mW.
        /// </summary>
        /// <value>
        /// The current  power usage in mW.
        /// </value>
        public double CurrentMW { get; set; }
        /// <summary>
        /// Gets or sets the power usage in mW for today.
        /// </summary>
        /// <value>
        /// The power usage in mW for today.
        /// </value>
        public double TodayMW { get; set; }
        /// <summary>
        /// Gets or sets the power usage in mW.
        /// </summary>
        /// <value>
        /// Gets or sets the power usage in mW.
        /// </value>
        public double TotalMW { get; set; }
        /// <summary>
        /// Gets the today KWH.
        /// </summary>
        /// <value>
        /// The today KWH.
        /// </value>
        public double TodayKWH { get { return this.TodayMW * 1.6666667e-8;  } }
        /// <summary>
        /// Gets the total KWH.
        /// </summary>
        /// <value>
        /// The total KWH.
        /// </value>
        public double TotalKWH { get { return this.TotalMW * 1.6666667e-8; } }
    }

    /// <summary>
    /// Represent Wemo device
    /// </summary>
    [StateObject]
    public class WemoDevice
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Uri Location { get; set; }
        /// <summary>
        /// Gets or sets the friendly name.
        /// </summary>
        /// <value>
        /// The friendly name.
        /// </value>
        public string FriendlyName { get; set; }
        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        /// <value>
        /// The type of the device.
        /// </value>
        public string DeviceType { get; set; }
        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer { get; set; }
        /// <summary>
        /// Gets or sets the model description.
        /// </summary>
        /// <value>
        /// The model description.
        /// </value>
        public string ModelDescription { get; set; }
        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        /// <value>
        /// The name of the model.
        /// </value>
        public string ModelName { get; set; }
        /// <summary>
        /// Gets or sets the model number.
        /// </summary>
        /// <value>
        /// The model number.
        /// </value>
        public string ModelNumber { get; set; }
        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public string SerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the UDN.
        /// </summary>
        /// <value>
        /// The UDN.
        /// </value>
        public string UDN { get; set; }
        /// <summary>
        /// Gets or sets the UPC.
        /// </summary>
        /// <value>
        /// The UPC.
        /// </value>
        public string UPC { get; set; }
        /// <summary>
        /// Gets or sets the mac address.
        /// </summary>
        /// <value>
        /// The mac address.
        /// </value>
        public string MacAddress { get; set; }
        /// <summary>
        /// Gets or sets the firmware version.
        /// </summary>
        /// <value>
        /// The firmware version.
        /// </value>
        public string FirmwareVersion { get; set; }
    }
}

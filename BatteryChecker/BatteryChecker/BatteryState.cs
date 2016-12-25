/*
 *	 BatteryChecker Package for Constellation
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

namespace BatteryChecker
{
    using Constellation.Package;

    /// <summary>
    /// Represent the BatteryState
    /// </summary>
    [StateObject]
    public class BatteryState
    {
        /// <summary>
        /// Gets the battery device identifier.
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets the battery name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the battery state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets the estimated charge remaining.
        /// </summary>
        public int EstimatedChargeRemaining { get; set; }

        /// <summary>
        /// Gets the estimated run time.
        /// </summary>
        public int EstimatedRunTime { get; set; }

        /// <summary>
        /// Gets the battery status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets the battery status label.
        /// </summary>
        public string StatusLabel
        {
            get
            {
                switch (this.StatusCode)
                {
                    case 1: return "Battery is discharging";
                    case 2: return "Plugged to AC";
                    case 3: return "Fully Charged";
                    case 4: return "Low";
                    case 5: return "Critical";
                    case 6: return "Charging";
                    case 7: return "Charging and High";
                    case 8: return "Charging and Low";
                    case 9: return "Charging and Critical ";
                    case 10: return "Unknown State";
                    case 11: return "Partially Charged";
                    default: return "Unknown";
                }
            }
        }
    }
}

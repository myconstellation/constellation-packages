/*
 *	 PoolCop connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018-2019 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace PoolCop.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// PoolCop settings
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// All settings related to filtration
        /// </summary>
        [PoolCopilotProperty("filter")]
        public Filter Filter { get; set; }

        /// <summary>
        /// All settings related to the pool
        /// </summary>
        [PoolCopilotProperty("pool")]
        public PoolClass Pool { get; set; }

        /// <summary>
        /// All settings related to the pump
        /// </summary>
        [PoolCopilotProperty("pump")]
        public Pump Pump { get; set; }

        /// <summary>
        /// All settings related to the pH Injection
        /// </summary>
        [PoolCopilotProperty("ph")]
        public Ph Ph { get; set; }

        /// <summary>
        /// All settings related to the ORP Control
        /// </summary>
        [PoolCopilotProperty("orp")]
        public Orp Orp { get; set; }

        /// <summary>
        /// All settings related to the Water Level Control
        /// </summary>
        [PoolCopilotProperty("waterlevel")]
        public Waterlevel Waterlevel { get; set; }

        /// <summary>
        /// All settings related to the AutChlor
        /// </summary>
        [PoolCopilotProperty("autochlor")]
        public Autochlor Autochlor { get; set; }

        /// <summary>
        /// All settings related to the Ioniser
        /// </summary>
        [PoolCopilotProperty("ioniser")]
        public Ioniser Ioniser { get; set; }
    }

    /// <summary>
    /// All settings related to the AutoChlor
    /// </summary>
    public partial class Autochlor
    {
        /// <summary>
        /// Is Mode auto enabled
        /// </summary>
        [PoolCopilotProperty("auto")]
        public bool AutoMode { get; set; }

        /// <summary>
        /// Acid Status
        /// </summary>
        [PoolCopilotProperty("acid")]
        public int Acid { get; set; }

        /// <summary>
        /// Minimum Injection Duration in Percent per filtration duration
        /// </summary>
        [PoolCopilotProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// The duration of the next injection (in seconds)
        /// </summary>
        [PoolCopilotProperty("next_injection")]
        public int NextInjection { get; set; }
    }

    /// <summary>
    /// All settings related to filtration
    /// </summary>
    public partial class Filter
    {
        /// <summary>
        /// Is the cleaning cycle automatic ?
        /// </summary>
        [PoolCopilotProperty("mode")]
        public bool AutoMode { get; set; }

        /// <summary>
        /// The pressure (kPa) that trigger a cleaning cycle
        /// </summary>
        [PoolCopilotProperty("pressure")]
        public int Pressure { get; set; }

        /// <summary>
        /// The backwash duration (seconds)
        /// </summary>
        [PoolCopilotProperty("backwash_duration")]
        public int BackwashDuration { get; set; }

        /// <summary>
        /// The rinse duration (seconds)
        /// </summary>
        [PoolCopilotProperty("rinse_duration")]
        public int RinseDuration { get; set; }

        /// <summary>
        /// The number of days between 2 cleaning cycle
        /// </summary>
        [PoolCopilotProperty("max_days")]
        public int MaxDays { get; set; }

        /// <summary>
        /// A valve to the waste is Installed or not
        /// </summary>
        [PoolCopilotProperty("waste_valve")]
        public bool WasteValve { get; set; }

        /// <summary>
        /// The method used for cycle 2 duration
        /// </summary>
        [PoolCopilotProperty("timer")]
        public int Timer { get; set; }
    }

    /// <summary>
    /// All settings related to Ioniser
    /// </summary>
    public partial class Ioniser
    {
        /// <summary>
        /// Is the ioniser automatic ?
        /// </summary>
        [PoolCopilotProperty("mode")]
        public bool AutoMode { get; set; }

        /// <summary>
        /// The current mode.
        /// </summary>
        [PoolCopilotProperty("current")]
        public int Current { get; set; }

        /// <summary>
        /// Minimum Injection Duration in Percent per filtration duration
        /// </summary>
        [PoolCopilotProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// The duration of the next injection (in seconds)
        /// </summary>
        [PoolCopilotProperty("next_injection")]
        public int NextInjection { get; set; }
    }

    /// <summary>
    /// All settings related to the ORP Control
    /// </summary>
    public partial class Orp
    {
        /// <summary>
        /// ORP Set Point
        /// </summary>
        [PoolCopilotProperty("set_point")]
        public int SetPoint { get; set; }

        /// <summary>
        /// Disinfectant Type
        /// </summary>
        [PoolCopilotProperty("disinfectant")]
        public int Disinfectant { get; set; }

        /// <summary>
        /// Is controlable.
        /// </summary>
        [PoolCopilotProperty("control")]
        public bool Control { get; set; }

        /// <summary>
        /// Tthe duration of the next injection (in seconds)
        /// </summary>
        [PoolCopilotProperty("next_injection")]
        public int NextInjection { get; set; }

        /// <summary>
        /// Hyperchloration Set Point
        /// </summary>
        [PoolCopilotProperty("hyper_set_point")]
        public int HyperSetPoint { get; set; }

        /// <summary>
        /// Hyperchloration Week Day (0 is for Null, 1 for Monday to 7 for Sunday)
        /// </summary>
        [PoolCopilotProperty("hyper_day")]
        public int HyperDay { get; set; }

        /// <summary>
        /// Stop ORP Control is temperature is lower then this temperature in Celsius
        /// </summary>
        [PoolCopilotProperty("temperature_shutdown")]
        public int TemperatureShutdown { get; set; }
    }

    /// <summary>
    /// All settings related to the pH Injection
    /// </summary>
    public partial class Ph
    {
        /// <summary>
        /// pH Set Point.
        /// </summary>
        [PoolCopilotProperty("set_point")]
        public int SetPoint { get; set; }

        /// <summary>
        /// pH type can be pH- pH+ or read-only (0,1,2)
        /// </summary>
        [PoolCopilotProperty("type")]
        public PHType Type { get; set; }

        /// <summary>
        /// Maximum dosing duration (in minutes)
        /// </summary>
        [PoolCopilotProperty("dosing_time")]
        public int DosingTime { get; set; }

        /// <summary>
        /// The duration of the next injection(in seconds)
        /// </summary>
        [PoolCopilotProperty("next_injection")]
        public int NextInjection { get; set; }
    }

    /// <summary>
    /// The Pool class.
    /// </summary>
    public partial class PoolClass
    {
        /// <summary>
        /// Volume of the Pool
        /// </summary>
        [PoolCopilotProperty("volume")]
        public int Volume { get; set; }

        /// <summary>
        /// The number of turn over per day (used by timers)
        /// </summary> 
        [PoolCopilotProperty("turnover")]
        public int Turnover { get; set; }

        /// <summary>
        /// Is the Freeze protection enabled
        /// </summary>
        [PoolCopilotProperty("freeze_protection")]
        public bool FreezeProtection { get; set; }

        /// <summary>
        /// % of treatment reduction when the cover is closed
        /// </summary>
        [PoolCopilotProperty("cover_reduction")]
        public int CoverReduction { get; set; }

        /// <summary>
        /// Type of Pool (Classic, Spa, Infinity A, Infinity B)
        /// </summary>
        [PoolCopilotProperty("type")]
        public PoolType Type { get; set; }

        /// <summary>
        /// Service Mode is enabled
        /// </summary>
        [PoolCopilotProperty("service")]
        public bool Service { get; set; }
    }

    /// <summary>
    /// All settings related to the pump
    /// </summary>
    public partial class Pump
    {
        /// <summary>
        /// Pressure Low when protection is enabled
        /// </summary>
        [PoolCopilotProperty("pressure_low")]
        public bool PressureLow { get; set; }

        /// <summary>
        /// Pressure Alarm
        /// </summary>
        [PoolCopilotProperty("pressure_alarm")]
        public int PressureAlarm { get; set; }

        /// <summary>
        /// Stop the pump when pressure is under Pressure Low
        /// </summary>
        [PoolCopilotProperty("protect")]
        public bool Protect { get; set; }

        /// <summary>
        /// The pump type.
        /// </summary>
        [PoolCopilotProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// The Flow rate
        /// </summary>
        [PoolCopilotProperty("flowrate")]
        public int Flowrate { get; set; }

        /// <summary>
        /// Speed Cycle 1
        /// </summary>
        [PoolCopilotProperty("speed_cycle1")]
        public int SpeedCycle1 { get; set; }

        /// <summary>
        /// Speed Cycle 2
        /// </summary>
        [PoolCopilotProperty("speed_cycle2")]
        public int SpeedCycle2 { get; set; }

        /// <summary>
        /// Speed Backwash
        /// </summary>
        [PoolCopilotProperty("speed_backwash")]
        public int SpeedBackwash { get; set; }

        /// <summary>
        /// Speed Cover closed
        /// </summary>
        [PoolCopilotProperty("speed_cover")]
        public int SpeedCover { get; set; }
    }

    /// <summary>
    /// All settings related to the Water Level Control
    /// </summary>
    public partial class Waterlevel
    {
        /// <summary>
        /// Automatically add water if level is low
        /// </summary>
        [PoolCopilotProperty("auto_add")]
        public bool AutoAdd { get; set; }

        /// <summary>
        /// The cable status.
        /// </summary>
        [PoolCopilotProperty("cable_status")]
        public int CableStatus { get; set; }

        /// <summary>
        /// Can add water when the pump is ON
        /// </summary>
        [PoolCopilotProperty("continuous")]
        public bool Continuous { get; set; }

        /// <summary>
        /// The maximum refill duration is minutes
        /// </summary>
        [PoolCopilotProperty("max_duration")]
        public int MaxDuration { get; set; }

        /// <summary>
        /// Reduce level automatically is level is very high
        /// </summary>
        [PoolCopilotProperty("auto_reduce")]
        public bool AutoReduce { get; set; }

        /// <summary>
        /// The maximum reduce duration in seconds
        /// </summary>
        [PoolCopilotProperty("draining_duration")]
        public int DrainingDuration { get; set; }
    }
}


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
    /// <summary>
    /// Pool types
    /// </summary>
    public enum PoolType
    {
        /// <summary>
        /// Classic pool
        /// </summary>
        Classic = 0,
        /// <summary>
        /// Infinity A
        /// </summary>
        InfinityA = 1,
        /// <summary>
        /// Infinity B
        /// </summary>
        InfinityB = 2,
        /// <summary>
        /// SPA
        /// </summary>
        SPA = 3
    }

    /// <summary>
    /// Forced pump modes
    /// </summary>
    public enum ForcedPumpMode
    {
        /// <summary>
        /// Not forced
        /// </summary>
        None = 0,
        /// <summary>
        /// Forced for 24h
        /// </summary>
        Forced24H = 1,
        /// <summary>
        /// Forced for 48h
        /// </summary>
        Forced48H = 2,
        /// <summary>
        /// Forced for 72h
        /// </summary>
        Forced72H = 3,
    }

    /// <summary>
    /// Water level states
    /// </summary>
    public enum WaterlevelState
    {
        /// <summary>
        /// Not installed
        /// </summary>
        NotInstalled = 0,
        /// <summary>
        /// Low level
        /// </summary>
        Low = 1,
        /// <summary>
        /// Normal
        /// </summary>
        Normal = 2,
        /// <summary>
        /// High level
        /// </summary>
        High = 3,
        /// <summary>
        /// Error
        /// </summary>
        Error = 4
    }

    /// <summary>
    /// PoolCop running status
    /// </summary>
    public enum RunningStatus
    {
        /// <summary>
        /// PoolCop stopped
        /// </summary>
        Stop = 0,
        /// <summary>
        /// PoolCop freezed
        /// </summary>
        Freeze = 1,
        /// <summary>
        /// PoolCop forced
        /// </summary>
        Forced = 2,
        /// <summary>
        /// Auto mode
        /// </summary>
        Auto = 3,
        /// <summary>
        /// Timers mode
        /// </summary>
        Timer = 4,
        /// <summary>
        /// Manual mode
        /// </summary>
        Manual = 5,
        /// <summary>
        /// PoolCop paused
        /// </summary>
        Paused = 6,
        /// <summary>
        /// External mode
        /// </summary>
        External = 7
    }

    /// <summary>
    /// The valve positions
    /// </summary>
    public enum ValvePosition
    {
        /// <summary>
        /// Filter.
        /// </summary>
        Filter = 0,
        /// <summary>
        /// Waste
        /// </summary>
        Waste = 1,
        /// <summary>
        /// Closed
        /// </summary>
        Closed = 2,
        /// <summary>
        /// Backwash
        /// </summary>
        Backwash = 3,
        /// <summary>
        /// Bypass
        /// </summary>
        ByPass = 4,
        /// <summary>
        /// Rinse
        /// </summary>
        Rinse = 5,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 6,
        /// <summary>
        /// None
        /// </summary>
        None = 7
    }

    /// <summary>
    /// Water valve positions
    /// </summary>
    public enum WaterValvePosition
    {
        /// <summary>
        /// Standby
        /// </summary>
        Standby = 0,
        /// <summary>
        /// Refill in progress
        /// </summary>
        Refill = 1,
        /// <summary>
        /// Measure in progress
        /// </summary>
        Measure = 2
    }

    /// <summary>
    /// pH types
    /// </summary>
    public enum PHType
    {
        /// <summary>
        /// pH-
        /// </summary>
        PH_Minus = 0,
        /// <summary>
        /// pH+
        /// </summary>
        PH_Plus = 1,
        /// <summary>
        /// Read-only
        /// </summary>
        ReadOnly = 2
    }
}

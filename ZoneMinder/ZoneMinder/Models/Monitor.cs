/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016-2019 - Sebastien Warin <http://sebastien.warin.fr>  
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

namespace ZoneMinder
{
    using Constellation.Package;

    /// <summary>
    /// ZoneMinder monitor for ZM 1.29/1.30
    /// </summary>
    /// <seealso cref="ZoneMinder.MonitorBase" />
    [StateObject]
    public class Monitor : MonitorBase
    {
        /// <summary>
        /// First event ID
        /// </summary>
        public int MinEventId { get; set; }
        /// <summary>
        /// Last event ID
        /// </summary>
        public int MaxEventId { get; set; }
    }

    /// <summary>
    /// ZoneMinder monitor for ZM 1.32 and above
    /// </summary>
    /// <remarks>The API returns a lot of properties. Just select the most important for a common use !</remarks>
    /// <seealso cref="ZoneMinder.MonitorBase" />
    [StateObject]
    public class Monitor2 : MonitorBase
    {
        /// <summary>
        /// Current state of the monitor
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Server ID (for multiserver installation) 
        /// </summary>
        public int ServerID { get; set; }
        /// <summary>
        /// Storage ID (for multistorage installation)
        /// </summary>
        public int StorageID { get; set; }
        /// <summary>
        /// Capture method for this monitor
        /// </summary>
        public string CaptureMethod { get; set; }
        /// <summary>
        /// Capture path for this monitor
        /// </summary>
        public string CapturePath { get; set; }
        /// <summary>
        /// Capture options for this monitor
        /// </summary>
        public string CaptureOptions { get; set; }
        /// <summary>
        /// Current capture bandwidth
        /// </summary>
        public long CaptureBandwidth { get; set; }
        /// <summary>
        /// Current analysis frame rate (FPS)
        /// </summary>
        public decimal AnalysisFPS { get; set; }
        /// <summary>
        /// Number of zones
        /// </summary>
        public int ZoneCount { get; set; }
        /// <summary>
        /// Number of events archived
        /// </summary>
        public int ArchivedEvents { get; set; }
        /// <summary>
        /// Number of events in the month
        /// </summary>
        public int MonthEvents { get; set; }
        /// <summary>
        /// Number of events in the week
        /// </summary>
        public int WeekEvents { get; set; }
        /// <summary>
        /// Number of events today
        /// </summary>
        public int DayEvents { get; set; }
        /// <summary>
        /// Number of events for the last hour
        /// </summary>
        public int HourEvents { get; set; }
        /// <summary>
        /// Is the monitor controllable (PTZ)
        /// </summary>
        public bool Controllable { get; set; }
    }

    /// <summary>
    /// Base class for Monitor
    /// </summary>
    public abstract class MonitorBase
    {
        /// <summary>
        /// The Monitor's ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Monitor's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Monitor's type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The Monitor's function
        /// </summary>
        public MonitorFunction Function { get; set; }
        /// <summary>
        /// Is the monitor enabled
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Capture Width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Capture Height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Maximum FPS (0 for unlimited)
        /// </summary>
        public decimal MaxFPS { get; set; }
        /// <summary>
        /// Current alarm state
        /// </summary>
        public AlarmState AlarmState { get; set; }
        /// <summary>
        /// Current capture frame rate (FPS)
        /// </summary>
        public decimal FrameRate { get; set; }
        /// <summary>
        /// Number of events recorded for this monitor
        /// </summary>
        public int TotalEvents { get; set; }
        /// <summary>
        /// Space used for the events recorded
        /// </summary>
        public decimal SpaceUsed { get; set; }
    }
}

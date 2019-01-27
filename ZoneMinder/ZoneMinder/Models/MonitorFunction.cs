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
    /// <summary>
    /// Defines what the monitor is doing.
    /// </summary>
    public enum MonitorFunction
    {
        /// <summary>
        /// The monitor is currently disabled. No streams can be viewed or events generated. Nothing is recorded.
        /// </summary>
        None,
        /// <summary>
        /// The monitor is only available for live streaming. No image analysis is done so no alarms or events will be generated, and nothing will be recorded.
        /// </summary>
        Monitor,
        /// <summary>
        /// MOtion DEteCTtion. All captured images will be analysed and events generated with recorded video where motion is detected.
        /// </summary>
        Modect,
        /// <summary>
        /// The monitor will be continuously recorded. Events of a fixed-length will be generated regardless of motion, analogous to a conventional time-lapse video recorder. No motion detection takes place in this mode.
        /// </summary>
        Record,
        /// <summary>
        /// The monitor will be continuously recorded, with any motion being highlighted within those events.
        /// </summary>
        Mocord,
        /// <summary>
        /// No DEteCTtion. This is a special mode designed to be used with external triggers. In Nodect no motion detection takes place but events are recorded if external triggers require it.
        /// </summary>
        Nodect
    }
}

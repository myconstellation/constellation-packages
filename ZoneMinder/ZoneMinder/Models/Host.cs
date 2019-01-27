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
    /// ZoneMinder host
    /// </summary>
    [StateObject]
    public class Host
    {
        /// <summary>
        /// ZoneMinder root URI
        /// </summary>
        public string URI { get; set; }
        /// <summary>
        /// Current authentification token
        /// </summary>
        public string AuthentificationToken { get; set; }
        /// <summary>
        /// ZoneMinder version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// ZoneMinder API version
        /// </summary>
        public string APIVersion { get; set; }
        /// <summary>
        /// ZoneMinder CPU load average for the last minute
        /// </summary>
        public double LoadAverageLastMinute { get; set; }
        /// <summary>
        /// ZoneMinder CPU load average for the last 5 minutes
        /// </summary>
        public double LoadAverageLastFiveMinutes { get; set; }
        /// <summary>
        /// ZoneMinder CPU load average for the last 15 minutes
        /// </summary>
        public double LoadAverageLastFifteenMinutes { get; set; }
        /// <summary>
        /// Is the ZM daemon started
        /// </summary>
        public bool DaemonStarted { get; set; }
        /// <summary>
        /// Total space used by all events recorded
        /// </summary>
        public decimal SpaceUsed { get; set; }
    }
}

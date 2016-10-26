/*
 *	 RATP Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Hydro
 *	 Copyright (C) 2016 - Pierre Grimaud <https://github.com/pgrimaud>
 *	 Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Ratp.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represent the schedule result
    /// </summary>
    public class ScheduleResult
    {
        /// <summary>
        /// Gets or sets the schedule information.
        /// </summary>
        /// <value>
        /// The schedule information.
        /// </value>
        [RatpProperty("informations")]
        public RouteInformation Information { get; set; }
        /// <summary>
        /// Gets or sets the schedules.
        /// </summary>
        /// <value>
        /// The schedules.
        /// </value>
        [RatpProperty("schedules")]
        public List<Schedule> Schedules { get; set; }
    }

    /// <summary>
    /// Represent a schedule 
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        [RatpProperty("destination")]
        public string Destination { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [RatpProperty("message")]
        public string Message { get; set; }
    }
}

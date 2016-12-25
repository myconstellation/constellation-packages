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
    /// Represent a Stations list
    /// </summary>
    public class StationList
    {
        /// <summary>
        /// Gets or sets the stations.
        /// </summary>
        /// <value>
        /// The stations.
        /// </value>
        [RatpProperty("stations")]
        public List<Station> Stations { get; set; }
    }

    /// <summary>
    /// Represent a Station
    /// </summary>
    public class Station
    {
        /// <summary>
        /// Gets or sets the Station's identifier.
        /// </summary>
        /// <value>
        /// The Station's identifier.
        /// </value>
        [RatpProperty("id", "id_station", "id_destination")]
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the Station's name.
        /// </summary>
        /// <value>
        /// The Station's name.
        /// </value>
        [RatpProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the Station's slug.
        /// </summary>
        /// <value>
        /// The Station's slug.
        /// </value>
        [RatpProperty("slug")]
        public string Slug { get; set; }
    }
}

/*
 *	 Paradox connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Paradox
{
    using System;

    /// <summary>
    /// Base class for Paradox item (Zone, Area and User)
    /// </summary>
    public abstract class BaseParadoxItem
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The item type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The last activity.
        /// </summary>
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// Updates the name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void UpdateName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.Name = name.Trim();
            }
        }
    }    
}

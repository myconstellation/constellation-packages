﻿/*
 *	 Paradox .NET library
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Sebastien Warin under one or more contributor
 *	 license agreements. Sebastien Warin licenses this file to you under
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

namespace Paradox.Events
{
    /// <summary>
    /// Represent a Status event
    /// </summary>
    /// <typeparam name="TEventType">The type of the status.</typeparam>
    /// <seealso cref="Paradox.Core.ParadoxSystemEventArgs" />
    public class StatusEventArgs<TEventType> : ParadoxSystemEventArgs
    {
        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        public Area Area { get { return (Area)this.AreaNumber; } }

        /// <summary>
        /// Gets the type of the status.
        /// </summary>
        /// <value>
        /// The type of the status.
        /// </value>
        public TEventType StatusType { get { return (TEventType)(object)this.EventNumber; } }
    }
}

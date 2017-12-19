/*
 *	 OrangeTV Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace OrangeTV
{
    /// <summary>
    /// Represent the state of the Orange's set-top box
    /// </summary>
    public class State
    {
        /// <summary>
        /// The time shifting state.
        /// </summary>
        [OrangeJsonProperty("timeShiftingState")]
        public bool TimeShiftingState { get; set; }

        /// <summary>
        /// The type of the played media (ex: LIVE, ...)
        /// </summary>
        [OrangeJsonProperty("playedMediaType")]
        public string PlayedMediaType { get; set; }

        /// <summary>
        /// The state of the played media (ex: PLAY, PAUSE, REWIND, NA, ...)
        /// </summary>
        [OrangeJsonProperty("playedMediaState")]
        public string PlayedMediaState { get; set; }

        /// <summary>
        /// The played media identifier
        /// </summary>
        [OrangeJsonProperty("playedMediaId")]
        public int PlayedMediaId { get; set; }

        /// <summary>
        /// The played channel name
        /// </summary>
        public string PlayedChannelName => this.PlayedMediaId.GetOrangeServiceValue<Channel>().ToString();

        /// <summary>
        /// The played media context identifier
        /// </summary>
        [OrangeJsonProperty("playedMediaContextId")]
        public int PlayedMediaContextId { get; set; }

        /// <summary>
        /// The played media position
        /// </summary>
        [OrangeJsonProperty("playedMediaPosition")]
        public string PlayedMediaPosition { get; set; }
        
        /// <summary>
        /// The current context
        /// </summary>
        [OrangeJsonProperty("osdContext")]
        public string CurrentContext { get; set; }

        /// <summary>
        /// The MAC address.
        /// </summary>
        [OrangeJsonProperty("macAddress")]
        public string MacAddress { get; set; }

        /// <summary>
        /// Is the WOL is supported.
        /// </summary>
        [OrangeJsonProperty("wolSupport")]
        public bool WOLSupport { get; set; }

        /// <summary>
        /// The friendly name of the STB
        /// </summary>
        [OrangeJsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Is the set-top box is in standby mode
        /// </summary>
        [OrangeJsonProperty("activeStandbyState")]
        public bool StandbyState { get; set; }
    }
}

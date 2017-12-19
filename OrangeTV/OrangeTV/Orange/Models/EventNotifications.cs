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
    using System;

    /// <summary>
    /// Represent an OSD_CONTEXT_CHANGED notification
    /// </summary>
    /// <seealso cref="OrangeTV.EventNotification" />
    [EventNotification("OSD_CONTEXT_CHANGED")]
    public class ContextChanged : EventNotification
    {
        /// <summary>
        /// The new context (ex: HOMEPAGE, MAIN_PROCESS, ...)
        /// </summary>
        [OrangeJsonProperty("service")]
        public string Service { get; set; }

        /// <summary>
        /// Updates the state with the OSD_CONTEXT_CHANGED notification.
        /// </summary>
        /// <param name="state">The current state to update.</param>
        public override void UpdateState(State state)
        {
            state.CurrentContext = this.Service;
            if (this.Service != "LIVE")
            {
                state.TimeShiftingState = false;
                state.PlayedMediaContextId = 0;
                state.PlayedMediaId = 0;
                state.PlayedMediaPosition = null;
                state.PlayedMediaState = null;
                state.PlayedMediaType = null;
            }
        }
    }

    /// <summary>
    /// Represent an MEDIA_STATE_CHANGED notification
    /// </summary>
    /// <seealso cref="OrangeTV.EventNotification" />
    [EventNotification("MEDIA_STATE_CHANGED")]
    public class MediaStateChanged : EventNotification
    {
        /// <summary>
        /// The type of the played media (ex: LIVE, ...)
        /// </summary>
        [OrangeJsonProperty("mediaType")]
        public string PlayedMediaType { get; set; }

        /// <summary>
        /// The state of the played media (ex: PLAY, PAUSE, REWIND, NA, ...)
        /// </summary>
        [OrangeJsonProperty("state")]
        public string PlayedMediaState { get; set; }

        /// <summary>
        /// The played media identifier
        /// </summary>
        [OrangeJsonProperty("id")]
        public int PlayedMediaId { get; set; }

        /// <summary>
        /// The played channel name
        /// </summary>
        public string PlayedChannelName => this.PlayedMediaId.GetOrangeServiceValue<Channel>().ToString();

        /// <summary>
        /// The played media context identifier
        /// </summary>
        [OrangeJsonProperty("contextId")]
        public int PlayedMediaContextId { get; set; }

        /// <summary>
        /// The played media position
        /// </summary>
        [OrangeJsonProperty("position")]
        public string PlayedMediaPosition { get; set; }

        /// <summary>
        /// Updates the state with the MEDIA_STATE_CHANGED notification.
        /// </summary>
        /// <param name="state">The current state to update.</param>
        public override void UpdateState(State state)
        {
            state.PlayedMediaContextId = this.PlayedMediaContextId;
            state.PlayedMediaId = this.PlayedMediaId;
            state.PlayedMediaPosition = this.PlayedMediaPosition;
            state.PlayedMediaState = this.PlayedMediaState;
            state.PlayedMediaType = this.PlayedMediaType;
        }
    }

    /// <summary>
    /// Represent an TIME_SHIFTING notification
    /// </summary>
    /// <seealso cref="OrangeTV.EventNotification" />
    [EventNotification("TIME_SHIFTING")]
    public class TimeShiftingChanged : EventNotification
    {
        /// <summary>
        /// The time shifting state.
        /// </summary>
        [OrangeJsonProperty("state")]
        public bool TimeShiftingState { get; set; }

        /// <summary>
        /// Updates the state with the TIME_SHIFTING notification.
        /// </summary>
        /// <param name="state">The current state to update.</param>
        public override void UpdateState(State state)
        {
            state.TimeShiftingState = this.TimeShiftingState;
        }
    }

    /// <summary>
    /// Represent a base event notification
    /// </summary>
    public class EventNotification
    {
        /// <summary>
        /// The type of the event (ex: OSD_CONTEXT_CHANGED, MEDIA_STATE_CHANGED, ...).
        /// </summary>
        [OrangeJsonProperty("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="state">The current state to update.</param>
        public virtual void UpdateState(State state) { }
    }

    /// <summary>
    /// Maps an event type
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class EventNotificationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        /// <value>
        /// The event type.
        /// </value>
        public string EventType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventNotificationAttribute" /> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        public EventNotificationAttribute(string eventType)
        {
            this.EventType = eventType;
        }
    }
}
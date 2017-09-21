/*
 *	 Exchange Package for Constellation
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

namespace Exchange
{
    using Constellation.Package;
    using System;

    /// <summary>
    /// Represents an appointment or a meeting.
    /// </summary>
    [StateObject]
    public class Appointment
    {
        /// <summary>
        /// The unique identifier of the appointment.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The subject of this appointment.
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// The start time of the appointment.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// The end time of the appointment.
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// The location of this appointment.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// The duration of this appointment.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    ///  Represents a request to send a mail message.
    /// </summary>
    public class SendMailRequest
    {
        /// <summary>
        /// The subject of this  e-mail message.
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// The list of To recipients for the e-mail message (semicolon separated list)
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// The body of this e-mail message.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// The importance of this item.
        /// </summary>
        public bool IsImportant { get; set; }
    }
}

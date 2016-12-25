/*
 *	 Pushover Package for Constellation
 *	 Web site: http://www.myConstellation.io
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

namespace Pushover
{
    /// <summary>
    /// Emergency options
    /// </summary>
    public class EmergencyOptions
    {
        /// <summary>
        /// Specifies how often (in seconds) the Pushover servers will send the same notification to the user if the notification has not been acknowledged and is not expired. This parameter must have a value of at least 30 seconds between retries.
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// Specifies how many seconds your notification will continue to be retried for (every retry seconds). If the notification has not been acknowledged in expire seconds, it will be marked as expired and will stop being sent to the user. This parameter must have a maximum value of at most 86400 seconds (24 hours).
        /// </summary>
        public int Expire { get; set; }
    }

    /// <summary>
    /// URL to show with your message. 
    /// </summary>
    public class Url
    {
        /// <summary>
        /// A supplementary URL to show with your message.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// A title for your supplementary URL, otherwise just the URL is shown.
        /// </summary>
        public string Title { get; set; }
    }
}

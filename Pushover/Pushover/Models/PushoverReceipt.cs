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
    using Newtonsoft.Json;

    /// <summary>
    /// Pushover Receipt
    /// </summary>
    public class PushoverReceipt
    {
        /// <summary>
        /// Gets or sets a value indicating if the request was successful
        /// </summary>
        /// <value>
        ///   <c>true</c> if status is OK; otherwise, <c>false</c>.
        /// </value>
        [PushoverProperty("status"), JsonConverter(typeof(BoolConverter))]
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has acknowledged the notification.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the user has acknowledged the notification; otherwise, <c>false</c>.
        /// </value>
        [PushoverProperty("acknowledged"), JsonConverter(typeof(BoolConverter))]
        public bool Acknowledged { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp of when the user acknowledged, or 0
        /// </summary>
        /// <value>
        /// The acknowledged timestamp.
        /// </value>
        [PushoverProperty("acknowledged_at")]
        public int AcknowledgedTimestamp { get; set; }
        
        /// <summary>
        /// Gets or sets the user key of the user that first acknowledged the notification
        /// </summary>
        /// <value>
        /// The user key of the user that first acknowledged the notification.
        /// </value>
        [PushoverProperty("acknowledged_by")]
        public string AcknowledgedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the device name of the user that first acknowledged the notification.
        /// </summary>
        /// <value>
        /// The device name of the user that first acknowledged the notification.
        /// </value>
        [PushoverProperty("acknowledged_by_device")]
        public string AcknowledgedDevice { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp of when the notification was last retried, or 0
        /// </summary>
        /// <value>
        /// The Unix timestamp of when the notification was last retried, or 0
        /// </value>
        [PushoverProperty("last_delivered_at")]
        public int LastDeliveredTimestamp { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the expiration date has passed
        /// </summary>
        /// <value>
        ///   <c>true</c> whether the expiration date has passed; otherwise, <c>false</c>.
        /// </value>
        [PushoverProperty("expired"), JsonConverter(typeof(BoolConverter))]
        public bool Expired { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp of when the notification will stop being retried
        /// </summary>
        /// <value>
        /// The Unix timestamp of when the notification will stop being retried
        /// </value>
        [PushoverProperty("expires_at")]
        public int ExpiresTimestamp { get; set; }
    }
}

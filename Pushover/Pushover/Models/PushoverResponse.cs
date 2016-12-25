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
    /// Pushover Response
    /// </summary>
    public class PushoverResponse
    {
        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        [PushoverProperty("errors")]
        public string[] Errors { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating if the request was successful
        /// </summary>
        /// <value>
        ///   <c>true</c> if status is OK; otherwise, <c>false</c>.
        /// </value>
        [PushoverProperty("status"), JsonConverter(typeof(BoolConverter))]
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [PushoverProperty("request")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the receipt identifier if you sent an emergency-priority notification,
        /// </summary>
        /// <value>
        /// The receipt identifier.
        /// </value>
        [PushoverProperty("receipt")]
        public string ReceiptId { get; set; }
    }
}

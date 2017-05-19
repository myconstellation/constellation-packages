/*
 *	 NetworkTools package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace NetworkTools
{
    using System.Net;

    /// <summary>
    /// Standard .NET WebClient with request timeout
    /// </summary>
    /// <seealso cref="System.Net.WebClient" />
    public class ExtendedWebClient : WebClient
    {
        /// <summary>
        /// The standard HTTP Request Timeout default
        /// </summary>
        public const int DEFAULT_TIMEOUT = 100000;

        /// <summary>
        /// Gets or sets the HTTP Request timeout.
        /// </summary>
        /// <value>
        /// The HTTP Request timeout.
        /// </value>
        public int Timeout { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.Net.WebRequest" /> object for the specified resource.
        /// </summary>
        /// <param name="address">A <see cref="T:System.Uri" /> that identifies the resource to request.</param>
        /// <returns>
        /// A new <see cref="T:System.Net.WebRequest" /> object for the specified resource.
        /// </returns>
        protected override WebRequest GetWebRequest(System.Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedWebClient"/> class.
        /// </summary>
        /// <param name="timeout">The HTTP Request timeout.</param>
        public ExtendedWebClient(int timeout = DEFAULT_TIMEOUT)
        {
            this.Timeout = (timeout > 0) ? timeout : DEFAULT_TIMEOUT;
        }
    }
}

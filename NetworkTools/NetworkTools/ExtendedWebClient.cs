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

    public class ExtendedWebClient : WebClient
    {
        public int timeout;

        protected override WebRequest GetWebRequest(System.Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = timeout;
            }
            return request;
        }

        public ExtendedWebClient()
        {
            timeout = 100000; // the standard HTTP Request Timeout default
        }
    }
}

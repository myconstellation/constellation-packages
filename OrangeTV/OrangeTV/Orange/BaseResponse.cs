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
    /// Container for all response from Orange STB service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [OrangeJsonProperty("result")]
        public BaseResult<T> Result { get; set; }
    }

    /// <summary>
    /// Represent the response code from Orange STB service  
    /// </summary>
    public enum ResponseCode : int
    {
        /// <summary>
        /// The Bad/Invalid Request code
        /// </summary>
        BadRequest = -11,
        /// <summary>
        /// The Not Found code
        /// </summary>
        NotFound = -10,
        /// <summary>
        /// The Unknown code
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// The OK code
        /// </summary>
        OK = 0,
        /// <summary>
        /// The event notification code
        /// </summary>
        EventNotification = 1,
        /// <summary>
        /// The timeout code
        /// </summary>
        TimeOut = 2
    }

    /// <summary>
    /// Represent the base result from Orange STB service 
    /// </summary>
    /// <typeparam name="T">Data specific type</typeparam>
    public class BaseResult<T>
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>
        /// The response code.
        /// </value>
        [OrangeJsonProperty("responseCode")]
        public ResponseCode Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [OrangeJsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [OrangeJsonProperty("data")]
        public T Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResult{T}"/> class.
        /// </summary>
        public BaseResult()
        {
            this.Code = ResponseCode.Unknown;
        }
    }
}

/*
 *	 Huawei Mobile Router Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace HuaweiMobileRouter
{
    using HuaweiMobileRouter.Models;
    using System;

    /// <summary>
    /// Router error exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RouterErrorException : Exception
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public Error Error { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterErrorException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public RouterErrorException(Error error)
            : base(error.ToString())
        {
            this.Error = error;
        }
    }
}

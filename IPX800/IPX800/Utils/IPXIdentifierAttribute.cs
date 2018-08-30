/*
 *	 GCE Electronics IPX800 Package for Constellation
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

namespace IPX800
{
    using IPX800.Enumerations;
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// IPX identifier attribute use to identify class or enum value. 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class IPXIdentifierAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the IPX element's type.
        /// </summary>
        /// <value>
        /// The IPX element's type.
        /// </value>
        public IPXElementType ElementType { get; set; }

        /// <summary>
        /// Gets or sets the Regex to match.
        /// </summary>
        /// <value>
        /// The regex.
        /// </value>
        public Regex Regex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPXIdentifierAttribute"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="elementType">Type of the IPX element.</param>
        public IPXIdentifierAttribute(string pattern, IPXElementType elementType = IPXElementType.Undefined)
        {
            this.Regex = new Regex(pattern);
            this.ElementType = elementType;
        }
    }
}

/*
 *	 SNMP Package for Constellation
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

namespace Snmp
{
    using SnmpSharpNet;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Declare the class as a SNMP object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SnmpObjectAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that a value is required
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies the SNMP Object identifier (OID)
    /// </summary>
    /// <seealso cref="SystemDescription.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class OIDAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the object identifier (OID).
        /// </summary>
        /// <value>
        /// The object identifier.
        /// </value>
        public Oid ObjectId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OIDAttribute"/> class.
        /// </summary>
        /// <param name="oid">The OID.</param>
        public OIDAttribute(string oid)
        {
            this.ObjectId = new Oid(oid);
        }
    }

    /// <summary>
    /// Represents SNMP sequence of entry.
    /// </summary>
    /// <typeparam name="T">The type of the entry</typeparam>
    public class Sequence<T> : Dictionary<string, T> { }
}
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
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Maps an enum value to an Orange's identifier
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class OrangeServiceIdAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrangeServiceIdAttribute"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public OrangeServiceIdAttribute(int id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Provides extension method to get the Orange's ServiceId value
    /// </summary>
    public static class OrangeServiceIdExtension
    {
        /// <summary>
        /// Gets the Orange's Service identifier.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>Orange's Service Id</returns>
        public static int GetOrangeServiceId<TEnum>(this TEnum value)
        {
            return typeof(TEnum).GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttributes<OrangeServiceIdAttribute>()?.FirstOrDefault()?.Id ?? 0;
        }

        /// <summary>
        /// Gets the Enum value from identifier.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="identifier">The identifier.</param>
        /// <returns>The valuye</returns>
        public static TEnum GetOrangeServiceValue<TEnum>(this int identifier)
        {
            return (TEnum)typeof(TEnum).GetFields().SingleOrDefault(a => a.GetCustomAttributes<OrangeServiceIdAttribute>()?.FirstOrDefault()?.Id == identifier).GetValue(null);
        }
    }
}
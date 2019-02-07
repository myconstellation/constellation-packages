/*
 *	 Paradox .NET library
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Sebastien Warin under one or more contributor
 *	 license agreements. Sebastien Warin licenses this file to you under
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

namespace Paradox
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Internal utility methods
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Gets the formated identifier from number.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        internal static string GetStringId(int id)
        {
            return id.ToString("000");
        }

        /// <summary>
        /// Gets the formated identifier from enum.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        internal static string GetStringId(Enum id)
        {
            return Convert.ToInt32(id).ToString("000");
        }

        /// <summary>
        /// Gets the enum value from string identifier.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">The string value.</param>
        /// <returns></returns>
        internal static T GetEnumValueFromStringId<T>(string value) 
        {
            return (T)(object)Convert.ToInt32(value);
        }

        /// <summary>
        /// Gets the Enum's value description from his attribute.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns></returns>
        internal static string GetDescription(Enum value)
        {
            var attrib = value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
            if (attrib != null)
            {
                return attrib.Description;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the enum value from his description.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static T GetEnumValueFromDescription<T>(string value) 
        {
            return (T)(object)Enum.GetValues(typeof(T)).OfType<Enum>().FirstOrDefault(v => GetDescription(v) == value);
        }
    }
}

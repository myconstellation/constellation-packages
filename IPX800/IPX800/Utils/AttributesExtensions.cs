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
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Extensions to get enum attribute
    /// </summary>
    public static class AttributesExtensions
    {
        /// <summary>
        /// Gets the enum member value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetEnumMemberValue<T>(this T source)
        {
            return source.GetEnumAttribute<EnumMemberAttribute>()?.Value;
        }

        /// <summary>
        /// Gets the enum attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TAttribute GetEnumAttribute<TAttribute>(this object source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            TAttribute[] attributes = fi.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0];
            }
            else
            {
                return default(TAttribute);
            }
        }
    }
}

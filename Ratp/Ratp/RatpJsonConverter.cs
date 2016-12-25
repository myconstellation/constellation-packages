/*
 *	 RATP Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Hydro
 *	 Copyright (C) 2016 - Pierre Grimaud <https://github.com/pgrimaud>
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

namespace Ratp
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Maps a JSON property to a .NET member or constructor parameter.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class RatpPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the names of the property.
        /// </summary>
        /// <value>
        /// The names of the property.
        /// </value>
        public string[] PropertyNames { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatpPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyNames">Names of the property.</param>
        public RatpPropertyAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }
    }

    /// <summary>
    /// Converts JSON property names to a .NET member.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class PropertyNamesMatchingConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass && objectType.FullName.StartsWith(this.GetType().Namespace);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            object instance = objectType.GetConstructor(Type.EmptyTypes).Invoke(null);
            foreach (PropertyInfo prop in objectType.GetProperties())
            {
                var attribute = prop.GetCustomAttribute<RatpPropertyAttribute>(true);
                if (attribute != null)
                {
                    foreach (JProperty jp in jo.Properties())
                    {
                        if (attribute.PropertyNames.Any(propName => string.Equals(jp.Name, propName, StringComparison.OrdinalIgnoreCase)))
                        {
                            prop.SetValue(instance, jp.Value.ToObject(prop.PropertyType, serializer));
                        }
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
/*
 *	 Vorwerk connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2020 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Vorwerk.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Used by Newtonsoft.Json.JsonSerializer to resolves a Newtonsoft.Json.Serialization.JsonContract for a given System.Type.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.DefaultContractResolver" />
    public class VorwerkContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Gets the JsonSerializerSettings that use the VorwerkContractResolver.
        /// </summary>
        /// <value>
        /// The JsonSerializerSettings.
        /// </value>
        public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings()
        {
            ContractResolver = new VorwerkContractResolver(),
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>
        /// Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
            foreach (JsonProperty prop in list)
            {
                var attribute = prop.AttributeProvider.GetAttributes(typeof(VorwerkPropertyAttribute), true).FirstOrDefault() as VorwerkPropertyAttribute;
                if (attribute != null)
                {
                    prop.PropertyName = attribute.PropertyName;
                }
            }
            return list;
        }
    }

    /// <summary>
    /// Maps a JSON property to a .NET member or constructor parameter.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class VorwerkPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VorwerkPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public VorwerkPropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }

    /// <summary>
    /// JSON string converter
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class ParseStringConverter : JsonConverter
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static readonly ParseStringConverter Singleton = new ParseStringConverter();

        /// <summary>
        /// Determines whether this instance can convert the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified t; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type t) => true;

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="t">The t.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Cannot unmarshal type long</exception>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return Convert.ChangeType(value, t);
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="untypedValue">The untyped value.</param>
        /// <param name="serializer">The serializer.</param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            serializer.Serialize(writer, untypedValue);
        }
    }

    /// <summary>
    /// JSON string converter
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class CustomEnumJsonConverter<TEnum> : JsonConverter where TEnum : struct, IConvertible, IComparable, IFormattable
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
            return objectType == typeof(TEnum);
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
            if (objectType == typeof(TEnum))
            {
                foreach (var item in (TEnum[])Enum.GetValues(typeof(TEnum)))
                {
                    var attr = item.GetType().GetTypeInfo().GetRuntimeField(item.ToString())
                        .GetCustomAttribute<VorwerkPropertyAttribute>();
                    if (attr != null && attr.PropertyName == reader.Value.ToString())
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // write out the JsonValue property's value
            serializer.Serialize(writer, value.ToString());
        }
    }
}

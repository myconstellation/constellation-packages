/*
 *	 Modbus Package for Constellation
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

namespace Modbus
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Represent Modbus device to request
    /// </summary>
    public class ModbusDevice
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        /// <value>
        /// The type of the device.
        /// </value>
        public string DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the slave identifier.
        /// </summary>
        /// <value>
        /// The slave identifier.
        /// </value>
        public byte SlaveID { get; set; }

        /// <summary>
        /// Gets or sets the request interval in second (default: 10sec).
        /// </summary>
        /// <value>
        /// The request interval in second.
        /// </value>
        [DefaultValue(10), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int RequestInterval { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Name} #{SlaveID}";
        }
    }

    /// <summary>
    /// Represent a Modbus device's definition
    /// </summary>
    public class ModbusDeviceDefinition
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the properties of the device.
        /// </summary>
        /// <value>
        /// The properties of the device.
        /// </value>
        public List<Property> Properties { get; set; }

        /// <summary>
        /// Represent a Modbus device property.
        /// </summary>
        public class Property
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>
            /// The description.
            /// </value>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the type of the register.
            /// </summary>
            /// <value>
            /// The type of the register.
            /// </value>
            [JsonConverter(typeof(StringEnumConverter))]
            public RegisterType RegisterType { get; set; }

            /// <summary>
            /// Gets or sets Data Address of the first register to read.
            /// </summary>
            /// <value>
            /// The Data Address of the first register to read.
            /// </value>
            public string Address { get; set; }

            /// <summary>
            /// Gets or sets the total number of registers requested.
            /// </summary>
            /// <value>
            /// The total number of registers requested.
            /// </value>
            [DefaultValue(1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public int Length { get; set; }

            /// <summary>
            /// Gets or sets the ratio to apply to the raw value.
            /// </summary>
            /// <value>
            /// The ratio to apply to the raw value.
            /// </value>
            [DefaultValue(1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public decimal Ratio { get; set; }

            /// <summary>
            /// Gets or sets the property's type.
            /// </summary>
            /// <value>
            /// The  property's type.
            /// </value>
            [JsonConverter(typeof(StringEnumConverter))]
            public PropertyType Type { get; set; }

            /// <summary>
            /// Gets the .NET CLR type of the property.
            /// </summary>
            /// <returns>The .NET CLR type of the property.</returns>
            public Type GetCLRType()
            {
                switch (this.Type)
                {
                    case PropertyType.Int:
                        return typeof(int);
                    case PropertyType.Float:
                        return typeof(float);
                    case PropertyType.Boolean:
                        return typeof(bool);
                    default:
                        return typeof(object);
                }
            }
        }

        /// <summary>
        /// Modbus Register Type (Holding or Input)
        /// </summary>
        public enum RegisterType
        {
            /// <summary>
            /// Holding register (FC3)
            /// </summary>
            Holding,
            /// <summary>
            /// Input register (FC4)
            /// </summary>
            Input
        }

        /// <summary>
        /// Property's types supported
        /// </summary>
        public enum PropertyType
        {
            /// <summary>
            /// Interger value
            /// </summary>
            Int,
            /// <summary>
            /// Decimal value
            /// </summary>
            Float,
            /// <summary>
            /// Boolean value
            /// </summary>
            Boolean
        }
    }
}

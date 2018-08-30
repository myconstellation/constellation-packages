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

namespace IPX800.Elements
{
    using IPX800.Enumerations;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represent an analog value (Analog Input, Virtual Analog or EnOcean Analog).
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    [IPXIdentifier(IPXIdentifierFormats.Analog, IPXElementType.Analog)]
    [IPXIdentifier(IPXIdentifierFormats.VirtualAnalog, IPXElementType.VirtualAnalog)]
    [IPXIdentifier(IPXIdentifierFormats.EnOceanAnalog, IPXElementType.EnOceanAnalog)]
    public class Analog : IPXBaseElement
    {
        /// <summary>
        /// The analog maximum value (3.3v)
        /// </summary>
        public const decimal MaxValue = 3.3M;

        /// <summary>
        /// Gets the analog value from the ADC (16 bits).
        /// </summary>
        /// <value>
        /// The analog value from the ADC (16 bits).
        /// </value>
        public ushort NumericValue { get; private set; }
        
        /// <summary>
        /// Gets the analog value.
        /// </summary>
        /// <value>
        /// The analog value.
        /// </value>
        public decimal AnalogValue { get; private set; }

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            if (prop.StartsWith("ENO")) // ENO ANALOG
            {
                var newValue = token.Value<decimal>(); // Analog value
                if (this.AnalogValue != newValue)
                {
                    this.AnalogValue = newValue;
                    var numericValue = (int)(this.AnalogValue / (MaxValue / (decimal)ushort.MaxValue));
                    this.NumericValue = (numericValue < 0 ||numericValue > ushort.MaxValue) ? ushort.MaxValue : (ushort)numericValue;
                    this.NotifyPropertyChanged(nameof(NumericValue));
                    this.NotifyPropertyChanged(nameof(AnalogValue));
                }
            }
            else // Analog or Virtual Analog
            {
                
                var newValue = token.Value<ushort>(); // Value (ushort) from the ADC 
                if (this.NumericValue != newValue)
                {
                    this.NumericValue = newValue;
                    this.AnalogValue = this.NumericValue * (MaxValue / ushort.MaxValue);
                    this.NotifyPropertyChanged(nameof(NumericValue));
                    this.NotifyPropertyChanged(nameof(AnalogValue));
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{base.ToString()} = {AnalogValue}";
        }
    }
}

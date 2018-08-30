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
    /// Represent a Thermostat
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    [IPXIdentifier(IPXIdentifierFormats.Thermostat, IPXElementType.Thermostat)]
    public class Thermostat : IPXBaseElement
    {
        /// <summary>
        /// Gets the target temperature.
        /// </summary>
        /// <value>
        /// The target temperature.
        /// </value>
        public double Temperature { get; private set; }

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            var currentValue = token.Value<double>();
            if (currentValue != this.Temperature)
            {
                this.Temperature = currentValue;
                this.NotifyPropertyChanged(nameof(Temperature));
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
            return $"{base.ToString()} is set to {Temperature}°C";
        }
    }
}

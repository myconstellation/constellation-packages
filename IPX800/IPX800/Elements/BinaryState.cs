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
    /// Represent a BinaryState element that can be On/true or Off/false (like an Output/Relay, Input, Virtual Output, Virtual Input, EnOcean switch/actuator/contact or a WatchDog)
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    [IPXIdentifier(IPXIdentifierFormats.Output, IPXElementType.Output)]
    [IPXIdentifier(IPXIdentifierFormats.Input, IPXElementType.Input)]
    [IPXIdentifier(IPXIdentifierFormats.VirtualInput, IPXElementType.VirtualInput)]
    [IPXIdentifier(IPXIdentifierFormats.VirtualOutput, IPXElementType.VirtualOutput)]
    [IPXIdentifier(IPXIdentifierFormats.EnOceanActuator, IPXElementType.EnOceanActuator)]
    [IPXIdentifier(IPXIdentifierFormats.EnOceanContact, IPXElementType.EnOceanContact)]
    [IPXIdentifier(IPXIdentifierFormats.EnOceanSwitch, IPXElementType.EnOceanSwitch)]
    [IPXIdentifier(IPXIdentifierFormats.WatchDog, IPXElementType.WatchDog)]
    public class BinaryState : IPXBaseElement
    {
        /// <summary>
        /// Gets the state of this IPX element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ON; otherwise, <c>false</c>.
        /// </value>
        public bool State { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this IPX element is Normally Closed (NC) or Normally Open (NO).
        /// </summary>
        /// <value>
        ///   <c>true</c> if normally closed; otherwise, <c>false</c>.
        /// </value>
        public bool NormallyClosed { get; set; }

        /// <summary>
        /// Configures the IPX element with the specifed configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public override void Configure(IPXElementConfiguration config)
        {
            if (config?.Options?["NC"] != null)
            {
                this.NormallyClosed = (bool)config.Options["NC"];
            }
        }

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            var newValue = token.Value<int>() == (this.NormallyClosed ? 0 : 1);
            if (this.State != newValue)
            {
                this.State = newValue;
                this.NotifyPropertyChanged(nameof(State));
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
            return $"{base.ToString()} is {(State ? "ON" : "OFF")}";
        }
    }
}

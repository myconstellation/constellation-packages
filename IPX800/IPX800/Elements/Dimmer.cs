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
    /// Represent an X-Dimmer
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    [IPXIdentifier(IPXIdentifierFormats.Dimmer, IPXElementType.Dimmer)]
    public class Dimmer : IPXBaseElement
    {
        /// <summary>
        /// Gets the state of this X-Dimmer.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ON; otherwise, <c>false</c>.
        /// </value>
        public bool State { get; private set; }

        /// <summary>
        /// Gets the X-Dimmer level.
        /// </summary>
        /// <value>
        /// The X-Dimmer level.
        /// </value>
        public int Level { get; private set; }

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            var newState = token["Etat"].Value<string>() == "ON";
            if (newState != this.State)
            {
                this.State = newState;
                this.NotifyPropertyChanged(nameof(State));
            }
            var newLevel = token["Valeur"].Value<int>();
            if (newLevel != this.Level)
            {
                this.Level = newLevel;
                this.NotifyPropertyChanged(nameof(Level));
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
            return $"{base.ToString()} is {(State ? "ON" : "OFF")} at {Level}%";
        }
    }
}

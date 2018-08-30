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
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// Represent a wire pilot
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    [IPXIdentifier(IPXIdentifierFormats.WirePilotGetId, IPXElementType.WirePilot)]
    public class WirePilot : IPXBaseElement
    {
        /// <summary>
        /// Gets the current mode.
        /// </summary>
        /// <value>
        /// The current mode.
        /// </value>
        public WirePilotMode Mode { get; private set; }

        /// <summary>
        /// Gets the Wire Pilot identifier use for the SET command.
        /// </summary>
        /// <value>
        /// The Wire Pilot identifier.
        /// </value>
        public string WirePilotId { get; private set; }

        /// <summary>
        /// Configures the IPX element with the specifed configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public override void Configure(IPXElementConfiguration config)
        {
            var match = this.GetType().GetCustomAttribute<IPXIdentifierAttribute>().Regex.Match(this.Id);
            this.WirePilotId = "FP" + (((Convert.ToInt32(match.Groups[1].Value) - 1) * 4) + Convert.ToInt32(match.Groups[2].Value)).ToString("00");
        }

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            var newMode = (token.Type == JTokenType.Integer) ? (WirePilotMode)token.Value<int>() : this.GetValueFromStringDescription<WirePilotMode>(token.Value<string>());
            if (newMode != this.Mode)
            {
                this.Mode = newMode;
                this.NotifyPropertyChanged(nameof(Mode));
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
            return $"{Type}: {Label} ({WirePilotId} or {Id}) is set to {Mode.ToString()}";
        }

        private T GetValueFromStringDescription<T>(string value)
        {
            foreach (object o in Enum.GetValues(typeof(T)))
            {
                T enumValue = (T)o;
                if (enumValue.GetEnumAttribute<DescriptionAttribute>().Description.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)o;
                }
            }
            return default(T);
        }
    }
}

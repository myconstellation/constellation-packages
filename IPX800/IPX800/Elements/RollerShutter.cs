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
    using System.Reflection;

    /// <summary>
    /// Represent a roller shutter
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    [IPXIdentifier(IPXIdentifierFormats.RollerShutterGetId, IPXElementType.RollerShutter)]
    public class RollerShutter : IPXBaseElement
    {
        /// <summary>
        /// Gets the Roller Shutter level.
        /// </summary>
        /// <value>
        /// The Roller Shutter level.
        /// </value>
        public int Level { get; private set; }
        
        /// <summary>
        /// Gets the roller shutter identifier use for the SET command.
        /// </summary>
        /// <value>
        /// The roller shutter identifier.
        /// </value>
        public string RollerShutterId { get; private set; }

        /// <summary>
        /// Configures the IPX element with the specifed configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public override void Configure(IPXElementConfiguration config)
        {
            var match = this.GetType().GetCustomAttribute<IPXIdentifierAttribute>().Regex.Match(this.Id);
            this.RollerShutterId = "VR" + (((Convert.ToInt32(match.Groups[1].Value) - 1) * 4) + Convert.ToInt32(match.Groups[2].Value)).ToString("00");
        }

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            var newLevel = token.Value<int>();
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
            return $"{Type}: {Label} ({RollerShutterId} or {Id}) is openned at {Level}%";
        }
    }
}

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
    using System.ComponentModel;

    /// <summary>
    ///  Defines an IPX800v4 element interface
    /// </summary>
    public interface IIPXElement : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the element's identifier.
        /// </summary>
        /// <value>
        /// The element's  identifier.
        /// </value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the element's type.
        /// </summary>
        /// <value>
        /// The element's type.
        /// </value>
        IPXElementType Type { get; set; }

        /// <summary>
        /// Configures the IPX element with the specifed configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void Configure(IPXElementConfiguration config);

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        void UpdateProperty(string prop, JToken token);
    }
}

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
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represent a SMS
    /// </summary>
    /// <seealso cref="IPX800.Elements.IPXBaseElement" />
    public class SMS : IPXBaseElement
    {
        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; private set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public string Date { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this SMS has message.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this SMS has message; otherwise, <c>false</c>.
        /// </value>
        public bool HasMessage => !string.IsNullOrEmpty(this.From) && !string.IsNullOrEmpty(this.Message);

        /// <summary>
        /// Updates the property of this element.
        /// </summary>
        /// <param name="prop">The property update.</param>
        /// <param name="token">The JSON value.</param>
        public override void UpdateProperty(string prop, JToken token)
        {
            var newValue = token["From"].Value<string>();
            if (newValue != this.From)
            {
                this.From = newValue;
                this.NotifyPropertyChanged(nameof(From));
            }
            newValue = token["Date"].Value<string>();
            if (newValue != this.Date)
            {
                this.Date = newValue;
                this.NotifyPropertyChanged(nameof(Date));
            }
            newValue = token["Message"].Value<string>();
            if (newValue != this.Message)
            {
                this.Message = newValue;
                this.NotifyPropertyChanged(nameof(Message));
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
            return HasMessage ? $"From {From} ({Date}) : {Message}°C" : "No SMS message";
        }
    }
}

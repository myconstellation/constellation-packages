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

namespace IPX800
{
    using IPX800.Communication;
    using IPX800.Elements;
    using IPX800.Enumerations;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents the IPX800 v4 manager
    /// </summary>
    public class IPX800v4
    {
        private Dictionary<Regex, Tuple<Type, IPXElementType>> ipxElementKnownTypes = null;

        /// <summary>
        /// Occurs when new IPX element is added.
        /// </summary>
        public event EventHandler<IPXElementEventArgs> ElementAdded;

        /// <summary>
        /// Gets the IPX800v4 interface.
        /// </summary>
        /// <value>
        /// The IPX800v4 interface.
        /// </value>
        public IIPX800v4Interface Interface { get; private set; }

        /// <summary>
        /// Gets the IPX elements attached to this IPX device.
        /// </summary>
        /// <value>
        /// The IPX elements attached to this IPX device.
        /// </value>
        public Dictionary<string, IIPXElement> Elements { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore unknown element.
        /// </summary>
        /// <value>
        ///   <c>true</c> to ignore unknown element; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreUnknownElement { get; set; }

        /// <summary>
        /// Gets or sets the elements configurations.
        /// </summary>
        /// <value>
        /// The elements configurations.
        /// </value>
        public List<IPXElementConfiguration> ElementsConfigurations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPX800v4"/> class.
        /// </summary>
        /// <param name="interface">The interface.</param>
        /// <param name="elementsConfigurations">The elements configurations.</param>
        /// <param name="ignoreUnknownElement">if set to <c>true</c> to ignore unknown element.</param>
        public IPX800v4(IIPX800v4Interface @interface, List<IPXElementConfiguration> elementsConfigurations = null, bool ignoreUnknownElement = false)
        {
            this.Interface = @interface;
            this.Elements = new Dictionary<string, IIPXElement>();
            this.ElementsConfigurations = elementsConfigurations;
            this.IgnoreUnknownElement = ignoreUnknownElement;
            // Load IPX object known types
            this.ipxElementKnownTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Select(type => new { Type = type, Attributes = type.GetCustomAttributes<IPXIdentifierAttribute>() })
                .SelectMany((type, idx) => type.Attributes, (type, attr) => new { type.Type, attr.Regex, attr.ElementType })
                .Where(t => t.Regex != null)
                .ToDictionary(k => k.Regex, v => new Tuple<Type, IPXElementType>(v.Type, v.ElementType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPX800v4"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="httpPort">The HTTP port.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="elementsConfigurations">The elements configurations.</param>
        /// <param name="ignoreUnknownElement">if set to <c>true</c> to ignore unknown element.</param>
        public IPX800v4(string host, int httpPort = 80, string apiKey = "apikey", List<IPXElementConfiguration> elementsConfigurations = null, bool ignoreUnknownElement = false)
            : this(new IPX800v4HttpInterface(host, httpPort, apiKey), elementsConfigurations, ignoreUnknownElement)
        {
        }

        /// <summary>
        /// Refreshes the specified element type.
        /// </summary>
        /// <param name="argument">The GET argument.</param>
        /// <param name="parameters">The optional parameters.</param>
        public void Refresh(GetArgument argument = GetArgument.All, string parameters = null)
        {
            JObject ipxJsonResponse = this.Interface.Get(argument, parameters);
            if (argument == GetArgument.SMS) // The SMS is a special case !
            {
                if (!this.Elements.ContainsKey("SMS"))
                {
                    this.Elements["SMS"] = new SMS { Id = "SMS", Label = "Last SMS" };
                }
                this.Elements["SMS"].UpdateProperty("SMS", ipxJsonResponse);
            }
            else
            {
                this.ProcessIPXResponse(ipxJsonResponse);
            }
        }

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="to">The phone number.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.ArgumentNullException">
        /// to
        /// or
        /// message
        /// </exception>
        public void SendSMS(string to, string message)
        {
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            this.Interface.Set(SetArgument.SMS, Int32.MaxValue, $"{to}:{message}");
        }

        /// <summary>
        /// Dims X-Dimmer channels.
        /// </summary>
        /// <param name="percent">The percent value of channel.</param>
        /// <param name="id">The X-Dimmer identifier (eg. G1, G2, G24 or null for all channels).</param>
        /// <param name="time">The optional time in millisecond.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">percent - Value must be between 0 and 100%</exception>
        public void Dim(int percent, string id = null, int? time = null)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(percent), "Value must be between 0 and 100%");
            }
            this.SetValue(SetArgument.Dimmer, id, percent.ToString() + (time != null ? "&Time=" + time.ToString() : ""));
        }

        /// <summary>
        /// Pulses to the specified BSO.
        /// </summary>
        /// <param name="bsoId">The BSO identifier.</param>
        /// <param name="type">The pulse's type.</param>
        /// <param name="pulseCount">The pulse's count.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// bsoId - The BSO Identifier must be between 1 and 32
        /// or
        /// pulseCount - The number of pulse must be between 1 and 127
        /// </exception>
        public void Pulse(int bsoId, SetPulseType type, int pulseCount)
        {
            if (pulseCount < 1 || pulseCount > 127)
            {
                throw new ArgumentOutOfRangeException(nameof(pulseCount), "The number of pulse must be between 1 and 127");
            }
            this.Interface.Set(type == SetPulseType.Up ? SetArgument.PulseUp : SetArgument.PulseDown, bsoId, ((int)pulseCount).ToString());
        }

        /// <summary>
        /// Sets the wire pilot mode.
        /// </summary>
        /// <param name="id">The FP identifier (FP00, FP01, FP16).</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="System.ArgumentNullException">id - The identifier is required</exception>
        public void SetWirePilot(string id, WirePilotMode mode)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "The identifier is required");
            }
            this.SetValue(SetArgument.WirePilot, id, ((int)mode).ToString());
        }

        /// <summary>
        /// Sets the roller shutter.
        /// </summary>
        /// <param name="percent">The percent value of the roller shutter.</param>
        /// <param name="id">The IPX roller shutter identifier (eg. VR01, VR32 or null for all roller shutters).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">percent - Value must be between 0 and 100%</exception>
        public void SetRollerShutter(int percent, string id = null)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(percent), "Value must be between 0 and 100%");
            }
            this.SetValue(SetArgument.RollerShutter, id, percent.ToString());
        }

        /// <summary>
        /// Sets the PWM channel.
        /// </summary>
        /// <param name="id">The X-PWM identifier (PWM1, PWM24).</param>
        /// <param name="percent">The percent value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">percent - Value must be between 0 and 100%</exception>
        public void SetPWMChannel(string id, int percent)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(percent), "Value must be between 0 and 100%");
            }
            this.SetValue(SetArgument.PWM, id, percent.ToString());
        }

        /// <summary>
        /// Sets the virtual analog.
        /// </summary>
        /// <param name="id">The IPX virtual analog identifier (eg. VA01, VA32).</param>
        /// <param name="value">The value (from 0 to 65535).</param>
        /// <exception cref="System.ArgumentNullException">id - The identifier is required</exception>
        public void SetVirtualAnalog(string id, ushort value)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "The identifier is required");
            }
            this.SetValue(SetArgument.VirtualAnalog, id, value.ToString());
        }

        /// <summary>
        /// Sets the counter.
        /// </summary>
        /// <param name="value">The value of the counter (from 0 to 255).</param>
        /// <param name="id">The IPX counter identifier (eg. C01, C16 or null for all counters).</param>
        /// <param name="type">The type of set (set value, increment or decrement value).</param>
        public void SetCounter(byte value, string id = null, SetCounterType type = SetCounterType.Set)
        {
            string strValue = ((type == SetCounterType.Increment) ? "+" : (type == SetCounterType.Decrement) ? "-" : "") + value.ToString();
            this.SetValue(SetArgument.Counter, id, strValue);
        }

        /// <summary>
        /// Sets the state for an Output (R), Virtual Output (VO), Virtual Input (VI) or EnOcean actuator (EnoPC).
        /// </summary>
        /// <param name="id">The IPX element identifier (eg. R01, VO012, EnoPC4, etc.).</param>
        /// <param name="state">The state (true = 1/Set, false = 0/Clear, null = Toggle).</param>
        /// <exception cref="System.ArgumentException">id - Invalid identifier</exception>
        /// <exception cref="System.NotSupportedException">This identifier doesn't support this command</exception>
        public void SetState(string id, bool? state = null)
        {
            var args = this.ParseIdentifier(id);
            if (args.Type == null)
            {
                throw new ArgumentException(nameof(id), "Invalid identifier");
            }
            else if (args.Type == SetArgument.Output || args.Type == SetArgument.VirtualInput || args.Type == SetArgument.VirtualOutput || args.Type == SetArgument.EnOcean)
            {
                if (state != null)
                {
                    if (state.Value)
                    {
                        this.Interface.Set(args.Type.Value, args.DeviceId);
                    }
                    else
                    {
                        this.Interface.Clear(args.Type.Value, args.DeviceId);
                    }
                }
                else
                {
                    this.Interface.Toggle(args.Type.Value, args.DeviceId);
                }
            }
            else
            {
                throw new NotSupportedException("This identifier doesn't support this command");
            }
        }

        private void SetValue(SetArgument type, string id, string value)
        {
            var args = string.IsNullOrEmpty(id) ? (type, Int32.MaxValue) : this.ParseIdentifier(id);
            if (args.Type == null)
            {
                throw new ArgumentException(nameof(id), "Invalid identifier");
            }
            else if (args.Type == type)
            {
                this.Interface.Set(type, args.DeviceId, value);
            }
            else
            {
                throw new NotSupportedException("This identifier doesn't support this command");
            }
        }

        private (SetArgument? Type, int DeviceId) ParseIdentifier(string id)
        {
            var args = Enum.GetValues(typeof(SetArgument))
               .Cast<object>()
               .Select(arg => new { Type = (SetArgument)arg, Match = arg.GetEnumAttribute<IPXIdentifierAttribute>()?.Regex.Match(id.ToString()) })
               .Where(o => o.Match?.Success ?? false)
               .FirstOrDefault();
            return (args?.Type, Convert.ToInt32(args?.Match.Groups[1].Value));
        }

        private void ProcessIPXResponse(JObject response)
        {
            foreach (var property in response.Properties())
            {
                var elementType = this.ipxElementKnownTypes
                    .Select(t => new { Match = t.Key.Match(property.Name), Type = t.Value })
                    .Where(t => t.Match.Success)
                    .FirstOrDefault();

                if (elementType != null)
                {
                    string elementId = string.IsNullOrEmpty(elementType.Match.Groups["id"].Value) ? elementType.Match.Groups[0].Value : elementType.Match.Groups["id"].Value;
                    IIPXElement currentElement = null;
                    // Getting the existing element
                    if (this.Elements.ContainsKey(elementId))
                    {
                        currentElement = this.Elements[elementId];
                    }
                    // Is a new element ?
                    if (currentElement == null)
                    {
                        // Get the element's configuration
                        IPXElementConfiguration elementConfiguration = this.ElementsConfigurations?.FirstOrDefault(e => e.Id == elementId);
                        // Ignore unknown element ?
                        if (this.ElementsConfigurations != null && elementConfiguration == null && this.IgnoreUnknownElement)
                        {
                            continue; // ignore it !
                        }
                        else
                        {
                            // Create new instance for the IPX element
                            currentElement = Activator.CreateInstance(elementType.Type.Item1) as IIPXElement;
                            // Set ID and configure
                            currentElement.Id = elementId;
                            currentElement.Type = elementType.Type.Item2;
                            if (currentElement is IPXBaseElement baseElement)
                            {
                                baseElement.Label = elementConfiguration?.Label ?? currentElement.Id;
                                baseElement.Room = elementConfiguration?.Room;
                            }
                            currentElement.Configure(elementConfiguration);
                            // Update the property
                            currentElement.UpdateProperty(property.Name, property.Value);
                            // Add the element to the current IPX and raise event
                            this.Elements.Add(elementId, currentElement);
                            this.ElementAdded?.Invoke(this, new IPXElementEventArgs(currentElement));
                        }
                    }
                    else
                    {
                        // Just update the property
                        currentElement.UpdateProperty(property.Name, property.Value);
                    }
                }
            }
        }
        
        /// <summary>
        /// Provides data for the ElementAdded event. 
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class IPXElementEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the element added.
            /// </summary>
            /// <value>
            /// The element added.
            /// </value>
            public IIPXElement Element { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="IPXElementEventArgs"/> class.
            /// </summary>
            /// <param name="element">The element.</param>
            public IPXElementEventArgs(IIPXElement element)
            {
                this.Element = element;
            }
        }
    }
}

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

namespace IPX800.Communication
{
    using IPX800.Enumerations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Diagnostics;
    using System.Net;

    /// <summary>
    /// Represent an IPX800v4 interface that use the HTTP API
    /// </summary>
    /// <seealso cref="IPX800.Communication.IIPX800v4Interface" />
    public class IPX800v4HttpInterface : IIPX800v4Interface
    {
        private string BaseAPIUri => $"http://{Host}:{HttpPort}/api/xdevices.json?key={ApiKey}";

        /// <summary>
        /// Gets or sets the IPX host.
        /// </summary>
        /// <value>
        /// The IPX host.
        /// </value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the HTTP port.
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        public int HttpPort { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public string ApiKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPX800v4HttpInterface"/> class.
        /// </summary>
        /// <param name="host">The IPX host.</param>
        /// <param name="httpPort">The HTTP port.</param>
        /// <param name="apiKey">The API key.</param>
        public IPX800v4HttpInterface(string host, int httpPort = 80, string apiKey = "apikey")
        {
            this.Host = host;
            this.HttpPort = httpPort;
            this.ApiKey = apiKey;
        }

        /// <summary>
        /// Gets the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="parameters">The optional parameters.</param>
        /// <returns></returns>
        public JObject Get(GetArgument argument = GetArgument.All, string parameters = null)
        {
            if ((argument == GetArgument.PWM || argument == GetArgument.DMX) && string.IsNullOrEmpty(parameters))
            {
                parameters = "|1-" + (argument == GetArgument.PWM ? "24" : "512");
            }
            return this.DoRequest($"Get={argument.GetEnumMemberValue()}{parameters}");
        }

        /// <summary>
        /// Sets the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="value">The optional value.</param>
        /// <returns></returns>
        public JObject Set(SetArgument argument, int elementId, string value = null)
        {
            var argumentValue = argument.GetEnumMemberValue();
            this.CheckDeviceIdRange(elementId, argumentValue);
            if (string.IsNullOrEmpty(value))
            {
                return this.DoRequest($"Set{argumentValue}={this.FormatDeviceId(elementId, argumentValue)}");
            }
            else
            {
                return this.DoRequest($"Set{argumentValue}{this.FormatDeviceId(elementId, argumentValue)}={value}");
            }
        }

        /// <summary>
        /// Clears the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">argument - Unsupported argument for this action</exception>
        public JObject Clear(SetArgument argument, int elementId)
        {
            switch (argument)
            {
                case SetArgument.Output:
                case SetArgument.VirtualInput:
                case SetArgument.VirtualOutput:
                case SetArgument.EnOcean:
                    var argumentValue = argument.GetEnumMemberValue();
                    this.CheckDeviceIdRange(elementId, argumentValue);
                    return this.DoRequest($"Clear{argumentValue}={this.FormatDeviceId(elementId, argumentValue)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(argument), "Unsupported argument for this action");
            }
        }

        /// <summary>
        /// Toggles the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">argument - Unsupported argument for this action</exception>
        public JObject Toggle(SetArgument argument, int elementId)
        {
            switch (argument)
            {
                case SetArgument.Output:
                case SetArgument.VirtualInput:
                case SetArgument.VirtualOutput:
                case SetArgument.EnOcean:
                    var argumentValue = argument.GetEnumMemberValue();
                    this.CheckDeviceIdRange(elementId, argumentValue);
                    return this.DoRequest($"Toggle{argumentValue}={this.FormatDeviceId(elementId, argumentValue)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(argument), "Unsupported argument for this action");
            }
        }

        private JObject DoRequest(string path)
        {
            using (var client = new WebClient())
            {
                string strResponse = client.DownloadString($"{BaseAPIUri}&{path}");
                Debug.WriteLine($"{BaseAPIUri}{path} : {strResponse}");
                return JsonConvert.DeserializeObject(strResponse) as JObject;
            }
        }

        private string FormatDeviceId(int deviceId, string type)
        {
            string parameter = null;
            if (deviceId != Int32.MaxValue) // MaxValue = wildcard
            {
                switch (type)
                {
                    case "R":
                    case "VA":
                    case "C":
                    case "VR":
                    case "PulseDOWN":
                    case "PulseUP":
                    case "FP":
                    case "G":
                        parameter = deviceId.ToString("00");
                        break;
                    case "VI":
                    case "VO":
                        parameter = deviceId.ToString("000");
                        break;
                    case "EnoPC":
                    case "PWM":
                        parameter = deviceId.ToString();
                        break;
                }
            }
            return parameter;
        }

        private void CheckDeviceIdRange(int deviceId, string type)
        {
            if (deviceId != Int32.MaxValue) // MaxValue = wildcard
            {
                switch (type)
                {
                    case "R":
                        if (deviceId < 1 || deviceId > 56)
                        {
                            throw new ArgumentOutOfRangeException("Relay must be between 1 and 56");
                        }
                        break;
                    case "VO":
                    case "VI":
                        if (deviceId < 1 || deviceId > 128)
                        {
                            throw new ArgumentOutOfRangeException("Virtual input and output must be between 1 and 128");
                        }
                        break;
                    case "EnoPC":
                        if (deviceId < 1 || deviceId > 24)
                        {
                            throw new ArgumentOutOfRangeException("EnOcean PC must be between 1 and 24");
                        }
                        break;
                    case "VA":
                        if (deviceId < 1 || deviceId > 32)
                        {
                            throw new ArgumentOutOfRangeException("Virtual analog must be between 1 and 32");
                        }
                        break;
                    case "C":
                        if (deviceId < 1 || deviceId > 16)
                        {
                            throw new ArgumentOutOfRangeException("Counter must be between 1 and 16");
                        }
                        break;
                    case "VR":
                        if (deviceId < 1 || deviceId > 32)
                        {
                            throw new ArgumentOutOfRangeException("Roller Shutter must be between 1 and 32");
                        }
                        break;
                    case "PulseUP":
                    case "PulseDOWN":
                        if (deviceId < 1 || deviceId > 32)
                        {
                            throw new ArgumentOutOfRangeException("BSO must be between 1 and 32");
                        }
                        break;
                    case "FP":
                        if (deviceId < 0 || deviceId > 16)
                        {
                            throw new ArgumentOutOfRangeException("Wire Pilot must be between 0 and 16");
                        }
                        break;
                    case "G":
                        if (deviceId < 1 || deviceId > 24)
                        {
                            throw new ArgumentOutOfRangeException("X-Dimmer channel must be between 0 and 24");
                        }
                        break;
                    case "PWM":
                        if (deviceId < 1 || deviceId > 24)
                        {
                            throw new ArgumentOutOfRangeException("X-PWM channel must be between 0 and 16");
                        }
                        break;
                }
            }
        }
    }
}

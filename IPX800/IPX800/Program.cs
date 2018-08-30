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
    using Constellation.Package;
    using IPX800.Elements;
    using IPX800.Enumerations;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Timers;
    using System.ComponentModel;

    /// <summary>
    /// IPX800v4 Constellation package
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    [StateObjectKnownTypes(
        typeof(Analog),
        typeof(BinaryState),
        typeof(Counter),
        typeof(Dimmer),
        typeof(PWM),
        typeof(RollerShutter),
        typeof(SMS),
        typeof(Thermostat),
        typeof(THL),
        typeof(WirePilot))]
    public class Program : PackageBase
    {
        private Dictionary<GetArgument, IPXElementType> elementTypesToPoll = null;
        private IPX800v4 ipx = null;
        private Timer timer = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Load the element's types to poll
            this.LoadElementTypesToPoll();

            // Create the IPX800 v4 manager
            this.ipx = new IPX800v4(
                PackageHost.GetSettingValue("Host"),
                PackageHost.GetSettingValue<int>("HttpPort"),
                PackageHost.GetSettingValue("ApiKey"),
                PackageHost.GetSettingAsJsonObject<List<IPXElementConfiguration>>("ElementsConfigurations"),
                PackageHost.GetSettingValue<bool>("IgnoreUnknownElement"));

            // When a new element is added, push it to Constellation and watch property changes
            this.ipx.ElementAdded += (s, e) =>
            {
                PackageHost.WriteInfo($"Adding {e.Element}");
                e.Element.PropertyChanged += (s2, e2) =>
                {
                    PackageHost.WriteInfo($"Updating '{e2.PropertyName}' of '{(s2 as IIPXElement).Id}' => {s2}");
                    this.PushIPXElement(e.Element);
                };
                this.PushIPXElement(e.Element);
            };

            // Refresh all !
            PackageHost.WriteInfo("Requesting IPX elements ...");
            this.ipx.Refresh();

            // Start the polling loop if needed
            if (this.elementTypesToPoll?.Count > 0)
            {
                PackageHost.WriteInfo($"Polling {string.Join(", ", this.elementTypesToPoll.Select(a => a.Key.ToString()))} every {PackageHost.GetSettingValue<int>("PollInterval")} second(s)");
                // Poll now
                elementTypesToPoll.Keys.ToList().ForEach(type => this.ipx.Refresh(type));
                // And start the timer
                this.timer = new Timer(PackageHost.GetSettingValue<int>("PollInterval") * 1000) { Enabled = true, AutoReset = true };
                this.timer.Elapsed += (s, e) => elementTypesToPoll.Keys.ToList().ForEach(type => this.ipx.Refresh(type));
                this.timer.Start();
            }

#if DEBUG
            // Used for debug  only !
            PackageHost.SubscribeMessages("IPX");
#endif

            PackageHost.WriteInfo("IPX800 connector is started!");
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            this.timer?.Stop();
        }

        /// <summary>
        /// Refreshes the specified element type.
        /// </summary>
        /// <param name="argument">The refresh type.</param>
        /// <param name="parameters">The optional parameters (required for X-PWM or X-DMX).</param>
        [MessageCallback]
        public void Refresh(GetArgument argument = GetArgument.All, string parameters = null)
        {
            this.ipx.Refresh(argument, parameters);
        }

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="to">The phone number.</param>
        /// <param name="message">The message to send.</param>
        [MessageCallback]
        public void SendSMS(string to, string message)
        {
            PackageHost.WriteInfo("Sending SMS to {0}", to);
            this.ipx.SendSMS(to, message);
        }

        /// <summary>
        /// Dims X-Dimmer channels.
        /// </summary>
        /// <param name="percent">The percent value of channel.</param>
        /// <param name="id">The X-Dimmer identifier (eg. G1, G2, G24 or null for all channels).</param>
        /// <param name="time">The optional time in millisecond.</param>
        [MessageCallback]
        public void Dim(int percent, string id = null, int? time = null)
        {
            PackageHost.WriteInfo("Dimming {0} to {1}% (transistion time: {2}ms)", id ?? "all channels", percent, time ?? 0);
            this.ipx.Dim(percent, id, time);
        }

        /// <summary>
        /// Pulses to the specified BSO.
        /// </summary>
        /// <param name="bsoId">The BSO identifier.</param>
        /// <param name="type">The pulse's type.</param>
        /// <param name="pulseCount">The pulse's count.</param>
        [MessageCallback]
        public void Pulse(int bsoId, SetPulseType type, int pulseCount)
        {
            PackageHost.WriteInfo("Pulse {0} {1} for {2} impulsion(s)", type.ToString(), bsoId, pulseCount);
            this.ipx.Pulse(bsoId, type, pulseCount);
        }

        /// <summary>
        /// Sets the wire pilot mode.
        /// </summary>
        /// <param name="id">The FP identifier (FP00, FP01, FP16).</param>
        /// <param name="mode">The mode.</param>
        [MessageCallback]
        public void SetWirePilot(string id, WirePilotMode mode)
        {
            PackageHost.WriteInfo("Setting {0} to {1}", id, mode.ToString());
            this.ipx.SetWirePilot(id, mode);
        }

        /// <summary>
        /// Sets the roller shutter.
        /// </summary>
        /// <param name="percent">The percent value of the roller shutter.</param>
        /// <param name="id">The IPX roller shutter identifier (eg. VR01, VR32 or null for all roller shutters).</param>
        [MessageCallback]
        public void SetRollerShutter(int percent, string id = null)
        {
            PackageHost.WriteInfo("Setting {0} to {1}%", id ?? "all VR", percent);
            this.ipx.SetRollerShutter(percent, id);
        }

        /// <summary>
        /// Sets the PWM channel.
        /// </summary>
        /// <param name="id">The X-PWM identifier (PWM1, PWM24).</param>
        /// <param name="percent">The percent value.</param>
        public void SetPWMChannel(string id, int percent)
        {
            PackageHost.WriteInfo("Setting {0} to {1}%", id, percent);
            this.ipx.SetPWMChannel(id, percent);
        }

        /// <summary>
        /// Sets the virtual analog.
        /// </summary>
        /// <param name="id">The IPX virtual analog identifier (eg. VA01, VA32).</param>
        /// <param name="value">The numeric value (from 0 to 65535).</param>
        [MessageCallback]
        public void SetVirtualAnalog(string id, ushort value)
        {
            PackageHost.WriteInfo("Setting {0} to {1}", id, value);
            this.ipx.SetVirtualAnalog(id, value);
        }

        /// <summary>
        /// Sets the counter.
        /// </summary>
        /// <param name="value">The value of the counter (from 0 to 255).</param>
        /// <param name="id">The IPX counter identifier (eg. C01, C16 or null for all counters).</param>
        /// <param name="type">The type of set (set value, increment or decrement value).</param>
        [MessageCallback]
        public void SetCounter(byte value, string id = null, SetCounterType type = SetCounterType.Set)
        {
            PackageHost.WriteInfo("{0} counter {1} to {2}", type.ToString(), id, value);
            this.ipx.SetCounter(value, id, type);
        }

        /// <summary>
        /// Sets the state for an Output/Relay (R), Virtual Output (VO), Virtual Input (VI) or EnOcean actuator (EnoPC).
        /// </summary>
        /// <param name="id">The IPX element identifier (eg. R01, VO012, EnoPC4, etc.).</param>
        /// <param name="state">The state (true = 1/Set, false = 0/Clear, null = Toggle).</param>
        [MessageCallback]
        public void SwitchState(string id, bool? state = null)
        {
            if (state != null)
            {
                PackageHost.WriteInfo("Setting {0} to {1}", id, state.Value);
            }
            else
            {
                PackageHost.WriteInfo("Toggling {0}", id);
            }
            this.ipx.SetState(id, state);
        }

        /// <summary>
        /// Called by an IPX OnEvent push
        /// </summary>
        /// <param name="event">The event.</param>
        [MessageCallback(IsHidden = true)]
        private void OnEvent(OnEventRequest @event)
        {
            Debug.WriteLine($"OnEvent {@event.Type} : {@event.Values}");
            for (int i = 1; i < @event.Values.Length + 1; i++)
            {
                string ipxObjectId = @event.Type + i.ToString();
                if (this.ipx.Elements.ContainsKey(ipxObjectId))
                {
                    this.ipx.Elements[ipxObjectId].UpdateProperty("State", JToken.Parse(@event.Values[i - 1].ToString()));
                }
            }
        }

        private void PushIPXElement(IIPXElement ipxElement)
        {
            PackageHost.PushStateObject(
                (PackageHost.GetSettingValue<bool>("UseLabelAsStateObjectName") && ipxElement is IPXBaseElement baseElement) ? baseElement.Label : ipxElement.Id,
                ipxElement,
                lifetime: (this.elementTypesToPoll != null && (this.elementTypesToPoll.ContainsKey(GetArgument.All) || this.elementTypesToPoll.Values.Any(t => ipxElement.Type == t))) ? PackageHost.GetSettingValue<int>("PollInterval") * 2 : 0);
        }

        private void LoadElementTypesToPoll()
        {
            // Get GetArgument & IPXElementType enum's value & code
            var getArgumentCodes = Enum.GetValues(typeof(GetArgument)).Cast<GetArgument>().Select(arg => new { Code = arg.GetEnumMemberValue(), Value = arg });
            var ipxElementTypeCodes = Enum.GetValues(typeof(IPXElementType)).Cast<IPXElementType>().Select(arg => new { Code = arg.GetEnumAttribute<DescriptionAttribute>()?.Description, Value = arg });
            // Load the PollElementTypes from settings
            this.elementTypesToPoll = PackageHost.ContainsSetting("PollElementTypes") ? PackageHost.GetSettingValue("PollElementTypes")
                .Split(',')
                .Select(typeName =>
                        getArgumentCodes
                            .Where(a => a.Code.Equals(typeName?.Trim(), StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault())
                .Where(a => a != null)
                .Select(a => new { GetArgument = a.Value, ElementType = ipxElementTypeCodes.FirstOrDefault(t => t.Code == a.Code)?.Value ?? IPXElementType.Undefined })
                .ToDictionary(k => k.GetArgument, v => v.ElementType) : null;
        }
    }
}

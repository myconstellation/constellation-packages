/*
 *	 Hue Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Hue
{
    using Constellation.Package;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Q42.HueApi;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Hue Package
    /// </summary>
    [StateObjectKnownTypes(typeof(Light), typeof(BridgeConfig))]
    public class Program : PackageBase
    {
        private HueClient hueClient = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Connecting to " + PackageHost.GetSettingValue<string>("BridgeAddress"));
            hueClient = new HueClient(PackageHost.GetSettingValue<string>("BridgeAddress"));
            hueClient.Initialize(PackageHost.GetSettingValue<string>("BridgeUsername"));

            PackageHost.WriteInfo("Getting configuration");
            var bridge = hueClient.GetBridgeAsync().GetAwaiter().GetResult();
            PackageHost.PushStateObject<BridgeConfig>("BridgeConfig", bridge.Config);
            foreach (Light light in bridge.Lights)
            {
                PackageHost.PushStateObject<Light>(light.Name, light);
            }

            PackageHost.WriteInfo("Starting query process");
            Task.Factory.StartNew(() =>
            {
                int pause = 1000;
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        var lights = hueClient.GetLightsAsync().GetAwaiter().GetResult();
                        foreach (Light light in lights)
                        {
                            PackageHost.PushStateObject<Light>(light.Name, light);
                        }
                        pause = 1000;
                    }
                    catch (Exception ex)
                    {
                        if (pause < 60000)
                        {
                            pause *= 2;
                        }
                        PackageHost.WriteError("Error to query lights : {0}", ex.ToString());
                    }
                    Thread.Sleep(pause);
                }
            });

            PackageHost.WriteInfo("Connected to {0}", hueClient.ApiBase);
        }

        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        [MessageCallback]
        public void SetState(int lightId, bool state)
        {
            this.SendCommandTo(new LightCommand() { On = state }, lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        [MessageCallback]
        public void SetColor(int lightId, int r, int g, int b)
        {
            this.SendCommandTo(new Q42.HueApi.LightCommand() { On = true }.SetColor(r, g, b), lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Sets the light (HS) state.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="bri">The bri.</param>
        [MessageCallback]
        public void Set(int lightId, bool state, int hue, int saturation, int bri)
        {
            this.SendCommandTo(new LightCommand() { On = state, Brightness = (byte)bri, Hue = hue, Saturation = saturation }, lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Sets the light (CT) state.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="colorTemperature">The color temperature.</param>
        /// <param name="bri">The bri.</param>
        [MessageCallback]
        public void SetCT(int lightId, bool state, int colorTemperature, int bri)
        {
            this.SendCommandTo(new LightCommand() { On = state, Brightness = (byte)bri, ColorTemperature = colorTemperature }, lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Sets the color temperature.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="colorTemperature">The color temperature.</param>
        [MessageCallback]
        public void SetColorTemperature(int lightId, int colorTemperature)
        {
            this.SendCommandTo(new LightCommand() { On = true, ColorTemperature = colorTemperature }, lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Sets the brightness.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="brightness">The brightness.</param>
        [MessageCallback]
        public void SetBrightness(int lightId, int brightness)
        {
            this.SendCommandTo(new LightCommand() { On = true, Brightness = (byte)brightness }, lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Shows the alert.
        /// </summary>
        /// <param name="lightId">The light identifier.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="duration">The duration.</param>
        [MessageCallback]
        public void ShowAlert(int lightId, int r, int g, int b, int duration = 1000)
        {
            // Get current state
            var light = hueClient.GetLightAsync(lightId.ToString()).GetAwaiter().GetResult();
            // Send command to show alert
            this.SendCommandTo(new Q42.HueApi.LightCommand()
            {
                On = true,
                Alert = duration == 0 ? Alert.Once : Alert.Multiple,
            }.SetColor(r, g, b), new List<string>() { lightId.ToString() });
            // Then restore initial state
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(duration > 0 ? duration > 30000 ? 30000 : duration : 1000);
                this.SendCommandTo(new Q42.HueApi.LightCommand()
                {
                    On = light.State.On,
                    ColorCoordinates = light.State.ColorCoordinates,
                    ColorTemperature = light.State.ColorTemperature,
                    Brightness = light.State.Brightness
                }, new List<string>() { lightId.ToString() });
            });
        }

        /// <summary>
        /// Sets the command to all.
        /// </summary>
        /// <param name="command">The command.</param>
        [MessageCallback]
        public void SetCommandToAll(LightCommand command)
        {
            this.SendCommandTo(command, null);
        }

        /// <summary>
        /// Sends the command to a list of lights.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="lightList">The light list.</param>
        [MessageCallback]
        public void SendCommandTo(LightCommand command, IEnumerable<string> lightList)
        {
            this.SendCommandTo(command.ToQ42Command(), lightList);
        }

        private void SendCommandTo(Q42.HueApi.LightCommand command, IEnumerable<string> lightList)
        {
            this.hueClient.SendCommandAsync(command, lightList);
        }

        /// <summary>
        /// Compose a light command to send to a light
        /// </summary>
        public class LightCommand
        {
            /// <summary>
            /// Gets or sets the colors based on CIE 1931 Color coordinates.
            /// </summary>
            public double[] ColorCoordinates { get; set; }
            
            /// <summary>
            /// Gets or sets the brightness 0-255.
            /// </summary>
            public byte? Brightness { get; set; }

            /// <summary>
            /// Gets or sets the hue for Hue and Q42.HueApi.LightCommand.Saturation mode.
            /// </summary>
            public int? Hue { get; set; }

            /// <summary>
            /// Gets or sets the saturation for Q42.HueApi.LightCommand.Hue and Saturation mode.
            /// </summary>
            public int? Saturation { get; set; }

            /// <summary>
            /// Gets or sets the Color Temperature
            /// </summary>
            public int? ColorTemperature { get; set; }

            /// <summary>
            /// Gets or sets whether the light is on.
            /// </summary>
            public bool? On { get; set; }

            /// <summary>
            /// Gets or sets the current effect for the light.
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Effect? Effect { get; set; }

            /// <summary>
            /// Gets or sets the current alert for the light.
            /// </summary>
            /// <value>
            /// The alert.
            /// </value>
            [JsonConverter(typeof(StringEnumConverter))]
            public Alert? Alert { get; set; }

            /// <summary>
            /// Gets or sets the current alert for the light.
            /// </summary>
            public TimeSpan? TransitionTime { get; set; }

            /// <summary>
            /// Convert to Q42 LightCommand.
            /// </summary>
            /// <returns></returns>
            public Q42.HueApi.LightCommand ToQ42Command()
            {
                return new Q42.HueApi.LightCommand
                {
                    Alert = this.Alert,
                    Brightness = this.Brightness,
                    ColorCoordinates = this.ColorCoordinates,
                    ColorTemperature = this.ColorTemperature,
                    Effect = this.Effect,
                    Hue = this.Hue,
                    On = this.On,
                    Saturation = this.Saturation,
                    TransitionTime = this.TransitionTime
                };
            }
        }
    }
}

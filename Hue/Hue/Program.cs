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
            this.SendCommandTo(new LightCommand() { On = true }.SetColor(r, g, b), lightId == 0 ? null : new List<string>() { lightId.ToString() });
        }

        /// <summary>
        /// Sets the light state.
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
        public void ShowAlert(int lightId, int r, int g, int b, int duration)
        {
            // Get current state
            var light = hueClient.GetLightAsync(lightId.ToString()).GetAwaiter().GetResult();
            // Send command to show alert
            this.SendCommandTo(new LightCommand()
            {
                On = true,
                Alert = duration == 0 ? Alert.Once : Alert.Multiple,
            }.SetColor(r, g, b), new List<string>() { lightId.ToString() });
            // Then restore initial state
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(duration > 0 ? duration > 30000 ? 30000 : duration : 1000);
                this.SendCommandTo(new LightCommand()
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
            this.hueClient.SendCommandAsync(command, lightList);
        }
    }
}

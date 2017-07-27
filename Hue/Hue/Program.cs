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
    using Q42.HueApi.ColorConverters;
    using Q42.HueApi.ColorConverters.Original;
    using Q42.HueApi.Models;
    using Q42.HueApi.Models.Groups;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Hue Package
    /// </summary>
    [StateObjectKnownTypes(typeof(Light), typeof(BridgeConfig), typeof(Dictionary<string, List<Scene>>), typeof(Dictionary<string, Group>))]
    public class Program : PackageBase
    {
        private LocalHueClient hueClient = null;

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
            hueClient = new LocalHueClient(PackageHost.GetSettingValue<string>("BridgeAddress"));
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

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        var scenes = hueClient.GetScenesAsync().GetAwaiter().GetResult();
                        Dictionary<string, List<Scene>> dicSceneByLight = new Dictionary<string, List<Scene>>();
                        foreach (Scene scene in scenes)
                        {
                            foreach (var item in scene.Lights)
                            {
                                if (!dicSceneByLight.ContainsKey(item))
                                {
                                    dicSceneByLight.Add(item, new List<Scene>());
                                }
                                dicSceneByLight[item].Add(scene);
                            }
                        }
                        PackageHost.PushStateObject("scene", dicSceneByLight);
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error to query scene", ex.ToString());
                    }
                    Thread.Sleep(60000);
                }
            });

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Dictionary<string, Group> dicGroupById = new Dictionary<string, Group>();
                    foreach (var item in this.hueClient.GetGroupsAsync().GetAwaiter().GetResult())
                    {
                        dicGroupById.Add(item.Id, item);
                    }
                    PackageHost.PushStateObject("groups", dicGroupById);
                }
                catch (Exception ex)
                {
                    PackageHost.WriteError("Error to query scene", ex.ToString());
                }
                Thread.Sleep(60000);
            });       
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
            this.SendCommandTo(LightCommandExtensions.SetColor(new LightCommand() { On = true }, new RGBColor(r, g, b)), lightId == 0 ? null : new List<string>() { lightId.ToString() });
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
            this.SendCommandTo(LightCommandExtensions.SetColor(new LightCommand()
            {
                On = true,
                Alert = duration == 0 ? Alert.Once : Alert.Multiple,
            }, new RGBColor(r, g, b)), new List<string>() { lightId.ToString() });
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

        /// <summary>
        /// Set scene for groupId
        /// </summary>
        /// <param name="sceneId">Id of scene</param>
        /// <param name="groupId">Id of group</param>
        [MessageCallback]
        public void setScene(string sceneId, string groupId)
        {
            SceneCommand sceneCommand = new SceneCommand()
            {
                Scene = sceneId
            };
            this.hueClient.SendGroupCommandAsync(sceneCommand, groupId);
        }

        /// <summary>
        /// Add Light to Scene
        /// </summary>
        /// <param name="groupId">Id of groups</param>
        /// <param name="lightId">Id of light to add</param>
        [MessageCallback]
        public void addLightToGroup(string groupId, string lightId)
        {
            Group group = this.hueClient.GetGroupAsync(groupId).GetAwaiter().GetResult();
            List<String> listLight = new List<string>(group.Lights);
            listLight.Add(lightId);
            this.hueClient.UpdateGroupAsync(group.Id, listLight);
        }

        /// <summary>
        /// Remove Light to Scene
        /// </summary>
        /// <param name="groupId">Id of groups</param>
        /// <param name="lightId">Id of light to remove</param>
        [MessageCallback]
        public void removeLightToGroup(string groupId, string lightId)
        {
            Group group = this.hueClient.GetGroupAsync(groupId).GetAwaiter().GetResult();
            List<String> listLight = new List<string>(group.Lights);
            listLight.Remove(lightId);
            this.hueClient.UpdateGroupAsync(group.Id, listLight);
        }

        /// <summary>
        /// Create Group
        /// </summary>
        /// <param name="groupName">group Name</param>
        /// <param name="lightId">light Id</param>
        /// <param name="roomClass">Type of room</param>
        [MessageCallback]
        public string createGroup(string groupName, string lightId, RoomClass roomClass)
        {
            return this.hueClient.CreateGroupAsync(new List<string>()
            {
                lightId
            }, groupName, roomClass).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Delete group
        /// </summary>
        /// <param name="groupId">Id of groups</param>
        [MessageCallback]
        public void deleteGroup(string groupId)
        {
            this.hueClient.DeleteGroupAsync(groupId);
        }
    }
}

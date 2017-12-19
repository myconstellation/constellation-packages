/*
 *	 OrangeTV Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace OrangeTV
{
    using Constellation.Package;
    using System;

    /// <summary>
    /// OrangeTV Package
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private OrangeSetTopBox orangeBox = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Create the STB service
            this.orangeBox = new OrangeSetTopBox(PackageHost.GetSettingValue("Hostname"));
            // Attach the event notification
            this.orangeBox.EventNotificationReceived += (s, e) =>
            {
                if (PackageHost.GetSettingValue<bool>("Verbose"))
                {
                    PackageHost.WriteInfo($"Event from {this.orangeBox.CurrentState.MacAddress} : {e.Notification.EventType}");
                }
                // Update the current state
                PackageHost.PushStateObject("State", this.orangeBox.CurrentState);
            };
            // Get the current state
            var task = this.orangeBox.GetCurrentState();
            if (task.Wait(10000) && task.IsCompleted && !task.IsFaulted)
            {
                // Push the current state
                PackageHost.PushStateObject("State", task.Result);
                // Listening event
                this.orangeBox.StartListening();
                // Read!
                PackageHost.WriteInfo($"Connected to {task.Result.FriendlyName} ({task.Result.MacAddress})");
            }
            else
            {
                throw new Exception("Unable to connect to your Orange set-top box! Check your configuration & network");
            }
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            this.orangeBox.StopListening();
        }

        /// <summary>
        /// Switches to EPG identifier.
        /// </summary>
        /// <param name="epgId">The EPG identifier.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SwitchTo(string epgId)
        {
            PackageHost.WriteInfo($"Switching to EPG #{epgId}");
            return this.orangeBox.SwitchTo(epgId).Result;
        }

        /// <summary>
        /// Switches to channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SwitchToChannel(Channel channel)
        {
            PackageHost.WriteInfo($"Switching to channel #{channel.GetOrangeServiceId()} ({channel.ToString()})");
            return this.orangeBox.SwitchToChannel(channel).Result;
        }

        /// <summary>
        /// Sends the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="mode">The mode (SinglePress by default).</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SendKey(Key key, PressKeyMode mode = PressKeyMode.SinglePress)
        {
            PackageHost.WriteInfo($"Sending key #{key.GetOrangeServiceId()} ({key.ToString()}) as {mode.ToString()}");
            return this.orangeBox.SendKey(key, mode).Result;
        }
    }
}

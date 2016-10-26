/*
 *	 RelayBoard Package for Constellation
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

namespace RelayBoard
{
    using Constellation.Package;
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public class Program : PackageBase
    {
        private RelayBoard board = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        /// <exception cref="Exception">Unable to connected to relay board</exception>
        public override void OnStart()
        {
            this.board = new RelayBoard();
            if (this.board.Connect())
            {
                // Push the board to StateObject
                PackageHost.PushStateObject("RelayBoard_" + this.board.SerialNumber, this.board);
                // Update all relays's StateObjects to "Off"
                this.UpdateStateObjects(Relay.All, false);
                // Ready !
                PackageHost.WriteInfo("The relay board {0} is ready!", this.board.SerialNumber);
            }
            else
            {
                throw new Exception("Unable to connected to the SainSmart relay board");
            }
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            // Switch off all relays
            this.SetSwitch(Relay.All, false);
        }

        /// <summary>
        /// Activate/De-activate a specific relay
        /// </summary>
        /// <param name="relay">The relay.</param>
        /// <param name="state">If set to <c>true</c> switch on, otherwise, switch off.</param>
        [MessageCallback]
        public void SetSwitch(Relay relay, bool state)
        {
            // Switch the relay
            this.board.RelaySwitch(relay, state);
            // Update the StateObject
            this.UpdateStateObjects(relay, state);
            // Write log
            PackageHost.WriteInfo("Switching relay '{0}' to {1}", relay, state ? "On" : "Off");
        }

        private void UpdateStateObjects(Relay relay, bool state)
        {
            int i = 0;
            foreach (var item in Enum.GetValues(typeof(Relay)))
            {
                if (++i > PackageHost.GetSettingValue<int>("RelayCount") || item == (object)Relay.All)
                {
                    break;
                }
                else if (((Relay)item & relay) != 0)
                {
                    PackageHost.PushStateObject(item.ToString(), state,
                        metadatas: new System.Collections.Generic.Dictionary<string, object>()
                        {
                            ["Id"] = Convert.ToInt16(typeof(Relay).GetMember(item.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description),
                            ["Flag"] = ((byte)item).ToString()
                        });
                }
            }
        }
    }
}

/*
 *	 PoolCop connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018-2019 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace PoolCop
{
    using Constellation.Package;
    using PoolCop.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// PoolCop Constellation package
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    [StateObjectKnownTypes(typeof(PoolCopAPI), typeof(Pool), typeof(PoolCop))]
    public class Program : PackageBase
    {
        private System.Timers.Timer timer = null;
        private PoolCopInterface poolcop = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Create the PoolCop interface
            this.poolcop = new PoolCopInterface(PackageHost.GetSettingValue("PoolCopilotAPISecretKey"), PackageHost.GetSettingValue("PoolCopLocalIP"))
            {
                APILanguage = PackageHost.GetSettingValue("PoolCopilotAPILanguage")
            };

            // On status changed, Push StateObject
            this.poolcop.APIStatusChanged += (s, e) => PackageHost.PushStateObject("API", this.poolcop.Status.API, lifetime: (int)this.poolcop.Status.API.ExpirationDate.Subtract(DateTime.UtcNow).TotalSeconds + 30);
            this.poolcop.StatusChanged += (s, e) =>
            {
                int lifetime = (int)this.poolcop.Status.API.ExpirationDate.Subtract(DateTime.UtcNow).TotalSeconds + 30;
                var metadatas = new Dictionary<string, object>
                {
                    ["Name"] = this.poolcop.Status.Pool.Nickname,
                    ["LocalIP"] = this.poolcop.LocalAddress,
                };
                PackageHost.PushStateObject("Pool", this.poolcop.Status.Pool, metadatas: metadatas, lifetime: lifetime);
                PackageHost.PushStateObject("PoolCop", this.poolcop.Status.PoolCop, metadatas: metadatas, lifetime: lifetime);
            };

            // First query
            this.poolcop.Query().Wait();

            // Scheduler 
            this.timer = new System.Timers.Timer(PackageHost.GetSettingValue<int>("Interval") * 1000) { Enabled = true, AutoReset = true };
            this.timer.Elapsed += async (s, e) =>
            {
                try
                {
                    await this.poolcop.Query();
                }
                catch (Exception ex)
                {
                    PackageHost.WriteError($"Unable to request {this.poolcop.Status?.Pool?.Nickname ?? "PoolCop"} : {ex.GetBaseException()?.Message ?? ex.Message}");
                }
            };
            this.timer.Start();

            // Done
            PackageHost.WriteInfo($"Connected to {this.poolcop.Status.Pool.Poolcop}");
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            this.timer?.Stop();
        }
        
        /// <summary>
        /// Switches the auxiliary state
        /// </summary>
        /// <param name="auxId">The aux identifier</param>
        [MessageCallback]
        public CommandResult SwitchAuxiliary(int auxId)
        {
            PackageHost.WriteInfo($"Switching auxiliary #{auxId}");
            return this.poolcop.SwitchAuxiliary(auxId).Result;
        }

        /// <summary>
        /// Clears alarm
        /// </summary>
        [MessageCallback]
        public CommandResult ClearAlarm()
        {
            PackageHost.WriteInfo($"Clearing alarm");
            return this.poolcop.ClearAlarm().Result;
        }

        /// <summary>
        /// Switches the pump state
        /// </summary>
        [MessageCallback]
        public CommandResult SwitchPumpState()
        {
            PackageHost.WriteInfo("Switching pump state");
            return this.poolcop.SwitchPumpState().Result;
        }
    }
}
/*
 *	 Vorwerk connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2020 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Vorwerk
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Vorwerk.Models;

    /// <summary>
    /// PoolCop Constellation package
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    [StateObjectKnownTypes(typeof(Dashboard), typeof(Robot), typeof(FirmwareInfo))]
    public class Program : PackageBase
    {
        private System.Timers.Timer timer = null;
        private VorwerkInterface api = null;

        private Dashboard dashboard = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Intervals
            var robotPollingInterval = TimeSpan.FromSeconds(PackageHost.GetSettingValue<int>("RobotPollingInterval"));
            var dashboardPollingInterval = TimeSpan.FromSeconds(PackageHost.GetSettingValue<int>("DashboardPollingInterval"));

            // Create the Vorwerk interface
            this.api = new VorwerkInterface(
                PackageHost.GetSettingValue("Username"),
                PackageHost.GetSettingValue("Password"),
                (VorwerkInterface.VendorId)Enum.Parse(typeof(VorwerkInterface.VendorId), PackageHost.GetSettingValue("Vendor")));

            // Get and push the dashboard
            var updateDashboard = new Func<Task<Dashboard>>(async () =>
            {
                this.dashboard = await this.api.GetDashboard();
                PackageHost.PushStateObject("Dashboard", this.dashboard,
                    lifetime: (int)dashboardPollingInterval.TotalSeconds * 2);
                return this.dashboard;
            });

            // Get and push robot
            var updateRobot = new Action<Robot>(async (robot) =>
            {
                var state = await this.api.GetRobotState(robot);
                PackageHost.PushStateObject(robot.Name, state,
                    lifetime: (int)robotPollingInterval.TotalSeconds * 2,
                    metadatas: new Dictionary<string, object>()
                    {
                        ["Name"] = robot.Name,
                        ["Model"] = robot.Model,
                        ["MacAddress"] = robot.MacAddress,
                        ["Firmware"] = robot.Firmware
                    });
            });

            // First queries
            this.dashboard = updateDashboard().Result;
            this.dashboard.Robots.ForEach(robot => updateRobot(robot));

            // Scheduler
            var lastDashboardUpdated = DateTime.Now;
            this.timer = new System.Timers.Timer(robotPollingInterval.TotalMilliseconds) { Enabled = true, AutoReset = true };
            this.timer.Elapsed += async (s, e) =>
            {
                try
                {
                    if (DateTime.Now.Subtract(lastDashboardUpdated) > dashboardPollingInterval)
                    {
                        this.dashboard = await updateDashboard();
                        lastDashboardUpdated = DateTime.Now;
                    }
                    this.dashboard.Robots.ForEach(robot => updateRobot(robot));
                }
                catch (Exception ex)
                {
                    PackageHost.WriteError($"Error during polling : {ex.GetBaseException()?.Message ?? ex.Message}");
                }
            };
            this.timer.Start();

            // Done
            PackageHost.WriteInfo($"Connected behalf {dashboard.Email}");
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            this.timer?.Stop();
        }

        /// <summary>
        /// Starts the cleaning
        /// </summary>
        /// <param name="robotName">Name of the robot.</param>
        /// <param name="ecoMode">if set to <c>true</c> clean in eco mode.</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<RobotState> StartCleaning(string robotName, bool ecoMode = true)
        {
            PackageHost.WriteInfo($"Starting cleaning for {robotName}");
            return await this.ExecuteAction(robotName, (robot) => this.api.StartCleaning(robot, ecoMode));
        }

        /// <summary>
        /// Starts the spot cleaning.
        /// </summary>
        /// <param name="robotName">Name of the robot.</param>
        /// <param name="ecoMode">if set to <c>true</c> clean in eco mode.</param>
        /// <param name="height">The spot width in cm (min 100cm).</param>
        /// <param name="width">The spot height in cm (min 100cm).</param>
        /// <param name="repeat">if set to <c>true</c> clean spot two times.</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<RobotState> StartSpotCleaning(string robotName,
            bool ecoMode = true,
            int height = 200,
            int width = 200,
            bool repeat = false)
        {
            PackageHost.WriteInfo($"Starting spot cleaning for {robotName}");
            return await this.ExecuteAction(robotName, (robot) => this.api.StartSpotCleaning(robot, ecoMode, height, width, repeat));
        }

        /// <summary>
        /// Stops the cleaning.
        /// </summary>
        /// <param name="robotName">Name of the robot.</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<RobotState> StopCleaning(string robotName)
        {
            PackageHost.WriteInfo($"Stopping cleaning for {robotName}");
            return await this.ExecuteAction(robotName, (robot) => this.api.StopCleaning(robot));
        }

        /// <summary>
        /// Sends to base.
        /// </summary>
        /// <param name="robotName">Name of the robot.</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<RobotState> SendToBase(string robotName)
        {
            PackageHost.WriteInfo($"Sending {robotName} to base");
            return await this.ExecuteAction(robotName, (robot) => this.api.SendToBase(robot));
        }

        /// <summary>
        /// Resumes the cleaning.
        /// </summary>
        /// <param name="robotName">Name of the robot.</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<RobotState> ResumeCleaning(string robotName)
        {
            PackageHost.WriteInfo($"Resuming cleaning for {robotName}");
            return await this.ExecuteAction(robotName, (robot) => this.api.ResumeCleaning(robot));
        }

        /// <summary>
        /// Pauses the cleaning.
        /// </summary>
        /// <param name="robotName">Name of the robot.</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<RobotState> PauseCleaning(string robotName)
        {
            PackageHost.WriteInfo($"Pausing cleaning for {robotName}");
            return await this.ExecuteAction(robotName, (robot) => this.api.PauseCleaning(robot));
        }

        private async Task<RobotState> ExecuteAction(string robotName, Func<Robot, Task<RobotState>> action)
        {
            var robot = this.dashboard.Robots.FirstOrDefault(r => r.Name == robotName);
            if (robot != null)
            {
                return await action(robot);
            }
            else
            {
                PackageHost.WriteError($"Robot '{robotName}' not found");
                return null;
            }
        }
    }
}
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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Vorwerk.Models;

    /// <summary>
    /// Vorwerk communication interface
    /// </summary>
    public class VorwerkInterface
    {
        public enum VendorId { Vorwerk, Neato }

        /// <summary>
        /// The Vowerk API root URI
        /// </summary>
        private const string VORWERK_API_ROOT_URI = "https://vorwerk-beehive-production.herokuapp.com";

        /// <summary>
        /// The Neato API root URI
        /// </summary>
        private const string NEATO_API_ROOT_URI = "https://beehive.neatocloud.com";

        /// <summary>
        /// Occurs when robot state updated.
        /// </summary>
        public event EventHandler<RobotStateEventArgs> RobotStateUpdated;

        /// <summary>
        /// The access token
        /// </summary>
        private string accessToken = null;                

        /// <summary>
        /// Gets the username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        private string Password { get; set; }

        /// <summary>
        /// Gets the vendor.
        /// </summary>
        /// <value>
        /// The vendor.
        /// </value>
        public VendorId Vendor { get; private set; } = VendorId.Vorwerk;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="VorwerkInterface" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="vendor">The vendor (Vorwerk by default or Neato).</param>
        public VorwerkInterface(string username, string password, VendorId vendor = VendorId.Vorwerk)
        {
            this.Username = username;
            this.Password = password;
            this.Vendor = vendor;
        }

        /// <summary>
        /// Get the dashboard
        /// </summary>
        /// <returns></returns>
        public async Task<Dashboard> GetDashboard()
        {
            var token = await this.GetAccessToken();
            var strReponse = await HttpUtils.GetWebResponseAsync(new Uri($"{(this.Vendor == VendorId.Vorwerk ? VORWERK_API_ROOT_URI : NEATO_API_ROOT_URI)}/dashboard"), headers: new Dictionary<string, string>()
            {
                ["Authorization"] = $"Token token={this.accessToken}"
            });
            return Dashboard.FromJson(strReponse);
        }

        /// <summary>
        /// Gets the state of the robot.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> GetRobotState(Robot robot)
        {
            return await ExecuteCommand(robot, "getRobotState");
        }

        /// <summary>
        /// Starts the cleaning.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <param name="ecoMode">if set to <c>true</c> clean in eco mode.</param>
        /// <param name="navigationMode">The navigation mode (new neato models only).</param>
        /// <returns></returns>
        public async Task<RobotState> StartCleaning(Robot robot,
                    bool ecoMode = true,                    
                    Cleaning.CleaningNavigation? navigationMode = null)
        {
            return await this.StartCleaning(robot, new Dictionary<string, int?>()
            {
                ["category"] = (int)Cleaning.CleaningCategory.House,
                ["mode"] = ecoMode ? (int)Cleaning.CleaningMode.Eco : (int)Cleaning.CleaningMode.Normal,
                ["navigationMode"] = (int?)navigationMode
            });
        }

        /// <summary>
        /// Starts the spot cleaning.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <param name="ecoMode">if set to <c>true</c> clean in eco mode.</param>
        /// <param name="height">The spot width in cm (min 100cm).</param>
        /// <param name="width">The spot height in cm (min 100cm).</param>
        /// <param name="repeat">if set to <c>true</c> clean spot two times.</param>
        /// <param name="navigationMode">The navigation mode (new neato models only).</param>
        /// <returns></returns>
        public async Task<RobotState> StartSpotCleaning(Robot robot,
                    bool ecoMode = true,
                    int height = 200,
                    int width = 200,
                    bool repeat = false,
                    Cleaning.CleaningNavigation? navigationMode = null)
        {
            return await this.StartCleaning(robot, new Dictionary<string, int?>()
            {
                ["category"] = (int)Cleaning.CleaningCategory.Spot,
                ["mode"] = ecoMode ? (int)Cleaning.CleaningMode.Eco : (int)Cleaning.CleaningMode.Normal,
                ["navigationMode"] = (int?)navigationMode,
                ["modifier"] = repeat ? (int)Cleaning.CleaningModifier.Double : (int)Cleaning.CleaningModifier.Normal,
                ["spotWidth"] = width,
                ["spotHeight"] = height
            });
        }

        /// <summary>
        /// Starts the cleaning.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private async Task<RobotState> StartCleaning(Robot robot, Dictionary<string, int?> parameters)
        {
            return await ExecuteCommand(robot, "startCleaning",
                parameters.Where(i => i.Value != null).ToDictionary(i => i.Key, i => i.Value.ToString()));
        }

        /// <summary>
        /// Pauses the cleaning.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> PauseCleaning(Robot robot)
        {
            return await ExecuteCommand(robot, "pauseCleaning");
        }

        /// <summary>
        /// Resumes the cleaning.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> ResumeCleaning(Robot robot)
        {
            return await ExecuteCommand(robot, "resumeCleaning");
        }

        /// <summary>
        /// Stops the cleaning.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> StopCleaning(Robot robot)
        {
            return await ExecuteCommand(robot, "stopCleaning");
        }

        /// <summary>
        /// Sends to base.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> SendToBase(Robot robot)
        {
            return await ExecuteCommand(robot, "sendToBase");
        }

        /// <summary>
        /// Enables the schedule.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> EnableSchedule(Robot robot)
        {
            return await ExecuteCommand(robot, "enableSchedule");
        }

        /// <summary>
        /// Disables the schedule.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        public async Task<RobotState> DisableSchedule(Robot robot)
        {
            return await ExecuteCommand(robot, "disableSchedule");
        }

        /// <summary>
        /// Gets the schedule.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RobotState> GetSchedule(Robot robot)
        {
            throw new NotImplementedException();
            //return await ExecuteCommand(robot, "getSchedule");
        }

        /// <summary>
        /// Sets the schedule.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RobotState> SetSchedule(Robot robot)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="robot">The robot.</param>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The optional parameters.</param>
        /// <returns></returns>
        private async Task<RobotState> ExecuteCommand(Robot robot, string command, Dictionary<string, string> parameters = null)
        {
            var date = DateTime.UtcNow;
            var payload = new Dictionary<string, object>() 
            {
                ["reqId"] = "1",
                ["cmd"] = command
            };
            if (parameters != null)
            {
                payload.Add("params", parameters);
            }
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var hmac = ComputeHMAC(robot.SecretKey,  string.Join("\n", new string[] { robot.Serial.ToLower(), date.ToString("r"), jsonPayload })); 
            string strResponse = await HttpUtils.GetWebResponseAsync(
                new Uri(robot.NucleoUrl, $"/vendors/{this.Vendor.ToString().ToLower()}/robots/{robot.Serial}/messages"),
                postData: jsonPayload,
                requestDate: date,
                headers: new Dictionary<string, string>()
                {
                    ["Authorization"] = $"NEATOAPP {hmac}"
                });
            RobotState newState = RobotState.FromJson(strResponse);
            if (newState != null && this.RobotStateUpdated != null)
            {
                this.RobotStateUpdated(this, new RobotStateEventArgs { State = newState });
            }
            return newState;
        }

        /// <summary>
        /// Gets or retrieve the access token.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            if (string.IsNullOrEmpty(this.accessToken))
            {
                var tokenResponseStr = await HttpUtils.GetWebResponseAsync(
                    new Uri($"{VORWERK_API_ROOT_URI}/sessions"),
                    postData: $"email={this.Username}&password={this.Password}");
                var tokenResponse = JsonConvert.DeserializeObject(tokenResponseStr) as JObject;
                this.accessToken = tokenResponse["access_token"].Value<string>();
            }
            return this.accessToken;
        }

        /// <summary>
        /// Computes the HMAC256.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private static string ComputeHMAC(string key, string message)
        {
            var encoding = new ASCIIEncoding();
            var hmac256 = new HMACSHA256(encoding.GetBytes(key));
            var hash = hmac256.ComputeHash(encoding.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public class RobotStateEventArgs : EventArgs
        {
            public RobotState State { get; set; }
        }
    }
}

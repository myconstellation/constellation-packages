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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PoolCop.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// PoolCop communication interface
    /// </summary>
    public class PoolCopInterface
    {
        /// <summary>
        /// The poolcop API default language
        /// </summary>
        private const string POOLCOP_API_DEFAULT_LANGUAGE = "en";
        /// <summary>
        /// The poolcop API root URI
        /// </summary>
        private const string POOLCOP_API_ROOT_URI = "https://poolcopilot.com/api/v1";

        /// <summary>
        /// NumberFormatInfo to parse decimal with dot separator
        /// </summary>
        private readonly NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        /// <summary>
        /// Occurs when the PoolCop status changed.
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// Occurs when the PoolCop API status changed.
        /// </summary>
        public event EventHandler APIStatusChanged;

        /// <summary>
        /// Gets the API secret key.
        /// </summary>
        public string APIKey { get; private set; }
        /// <summary>
        /// Gets or sets the API language (en, fr or es).
        /// </summary>
        public string APILanguage { get; set; } = POOLCOP_API_DEFAULT_LANGUAGE;
        /// <summary>
        /// Gets the current PoolCop status.
        /// </summary>
        public PoolCopStatus Status { get; private set; } = new PoolCopStatus();

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolCopInterface"/> class.
        /// </summary>
        /// <param name="apiKey">The PoolCop API secret key.</param>
        public PoolCopInterface(string apiKey)
        {
            this.APIKey = apiKey;
        }

        /// <summary>
        /// Switches the auxiliary state.
        /// </summary>
        /// <param name="aux">The aux.</param>
        /// <returns></returns>
        public Task<CommandResult> SwitchAuxiliary(Auxiliary aux)
        {
            return this.SwitchAuxiliary(aux.Id);
        }

        /// <summary>
        /// Switches the auxiliary state.
        /// </summary>
        /// <param name="auxId">The aux identifier.</param>
        /// <returns></returns>
        public Task<CommandResult> SwitchAuxiliary(int auxId)
        {
            return this.ExecuteCommand($"aux/{auxId}");
        }

        /// <summary>
        /// Clears the alarm.
        /// </summary>
        /// <returns></returns>
        public Task<CommandResult> ClearAlarm()
        {
            return this.ExecuteCommand("clear_alarm");
        }

        /// <summary>
        /// Switches the state of the pump.
        /// </summary>
        /// <returns></returns>
        public Task<CommandResult> SwitchPumpState()
        {
            return this.ExecuteCommand($"pump");
        }

        /// <summary>
        /// Queries the PoolCop by using the PoolCopilot interface or local interface if failure.
        /// </summary>
        /// <returns></returns>
        public async Task Query()
        {
            this.Status = await this.RequestPoolCopilotStatus();
            this.StatusChanged?.Invoke(this, EventArgs.Empty);
            this.APIStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private async Task<CommandResult> ExecuteCommand(string command)
        {
            var token = await this.GetAuthentificationToken();
            var strReponse = await HttpUtils.GetWebResponseAsync(new Uri($"{POOLCOP_API_ROOT_URI}/command/{command}"), method: "POST", headers: new Dictionary<string, string>()
            {
                ["PoolCop-Token"] = token,
                ["X-PoolCopilot-Lang"] = this.APILanguage
            });
            var commandResult = CommandResult.FromJson(strReponse);
            this.Status.API = commandResult.API;
            this.APIStatusChanged?.Invoke(this, EventArgs.Empty);
            return commandResult;
        }

        /// <summary>
        /// Gets or retrieve the authentification token.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAuthentificationToken()
        {
            string token = this.Status?.API?.Token;
            if (!string.IsNullOrEmpty(token) && this.Status?.API?.RemainingCalls < 1 && DateTime.UtcNow < this.Status.API.ExpirationDate)
            {
                token = null;
            }
            if (token == null)
            {
                var tokenResponseStr = await HttpUtils.GetWebResponseAsync(new Uri($"{POOLCOP_API_ROOT_URI}/token"), postData: $"APIKEY={this.APIKey}");
                var tokenResponse = JsonConvert.DeserializeObject(tokenResponseStr) as JObject;
                return tokenResponse["token"].Value<string>();
            }
            else
            {
                return token;
            }
        }

        /// <summary>
        /// Requests the PoolCop status from the PoolCopilot API.
        /// </summary>
        /// <returns></returns>
        private async Task<PoolCopStatus> RequestPoolCopilotStatus()
        {
            var token = await this.GetAuthentificationToken();
            var statusResponse = await HttpUtils.GetWebResponseAsync(new Uri(POOLCOP_API_ROOT_URI + "/status"), headers: new Dictionary<string, string>()
            {
                ["PoolCop-Token"] = token,
                ["X-PoolCopilot-Lang"] = this.APILanguage
            });
            return PoolCopStatus.FromJson(statusResponse);
        }
    }
}

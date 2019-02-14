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
        /// Gets the PoolCop local address.
        /// </summary>
        public string LocalAddress { get; private set; }
        /// <summary>
        /// Gets the API secret key.
        /// </summary>
        public string APIKey { get; private set; }
        /// <summary>
        /// Gets the current PoolCop status.
        /// </summary>
        public PoolCopStatus Status { get; private set; } = new PoolCopStatus();

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolCopInterface"/> class.
        /// </summary>
        /// <param name="apiKey">The PoolCop API secret key.</param>
        /// <param name="localAddress">The PoolCop local address.</param>
        public PoolCopInterface(string apiKey, string localAddress)
        {
            this.APIKey = apiKey;
            this.LocalAddress = localAddress;
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
            return this.ExecuteCommand($"aux/pump");
        }

        /// <summary>
        /// Queries the PoolCop by using the PoolCopilot interface or local interface if failure.
        /// </summary>
        /// <returns></returns>
        public async Task Query()
        {
            try
            {
                this.Status = await this.RequestPoolCopilotStatus();                
            }
            catch when(!string.IsNullOrEmpty(this.LocalAddress))
            {
                this.Status = await this.RequestLocalStatus();
            }
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
                ["PoolCop-Token"] = token
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
                ["PoolCop-Token"] = token
            });
            return PoolCopStatus.FromJson(statusResponse);
        }

        /// <summary>
        /// Requests the PoolCop status from the local service.
        /// </summary>
        /// <returns></returns>
        private async Task<PoolCopStatus> RequestLocalStatus()
        {
            // Query local API
            var poolDataStr = await HttpUtils.GetWebResponseAsync(new Uri($"http://{LocalAddress}/ajax.htm?request=poolData"));
            var auxDataStr = await HttpUtils.GetWebResponseAsync(new Uri($"http://{LocalAddress}/ajax.htm?request=auxData"));
            var poolData = JsonConvert.DeserializeObject(poolDataStr) as JObject;
            var auxData = JsonConvert.DeserializeObject(auxDataStr) as JObject;
            // Return PoolCopStatus with local info
            return new PoolCopStatus
            {
                API = null, // local mode
                Pool = new Pool
                {
                    Nickname = "Local PoolCop",
                    Poolcop = "Local PoolCop"
                },
                PoolCop = new Models.PoolCop
                {
                    Voltage = double.Parse(poolData["BattVolts"].Value<string>().Replace(" VDC", ""), nfi),
                    Date = DateTime.Parse($"{poolData["Date"].Value<string>()} {poolData["Time"].Value<string>()}"),
                    Orp = poolData["ORP"].Value<int>(),
                    PH = poolData["pH"].Value<double>(),
                    Pressure = (int)(double.Parse(poolData["Pressure"].Value<string>().Replace(" BAR", ""), nfi) * 10),
                    Status = new Status
                    {
                        ValvePosition = (ValvePosition)poolData["FilterPos"].Value<int>(),
                        Pump = poolData["PumpState"].Value<string>() == "On",
                        Aux1 = auxData.Property("Aux1State").Value.ToString() == "On",
                        Aux2 = auxData.Property("Aux2State").Value.ToString() == "On",
                        Aux3 = auxData.Property("Aux3State").Value.ToString() == "On",
                        Aux4 = auxData.Property("Aux4State").Value.ToString() == "On",
                        Aux5 = auxData.Property("Aux5State").Value.ToString() == "On",
                        Aux6 = auxData.Property("Aux6State").Value.ToString() == "On",
                    },
                    Temperature = new Temperature
                    {
                        Water = double.Parse(poolData["WaterTemp"].Value<string>().Replace(" C", ""), nfi),
                    },
                    Auxiliaries = auxData.Properties()
                            .Where(p => p.Name.EndsWith("State"))
                            .Select(p => new Auxiliary
                            {
                                Id = Convert.ToInt32(p.Name.Substring(3, 1)),
                                Status = p.Value.ToString() == "On",
                                Label = p.Name.Replace("State", "")
                            })
                            .ToList()
                }
            };
        }
    }
}

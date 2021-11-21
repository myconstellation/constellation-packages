/*
 *	 Nest Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2015 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace GoogleNest
{
    using Constellation.Package;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program : PackageBase
    {
        private const string GOOGLE_API_REFRESH_TOKEN_URI = "https://www.googleapis.com/oauth2/v4/token?client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token";
        private const string SDM_API_URI = "https://smartdevicemanagement.googleapis.com/v1/enterprises/{0}/devices/{1}";

        private string AccessToken = "";
        private bool isAuthRevoked = false;
        private bool refreshTokenRequested = true;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue<string>("RefreshToken")))
            {
                PackageHost.WriteError("RefreshToken undefined. Please see documentation!");
                return;
            }
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue<string>("ClientID")))
            {
                PackageHost.WriteError("ClientID undefined. Please see documentation!");
                return;
            }
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue<string>("ClientSecret")))
            {
                PackageHost.WriteError("ClientSecret undefined. Please see documentation!");
                return;
            }
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue<string>("ProjectId")))
            {
                PackageHost.WriteError("ProjectId undefined. Please see documentation!");
                return;
            }
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue<string>("DeviceId")))
            {
                PackageHost.WriteError("DeviceId undefined. Please see documentation!");
                return;
            }
            if (PackageHost.GetSettingValue<int>("RefreshInterval") <= 0)
            {
                PackageHost.WriteError("RefreshInterval undefined. Please see documentation!");
                return;
            }

            // Main loop to pull updated date from Google Nest API.
            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning && !this.isAuthRevoked)
                {
                    this.RefreshGoogleNestData();

                    Thread.Sleep(PackageHost.GetSettingValue<int>("RefreshInterval"));
                }
            });

            // Started !
            PackageHost.WriteInfo("Nest package started!");
        }

        /// <summary>
        /// Main function. Generates main StateObject for Nest and handles refreshing the token.
        /// </summary>
        private void RefreshGoogleNestData()
        {
            this.GenerateStateobject();

            if (this.refreshTokenRequested == true)
            {
                this.RefreshToken();

                // Pulling the refreshed data right after receiving the new token.
                this.GenerateStateobject();
            }
        }

        /// <summary>
        /// Calls the Google SMD API to get last data and pushs a state object from what it retreived. 
        /// Changes the this.refreshTokenRequested to true if call fails because of revoked token.
        /// </summary>
        private void GenerateStateobject()
        {
            HttpWebRequest request = null;
            try
            {
                PackageHost.WriteInfo("Connecting to smart device management API ...");

                // Preparing the request.
                request = WebRequest.CreateHttp(string.Format(SDM_API_URI, PackageHost.GetSettingValue<string>("ProjectId"), PackageHost.GetSettingValue<string>("DeviceId")));
                request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                request.Accept = "text/event-stream";
                request.Headers.Add(string.Format("Authorization: Bearer {0}", AccessToken));

                string content = "";
                try
                {
                    WebResponse response = request.GetResponse();

                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }

                }
                catch (WebException)
                {
                    this.refreshTokenRequested = true;
                    PackageHost.WriteInfo("Token looks expired, refresh requested.");
                }

                if (!string.IsNullOrEmpty(content))
                {
                    // Cleaning and extracting information in a StateObject.
                    content = content.Replace("sdm.devices.types.", "").Replace("sdm.devices.traits.", ""); 

                    dynamic device = JsonConvert.DeserializeObject(content);
                    PackageHost.PushStateObject(device.parentRelations[0].displayName.ToString(), device, "Nest." + device.type,
                        new Dictionary<string, object>()
                        {
                                        { "DeviceId", PackageHost.GetSettingValue<string>("DeviceId") },
                                        { "DeviceType", device.type },
                        },
                        PackageHost.GetSettingValue<int>("RefreshInterval") / 1000 * 2);

                    PackageHost.WriteInfo("Google Nest StateObjects updated.");
                }
                else
                {
                    PackageHost.WriteError("No content received, Google Nest StateObjects were not updated.");
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Exception while generating StateObject : ", ex.Message);
            }
            finally
            {
                if (null != request)
                {
                    request.Abort();
                    request = null;
                }
            }
        }

        /// <summary>
        /// Renews the token from Google refresh token API. Pushes it as a StateObject.
        /// Changes the this.refreshTokenRequested to false once it's done.
        /// </summary>
        private void RefreshToken()
        {
            HttpWebRequest request = null;

            try
            {
                PackageHost.WriteInfo("Renewing token from Google API ...");

                // Preparing request.
                request = WebRequest.CreateHttp(string.Format(GOOGLE_API_REFRESH_TOKEN_URI, PackageHost.GetSettingValue<string>("ClientID"), PackageHost.GetSettingValue<string>("ClientSecret"), PackageHost.GetSettingValue<string>("RefreshToken")));
                request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Accept = "text/event-stream";

                int lifetimeAccessToken = 0;

                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = null;
                    while (null != (line = reader.ReadLine()) && !this.isAuthRevoked)
                    {
                        if (line.Trim().StartsWith("\"access_token\":"))
                        {
                            // Searching for the access token among the response lines.
                            this.AccessToken = line.Trim().Replace("\"", "").Replace("access_token:", "").Replace(",", "");
                            this.refreshTokenRequested = false;
                            PackageHost.WriteInfo("Token refreshed.");
                        }

                        if (line.Trim().StartsWith("\"expires_in\":"))
                        {
                            lifetimeAccessToken = Convert.ToInt32(line.Trim().Replace("\"", "").Replace("expires_in:", "").Replace(",", "").Trim());
                        }
                    }

                    // Refreshing the Access Token StateObject.
                    if (!string.IsNullOrEmpty(this.AccessToken) && lifetimeAccessToken != 0)
                    {
                        PackageHost.PushStateObject("AccessToken", this.AccessToken, lifetime: lifetimeAccessToken);
                    }
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Exception while refreshing token : ", ex.Message);
            }
            finally
            {
                if (null != request)
                {
                    request.Abort();
                    request = null;
                }
            }
        }

        /// <summary>
        /// Sets the device heat temperature.
        /// </summary>
        /// <param name="value">The new heat temperature.</param>
        [MessageCallback]
        private bool SetHeatTemperature(float value)
        {
            return this.ExecuteCommand("sdm.devices.commands.ThermostatTemperatureSetpoint.SetHeat", JsonConvert.SerializeObject(new Dictionary<string, object>() { { "heatCelsius", value } }));
        }

        /// <summary>
        /// Sets the device cool temperature.
        /// </summary>
        /// <param name="value">The new cool temperature.</param>
        [MessageCallback]
        private bool SetCoolTemperature(float value)
        {
            return this.ExecuteCommand("sdm.devices.commands.ThermostatTemperatureSetpoint.SetCool", JsonConvert.SerializeObject(new Dictionary<string, object>() { { "coolCelsius", value } }));
        }

        /// <summary>
        /// Sets the device heat & cool range temperatures.
        /// </summary>
        /// <param name="heatValue">The new heat temperature.</param>
        /// <param name="coolValue">The new cool temperature.</param>
        [MessageCallback]
        private bool SetTemperatureRange(float heatValue, float coolValue)
        {
            return this.ExecuteCommand("sdm.devices.commands.ThermostatTemperatureSetpoint.SetRange", JsonConvert.SerializeObject(new Dictionary<string, object>() { { "heatCelsius", heatValue }, { "coolCelsius", coolValue } }));
        }

        /// <summary>
        /// Sets the device fan timer.
        /// </summary>
        /// <param name="timerModeValue">ON or OFF.</param>
        /// <param name="durationValue">Timer duration (in seconds). Set to 15 min if void (900 seconds).</param>
        [MessageCallback]
        private bool SetFanTimer(string timerModeValue, int? durationValue = null)
        {
            var dict = new Dictionary<string, object>() { { "timerMode", timerModeValue } };
            if (durationValue.HasValue) dict.Add("duration", string.Format("{0}s", durationValue));

            return this.ExecuteCommand("sdm.devices.commands.Fan.SetTimer", JsonConvert.SerializeObject(dict));
        }

        /// <summary>
        /// Sets the device eco mode.
        /// </summary>
        /// <param name="value">The new mode : OFF, MANUAL_ECO.</param>
        [MessageCallback]
        private bool SetEcoMode(string value)
        {
            return this.ExecuteCommand("sdm.devices.commands.ThermostatEco.SetMode", JsonConvert.SerializeObject(new Dictionary<string, object>() { { "mode", value } }));
        }

        /// <summary>
        /// Sets the device heat mode.
        /// </summary>
        /// <param name="value">The new mode : OFF, HEAT, COOL, HEATCOOL.</param>
        [MessageCallback]
        private bool SetMode(string value)
        {
            return this.ExecuteCommand("sdm.devices.commands.ThermostatMode.SetMode", JsonConvert.SerializeObject(new Dictionary<string, object>() { { "mode", value } }));
        }

        /// <summary>
        /// Fonction récéptionnant le push du service pub/sub GCP.
        /// </summary>
        [MessageCallback()]
        public void ReceivePush(object data)
        {
            PackageHost.WriteWarn("Push nest triggered by remote API. Checking data integrity before refreshing.");

            // Extracting data from payload.
            string jwt = ((dynamic)data)?.message?.data;
            if (!string.IsNullOrEmpty(jwt))
            {
                // Decoding JWT.
                string jwtDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(jwt));
                if (!string.IsNullOrEmpty(jwtDecoded))
                {
                    string name = null;
                    dynamic json = JObject.Parse(jwtDecoded);

                    try
                    {
                        // Extracting the name of the device which originated the push.
                        name = Convert.ToString(json.resourceUpdate.name);

                        // Checking if the device matches our config.
                        if (!string.IsNullOrEmpty(name) &&
                                name.Contains(PackageHost.GetSettingValue<string>("ProjectId")) &&
                                name.Contains(PackageHost.GetSettingValue<string>("DeviceId")))
                        {
                            // Request refresh.
                            this.RefreshGoogleNestData();
                        }
                        else
                        {
                            PackageHost.WriteError("ProjectId or DeviceId received are invalid : " + name);
                        }
                    }
                    catch (Exception)
                    {
                        PackageHost.WriteError("Failed to check Client ID in received message (resourceUpdate.name) : " + jwtDecoded);
                    }
                }
                else
                {
                    PackageHost.WriteError("Received data JWT conversion failed : " + jwt);
                }
            }
            else
            {
                PackageHost.WriteError("Received data extracting failed : " + data);
            }
        }

        private bool ExecuteCommand(string command, string param, int tries = 3)
        {
            string payload = string.Format("{{ \"command\":\"{0}\",\"params\":{1} }}", command, param);
            HttpWebRequest request = null;
            try
            {
                request = WebRequest.CreateHttp(string.Format(SDM_API_URI + ":executeCommand", PackageHost.GetSettingValue<string>("ProjectId"), PackageHost.GetSettingValue<string>("DeviceId")));
                request.Method = "POST";
                request.Headers.Add(string.Format("Authorization: Bearer {0}", AccessToken));
                request.ContentType = "application/json";
                request.Accept = "*/*";
                request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                using (Stream requestBody = request.GetRequestStream())
                {
                    byte[] payloadData = UTF8Encoding.UTF8.GetBytes(payload);
                    requestBody.Write(payloadData, 0, payloadData.Length);
                    requestBody.Close();
                }

                var response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = null;
                    while ((null != (line = reader.ReadLine())))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            PackageHost.WriteInfo("POST '{1}' to '{0}'", command, param);
                        }
                    }
                }

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error on '{0}' : '{1}'", command, ex.ToString());
                if (tries-- > 0)
                {
                    return this.ExecuteCommand(command, param, tries);
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (null != request)
                {
                    request.Abort();
                }
            }
        }
    }
}

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
        private const int SLEEP_AFTER_ERROR = 30000; //ms
        private string AccessToken = "";
        private const string GOOGLE_API_REFRESH_TOKEN_URI = "https://www.googleapis.com/oauth2/v4/token?client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token";
        private const string SDM_API_URI = "https://smartdevicemanagement.googleapis.com/v1/enterprises/{0}/devices/{1}";

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

            Task.Factory.StartNew(() =>
            {
                bool authRevoked = false;
                bool refreshRequested = true;
                while (PackageHost.IsRunning && !authRevoked)
                {
                    HttpWebRequest request = null;
                    try
                    {
                        if (refreshRequested == true)
                        {
                            PackageHost.WriteInfo("Renewing token from Google API ...");
                            request = WebRequest.CreateHttp(string.Format(GOOGLE_API_REFRESH_TOKEN_URI, PackageHost.GetSettingValue<string>("ClientID"), PackageHost.GetSettingValue<string>("ClientSecret"), PackageHost.GetSettingValue<string>("RefreshToken")));
                            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                            request.Method = "POST";
                            request.ContentLength = 0;
                            request.Accept = "text/event-stream";

                            WebResponse response = request.GetResponse();
                            using (Stream stream = response.GetResponseStream())
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string line = null;
                                while (null != (line = reader.ReadLine()) && !authRevoked)
                                {
                                    if (line.Trim().StartsWith("\"access_token\":"))
                                    {
                                        AccessToken = line.Trim().Replace("\"", "").Replace("access_token:", "").Replace(",", "");
                                        refreshRequested = false;
                                        PackageHost.WriteInfo("Token refreshed.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            PackageHost.WriteInfo("Connecting to smart device management API ...");
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
                            catch (WebException ex)
                            {
                                refreshRequested = true;
                                PackageHost.WriteInfo("Token looks expired.");
                            }

                            if (!string.IsNullOrEmpty(content))
                            {
                                content = content.Replace("sdm.devices.types.", "").Replace("sdm.devices.traits.", ""); // Easier to read.

                                dynamic device = JsonConvert.DeserializeObject(content);
                                PackageHost.PushStateObject(device.parentRelations[0].displayName.ToString(), device, "Nest." + device.type,
                                    new Dictionary<string, object>()
                                    {
                                        { "DeviceId", PackageHost.GetSettingValue<string>("DeviceId") },
                                        { "DeviceType", device.type },
                                    }, 
                                    PackageHost.GetSettingValue<int>("RefreshInterval")/1000*2);


                                PackageHost.WriteInfo("Google Nest StateObjects updated.");
                                Thread.Sleep(PackageHost.GetSettingValue<int>("RefreshInterval"));
                            }
                            else
                            {
                                PackageHost.WriteError("No content received.");
                                Thread.Sleep(SLEEP_AFTER_ERROR);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError(ex.Message);
                        Thread.Sleep(SLEEP_AFTER_ERROR);
                    }
                    finally
                    {
                        if (null != request)
                        {
                            request.Abort();
                        }
                    }
                }
            });

            // Started !
            PackageHost.WriteInfo("Nest package started!");
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
            var dict = new Dictionary<string, object>() {{ "timerMode", timerModeValue }};
            if (durationValue.HasValue) dict.Add("duration", string.Format("{0}s", durationValue) );

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

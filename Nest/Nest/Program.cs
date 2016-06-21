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

namespace Nest
{
    using Constellation.Package;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program : PackageBase
    {
        private const int SLEEP_AFTER_ERROR = 30000; //ms
        private const string NEST_ROOT_URI = "https://developer-api.nest.com/";

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue<string>("AccessToken")))
            {
                PackageHost.WriteError("AccesToken undefined. Please see documentation!");
                return;
            }
     
            Task.Factory.StartNew(() =>
            {
                bool authRevoked = false;
                while (PackageHost.IsRunning && !authRevoked)
                {
                    HttpWebRequest request = null;
                    try
                    {
                        PackageHost.WriteInfo("Connecting to Nest REST Streaming API ...");
                        request = WebRequest.CreateHttp(NEST_ROOT_URI + "/?auth=" + PackageHost.GetSettingValue<string>("AccessToken"));
                        request.Accept = "text/event-stream";

                        WebResponse response = request.GetResponse();
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line = null;
                            while (null != (line = reader.ReadLine()) && !authRevoked)
                            {
                                if (line.Equals("event: auth_revoked"))
                                {
                                    PackageHost.WriteError("The Access Token is revoked!");
                                    authRevoked = true;
                                    break;
                                }
                                else if (line.StartsWith("data:") && !line.EndsWith("null"))
                                {
                                    dynamic nest = JsonConvert.DeserializeObject(line.Substring(5));
                                    // Devices
                                    JObject devices = nest.data.devices as JObject;
                                    foreach (var deviceType in devices)
                                    {
                                        foreach (JProperty device in deviceType.Value)
                                        {
                                            foreach (JObject deviceObject in device)
                                            {
                                                PackageHost.PushStateObject(deviceObject.Property("name").Value.ToString(), deviceObject, "Nest." + deviceType.Key,
                                                    new Dictionary<string, object>()
                                                    {
                                                        { "DeviceId", device.Name },
                                                        { "DeviceType", deviceType.Key },
                                                    });
                                                Thread.Sleep(100);
                                            }
                                        }
                                    }
                                    // Structures
                                    JObject structures = nest.data.structures as JObject;
                                    foreach (var structure in structures)
                                    {
                                        JObject structureObject = structure.Value as JObject;
                                        PackageHost.PushStateObject(structureObject.Property("name").Value.ToString(), structureObject, "Nest.Structure",
                                            new Dictionary<string, object>() { { "StructureId", structure.Key } });
                                        Thread.Sleep(100);
                                    }
                                    // Done !
                                    PackageHost.WriteInfo("Nest StateObjects updated");
                                }
                                else if (!string.IsNullOrEmpty(line))
                                {
                                    PackageHost.WriteDebug(line);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
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
        /// Sets the away mode for the specified Structure ID.
        /// </summary>
        /// <param name="structureId">the structure's ID.</param>
        /// <param name="isAway">if set to <c>true</c> if is away.</param>
        [MessageCallback]
        public void SetAwayMode(string structureId, bool isAway)
        {
            this.SetProperty("structures/" + structureId, "away", isAway ? "away" : "home");
        }

        /// <summary>
        /// Sets the target temperature for the specified thermostat.
        /// </summary>
        /// <param name="thermostatId">The thermostat's ID.</param>
        /// <param name="temperature">The target temperature in Celcius.</param>
        [MessageCallback]
        public void SetTargetTemperature(string thermostatId, double temperature)
        {
            this.SetProperty("devices/thermostats/" + thermostatId, "target_temperature_c", temperature);
        }

        /// <summary>
        /// Sets the property for a specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        [MessageCallback]
        private void SetProperty(string path, string propertyName, object value)
        {
            this.Set(path, JsonConvert.SerializeObject(new Dictionary<string, object>() { { propertyName, value } }));
        }

        private void Set(string path, string payload, int tries = 3)
        {
            HttpWebRequest request = null;
            try
            {
                request = WebRequest.CreateHttp(NEST_ROOT_URI + path + "?auth=" + PackageHost.GetSettingValue<string>("AccessToken"));
                request.Method = "PUT";
                using (Stream requestBody = request.GetRequestStream())
                {
                    byte[] payloadData = UTF8Encoding.UTF8.GetBytes(payload);
                    requestBody.Write(payloadData, 0, payloadData.Length);
                    requestBody.Close();
                }
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = null;
                    while ((null != (line = reader.ReadLine())))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            PackageHost.WriteInfo("Put '{1}' to '{0}'", path, line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error on '{0}' : '{1}'", path, ex.ToString());
                if (tries-- > 0)
                {
                    this.Set(path, payload, tries);
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

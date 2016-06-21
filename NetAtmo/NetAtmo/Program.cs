/*
 *	 NetAtmo Package for Constellation
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

namespace NetAtmo
{
    using Constellation.Package;
    using NetatmoBot.Model;
    using NetatmoBot.Model.Measurements;
    using NetatmoBot.Model.Modules;
    using NetatmoBot.Services;
    using NetatmoBot.Services.Wrappers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            int nbSecond = 0;
            while (PackageHost.IsRunning)
            {
                if (nbSecond++ == 0)
                {
                    try
                    {
                        PackageHost.WriteInfo("Getting NetAtmo measurements");
                        // Authenticate user
                        AuthenticationService authService = new AuthenticationService(PackageHost.GetSettingValue<string>("Netatmo.ClientId"), PackageHost.GetSettingValue<string>("Netatmo.ClientSecret"));
                        AuthenticationToken authToken = authService.AuthenticateUser(PackageHost.GetSettingValue<string>("Netatmo.Username"), PackageHost.GetSettingValue<string>("Netatmo.Password"));
                        // Get user devices
                        var devices = new DevicesService(authToken, new HttpWrapper()).Get();
                        // Init Measurements Service 
                        var measurementsService = new MeasurementsService(authToken, new HttpWrapper());
                        // For each devices
                        foreach (var device in devices.Result.Devices)
                        {
                            // Push the device
                            this.PushData(measurementsService, device);
                            // Push each modules of the device
                            foreach (var moduleId in device.ModuleIds)
                            {
                                var module = devices.Result.Modules.FirstOrDefault(m => m.Id == moduleId);
                                if (module != null)
                                {
                                    this.PushData(measurementsService, device, module);
                                }
                                else
                                {
                                    PackageHost.WriteError("Module '{0}' not exist !", moduleId);
                                }
                            }
                        }
                        // OK !
                        PackageHost.WriteInfo("All measurements are pushed to the Constellation");
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error while getting NetAtmo measurements : " + ex.ToString());                        
                    }
                }
                
                Thread.Sleep(1000);
                if (nbSecond == PackageHost.GetSettingValue<int>("RefreshInterval"))
                {
                    nbSecond = 0;
                }
            }
        }

        private void PushData(MeasurementsService service, Device device, Module module = null)
        {
            // Get measures
            var measures = service.Get(device.Id, module);
            // Push device/module info
            if (module != null)
            {
                PackageHost.PushStateObject<Module>(string.Concat(module.Name.Replace(" ", ""), ".Info"), module, "NetAtmo.Module");
            }
            else
            {
                PackageHost.PushStateObject<Device>(string.Concat(device.ModuleName, ".Info"), device, "NetAtmo.Device");
            }
            // Push measure
            foreach (SensorMeasurement measure in measures.Result)
            {
                string sensorType = measure.GetType().Name;
                string stateObjectName = string.Concat(module != null ? module.Name.Replace(" ", "") : device.ModuleName, ".", sensorType.Replace("Measurement", ""));
                var metadatas = new Dictionary<string, object>()
                {
                    { "TimeStamp", measure.TimeStamp },
                    { "ModuleId", module?.Id ?? device.Id },
                    { "ModuleName", module?.Name ?? device.ModuleName }
                };
                var rainMeasurement = measure as RainMeasurement;
                if (rainMeasurement != null)
                {
                    metadatas.Add("HourlySum", rainMeasurement.MillimetersIn60Minutes);
                    metadatas.Add("DailySum", rainMeasurement.MillimetersIn24Hour);                    
                }
                PackageHost.PushStateObject<decimal>(stateObjectName, measure.Value, "NetAtmo." + sensorType, metadatas);
            }
            // Ok!
            PackageHost.WriteInfo("Successfully pushed {0} measurement(s).", measures.Result.Count);
        }
    }
}

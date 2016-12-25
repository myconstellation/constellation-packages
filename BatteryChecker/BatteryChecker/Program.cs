/*
 *	 BatteryChecker Package for Constellation
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

namespace BatteryChecker
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Management;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    // Win32_Battery Reference on http://www.powertheshell.com/reference/wmireference/root/cimv2/win32_battery/
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            Dictionary<string, BatteryState> batteries = new Dictionary<string, BatteryState>();
            Task.Factory.StartNew(() =>
                {
                    while (PackageHost.IsRunning)
                    {
                        try
                        {
                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Battery");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                            ManagementObjectCollection collection = searcher.Get();

                            foreach (ManagementObject mo in collection)
                            {
                                // Register new battery
                                string deviceID = BitConverter.ToString(new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(mo["DeviceID"].ToString()))).Replace("-", "");
                                if (!batteries.ContainsKey(deviceID))
                                {
                                    batteries.Add(deviceID, new BatteryState() { Name = mo["Name"].ToString().Trim(), DeviceID = mo["DeviceID"].ToString().Trim() });
                                }

                                // Get current properties
                                int estimatedChargeRemaining = Convert.ToInt32(mo["EstimatedChargeRemaining"].ToString());
                                int estimatedRunTime = Convert.ToInt32(mo["EstimatedRunTime"].ToString());
                                int statusCode = Convert.ToInt32(mo["BatteryStatus"].ToString());
                                string state = mo["Status"].ToString().Trim();

                                // If changes
                                if (batteries[deviceID].EstimatedChargeRemaining != estimatedChargeRemaining
                                    || batteries[deviceID].EstimatedRunTime != estimatedRunTime
                                    || batteries[deviceID].State != state
                                    || batteries[deviceID].StatusCode != statusCode)
                                {
                                    // Update 
                                    batteries[deviceID].EstimatedChargeRemaining = estimatedChargeRemaining;
                                    batteries[deviceID].EstimatedRunTime = estimatedRunTime;
                                    batteries[deviceID].State = state;
                                    batteries[deviceID].StatusCode = statusCode;
                                    // Push to Constellation
                                    PackageHost.PushStateObject(deviceID, batteries[deviceID]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            PackageHost.WriteError("Critical error : " + ex.ToString());
                        }

                        Thread.Sleep(PackageHost.GetSettingValue<int>("RefreshInterval"));
                    }
                });

            PackageHost.WriteInfo("Ready!");
        }
    }
}
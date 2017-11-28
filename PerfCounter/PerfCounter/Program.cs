/*
 *	 Windows PerfCounter Package for Constellation
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

namespace PerfCounter
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program : PackageBase
    {
        private Dictionary<string, PerformanceCounter> counters = new Dictionary<string, PerformanceCounter>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            try
            {
                PerfCounterSection config = PackageHost.GetSettingAsConfigurationSection<PerfCounterSection>("PerfCounters");
                foreach (PerfCounter counter in config.PerfCounters)
                {
                    try
                    {
                        var perfCounter = new PerformanceCounter()
                        {
                            CategoryName = counter.CategoryName,
                            CounterName = counter.CounterName,
                            ReadOnly = true
                        };
                        if (!string.IsNullOrEmpty(counter.InstanceName))
                        {
                            perfCounter.InstanceName = counter.InstanceName;
                        }
                        if (!string.IsNullOrEmpty(counter.MachineName))
                        {
                            perfCounter.MachineName = counter.MachineName;
                        }
                        perfCounter.NextValue();
                        counters.Add(counter.ID, perfCounter);
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError($"Unable to add the counter {counter.CategoryName}:{counter.CounterName} : {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to initialize the package. Check package settings !", ex);
            }

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        foreach (var counter in counters)
                        {
                            PackageHost.PushStateObject<float>(counter.Key, counter.Value.NextValue(), metadatas: new Dictionary<string, object>()
                            {
                                ["CategoryName"] = counter.Value.CategoryName,
                                ["CounterName"] = counter.Value.CounterName,
                                ["InstanceName"] = counter.Value.InstanceName,
                                ["MachineName"] = counter.Value.MachineName == "." ? Environment.MachineName : counter.Value.MachineName,
                            });
                        }

                        Thread.Sleep(PackageHost.GetSettingValue<int>("RefreshInterval"));
                    }
                    catch { }
                }
            }, TaskCreationOptions.LongRunning);

            PackageHost.WriteInfo($"{counters.Count} counter(s) loaded. Package started!");
        }
    }
}

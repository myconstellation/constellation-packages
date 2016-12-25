/*
 *	 SNMP Package for Constellation
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

namespace Snmp
{
    using Constellation;
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// SNMP Package
    /// </summary>
    public class Program : PackageBase
    {
        private static readonly TimeSpan STATEOBJECT_TIMEOUT = new TimeSpan(0, 0, 10); // 10 seconds

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            var config = PackageHost.GetSettingAsConfigurationSection<SnmpConfiguration>("snmpConfiguration");
            if (config != null)
            {
                foreach (Device device in config.Devices)
                {
                    PackageHost.WriteInfo($"Starting monitoring task for {device.Host}/{device.Community} (every {config.QueryInterval.TotalSeconds} sec)");
                    Task.Factory.StartNew(() =>
                    {
                        string snmpDeviceId = $"{device.Host}/{device.Community}";
                        int stateObjectTimeout = (int)config.QueryInterval.Add(STATEOBJECT_TIMEOUT).TotalSeconds;
                        var snmpDeviceMetadatas = new Dictionary<string, object>()
                        {
                            ["Host"] = device.Host,
                            ["Community"] = device.Community
                        };
                        DateTime lastQuery = DateTime.MinValue;
                        while (PackageHost.IsRunning)
                        {
                            if (DateTime.Now.Subtract(lastQuery) >= config.QueryInterval)
                            {
                                try
                                {
                                    SnmpDevice snmpResult = SnmpScanner.ScanDevice(device.Host, device.Community);
                                    if (config.MultipleStateObjectsPerDevice)
                                    {
                                        // Push Description
                                        PackageHost.PushStateObject($"{snmpDeviceId}/Description", snmpResult.Description,
                                            lifetime: stateObjectTimeout,
                                            metadatas: snmpDeviceMetadatas);

                                        // Push Addresses
                                        foreach (var address in snmpResult.Addresses)
                                        {
                                            PackageHost.PushStateObject($"{snmpDeviceId}/Addresses/{address.Key}", address.Value,
                                                lifetime: stateObjectTimeout,
                                                metadatas: new Dictionary<string, object>(snmpDeviceMetadatas)
                                                {
                                                    ["Key"] = address.Key
                                                });
                                        }

                                        // Push Network Interfaces
                                        foreach (var netInterface in snmpResult.Interfaces)
                                        {
                                            PackageHost.PushStateObject($"{snmpDeviceId}/Interfaces/{netInterface.Key}", netInterface.Value,
                                                lifetime: stateObjectTimeout,
                                                metadatas: new Dictionary<string, object>(snmpDeviceMetadatas)
                                                {
                                                    ["Key"] = netInterface.Key
                                                });
                                        }

                                        // Push Host
                                        if (snmpResult.Host != null)
                                        {
                                            PackageHost.PushStateObject($"{snmpDeviceId}/Host", snmpResult.Host,
                                                lifetime: stateObjectTimeout,
                                                metadatas: snmpDeviceMetadatas);
                                        }

                                        // Push ProcessorsLoad
                                        if (snmpResult.ProcessorsLoad != null)
                                        {
                                            foreach (var proc in snmpResult.ProcessorsLoad)
                                            {
                                                PackageHost.PushStateObject($"{snmpDeviceId}/Processors/{proc.Key}", proc.Value,
                                                    lifetime: stateObjectTimeout,
                                                    metadatas: new Dictionary<string, object>(snmpDeviceMetadatas)
                                                    {
                                                        ["Key"] = proc.Key
                                                    });
                                            }
                                        }

                                        // Push Storages
                                        if (snmpResult.Storages != null)
                                        {
                                            foreach (var storage in snmpResult.Storages)
                                            {
                                                PackageHost.PushStateObject($"{snmpDeviceId}/Storages/{storage.Key}", storage.Value,
                                                    lifetime: stateObjectTimeout,
                                                    metadatas: new Dictionary<string, object>(snmpDeviceMetadatas)
                                                    {
                                                        ["Key"] = storage.Key
                                                    });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Push the full SNMP device
                                        PackageHost.PushStateObject(snmpDeviceId, snmpResult,
                                            lifetime: stateObjectTimeout,
                                            metadatas: snmpDeviceMetadatas);
                                    }
                                }
                                catch (MissingMemberException)
                                {
                                    // Device offline -> Send message "DeviceOffline" to the "SNMP" group
                                    PackageHost.CreateMessageProxy(MessageScope.ScopeType.Group, "SNMP").DeviceOffline(snmpDeviceId);
                                }
                                catch (Exception ex)
                                {
                                    PackageHost.WriteDebug($"Error while scanning {snmpDeviceId} : {ex.Message}");
                                }
                                lastQuery = DateTime.Now;
                            }
                            Thread.Sleep(1000);
                        }
                    }, TaskCreationOptions.LongRunning);
                }
            }
            PackageHost.WriteInfo("Package started!");
        }

        /// <summary>
        /// Checks the SNMP agent for the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="community">The community.</param>
        /// <returns><c>true</c> if the SNMP agent is valid, otherwise, <c>false</c></returns>
        [MessageCallback]
        public bool CheckAgent(string host, string community = SnmpScanner.DEFAULT_COMMUNITY)
        {
            return SnmpScanner.CheckAgent(host, community);
        }

        /// <summary>
        /// Scans the SNMP device.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="community">The community.</param>
        /// <returns>SNMP device</returns>
        [MessageCallback]
        public SnmpDevice ScanDevice(string host, string community = SnmpScanner.DEFAULT_COMMUNITY)
        {
            try
            {
                return SnmpScanner.ScanDevice(host, community);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Unable to scan '{host}' : {ex.Message}");
                return null;
            }
        }
    }
}

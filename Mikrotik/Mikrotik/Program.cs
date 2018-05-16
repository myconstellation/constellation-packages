/*
 *	 Mikrotik Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Mikrotik
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using tik4net;
    using tik4net.Objects;
    using tik4net.Objects.CapsMan;
    using tik4net.Objects.Interface;
    using tik4net.Objects.Ip;
    using tik4net.Objects.Ip.DhcpServer;
    using tik4net.Objects.Ipv6;
    using tik4net.Objects.Queue;
    using tik4net.Objects.System;

    /// <summary>
    /// Mikrotik Constellation package class
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    [StateObjectKnownTypes(
        typeof(SystemResource),
        typeof(List<Interface>),
        typeof(List<IpAddress>),
        typeof(List<IpPool>),
        typeof(List<QueueSimple>),
        typeof(List<DhcpServerLease>),
        typeof(List<IpDhcpClient>),
        typeof(List<Ipv6Neighbor>),
        typeof(List<Ipv6Pool>),
        typeof(List<Ipv6Address>),
        typeof(List<Ipv6DhcpClient>),
        typeof(List<CapsManRegistrationTable>))]
    public class Program : PackageBase
    {
        private ITikConnection connection = null;
        private List<string> unsupportedCommands = new List<string>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            DateTime lastQuery = DateTime.MinValue;
            Thread thQuery = new Thread(new ThreadStart(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (DateTime.Now.Subtract(lastQuery).TotalMilliseconds >= PackageHost.GetSettingValue<int>("QueryInterval"))
                    {
                        if (this.connection == null || !this.connection.IsOpened)
                        {
                            this.connection = this.GetConnection();
                        }
                        if (this.connection != null && this.connection.IsOpened)
                        {
                            try
                            {
                                // Test connection with simple command
                                connection.CreateCommand("/system/identity/print").ExecuteScalar();
                                // Query & push
                                // - Base objects -
                                this.QueryAndPush("SystemResource", () => connection.LoadSingle<SystemResource>());
                                this.QueryAndPush("Interfaces", () => connection.LoadList<Interface>());
                                this.QueryAndPush("IpAddresses", () => connection.LoadList<IpAddress>());
                                this.QueryAndPush("Pools", () => connection.LoadList<IpPool>());
                                this.QueryAndPush("Queues", () => connection.LoadList<QueueSimple>());
                                // - DHCP client & server leases -
                                this.QueryAndPush("DhcpClient", () => connection.LoadList<IpDhcpClient>());
                                this.QueryAndPush("DhcpServerLeases", () => connection.LoadList<DhcpServerLease>());
                                // - IPv6 objects -
                                this.QueryAndPush("Ipv6Neighbors", () => connection.LoadList<Ipv6Neighbor>());
                                this.QueryAndPush("Ipv6Pools", () => connection.LoadList<Ipv6Pool>());
                                this.QueryAndPush("Ipv6Addresses", () => connection.LoadList<Ipv6Address>());
                                this.QueryAndPush("Ipv6DhcpClient", () => connection.LoadList<Ipv6DhcpClient>());
                                // - CAPSMAN registration table  -
                                this.QueryAndPush("WirelessClients", () => connection.LoadList<CapsManRegistrationTable>());
                            }
                            catch (IOException)
                            {
                                this.connection = null;
                            }
                        }
                        lastQuery = DateTime.Now;
                    }
                    Thread.Sleep(100);
                }
            }));
            thQuery.Start();
            PackageHost.WriteInfo("Package started !");
        }

        /// <summary>
        /// Called when the package is shutdown (disconnected from Constellation)
        /// </summary>
        public override void OnShutdown()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
            }
        }

        /// <summary>
        /// Query the Mikrotik device and push the result as StateObject.
        /// </summary>
        /// <param name="name">The StateObject name.</param>
        /// <param name="query">The query.</param>
        /// <param name="type">The StateObject type.</param>
        private void QueryAndPush(string name, Func<object> query, string type = "")
        {
            if (!this.unsupportedCommands.Contains(name))
            {
                try
                {
                    PackageHost.PushStateObject(name, query(), type,
                        metadatas: new Dictionary<string, object>()
                        {
                            ["Host"] = PackageHost.GetSettingValue("Host")
                        },
                        lifetime: PackageHost.GetSettingValue<int>("QueryInterval") * 2);
                }
                catch (TikCommandException ex) when (ex.Code == "0" || ex.Message.StartsWith("no such command"))
                {
                    this.unsupportedCommands.Add(name);
                    PackageHost.WriteWarn("{0} is unsupported : {1}", name, ex.Message);
                }
                catch (Exception ex)
                {
                    PackageHost.WriteError("Unable to query and push the StateObject '{0}' : {1}", name, ex.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the Mikrotik connection.
        /// </summary>
        /// <returns></returns>
        private ITikConnection GetConnection()
        {
            PackageHost.WriteInfo("Connecting to {0} ...", PackageHost.GetSettingValue("Host"));
            ITikConnection connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
            try
            {
                connection.Open(PackageHost.GetSettingValue("Host"), PackageHost.GetSettingValue("Username"), PackageHost.GetSettingValue("Password"));
                PackageHost.WriteInfo("Connected!");
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Unable to connect to {PackageHost.GetSettingValue("Host")} : {ex.Message}");
                connection.Dispose();
                connection = null;
            }
            return connection;
        }
    }
}

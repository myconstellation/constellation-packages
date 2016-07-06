/*
 *	 NetworkTools package for Constellation
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

namespace NetworkTools
{
    using Constellation.Package;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program : PackageBase
    {
        private Dictionary<string, DateTime> monitoringChecks = new Dictionary<string, DateTime>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        if (PackageHost.ContainsSetting("Monitoring"))
                        {
                            dynamic config = PackageHost.GetSettingAsJsonObject("Monitoring", true);
                            foreach (dynamic ressource in config)
                            {
                                string name = ressource.Name.Value;
                                if (monitoringChecks.ContainsKey(name) == false ||
                                    DateTime.Now.Subtract(monitoringChecks[name]).TotalSeconds >= (ressource["Interval"] != null ? ressource.Interval.Value : 60))
                                {
                                    this.CheckRessource(name, ressource.Type.Value, ressource);
                                    monitoringChecks[name] = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Monitor task error : " + ex.Message);
                    }
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Pings the specified host and return the response time.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="timeout">The timeout (5000ms by defaut).</param>
        /// <returns></returns>
        [MessageCallback]
        public long Ping(string host, int timeout = 5000)
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(host, timeout);
            if (reply.Status == IPStatus.Success)
            {
                PackageHost.WriteInfo($"Reply from {host}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl ?? 0}");
                return reply.RoundtripTime;
            }
            else
            {
                PackageHost.WriteInfo("Request to {0} failed : {1}", host, reply.Status.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Check a port's status by entering an address and port number above and return the response time.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout (5000ms by defaut).</param>
        /// <returns></returns>
        [MessageCallback]
        public long CheckPort(string host, int port, int timeout = 5000)
        {
            TcpClient tcpClient = new TcpClient();
            try
            {
                long reply = 0;
                tcpClient.BeginConnect(host, port, null, null);
                for (reply = 0; reply <= timeout; reply++)
                {
                    Thread.Sleep(1);
                    if (tcpClient.Connected)
                    {
                        PackageHost.WriteInfo($"{host}:{port} is open");
                        return reply;
                    }
                }
            }
            catch { }
            finally
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks the HTTP address and return the response time.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        [MessageCallback]
        public long CheckHttp(string address)
        {
            var client = new ExtendedWebClient();
            try
            {
                var sw = Stopwatch.StartNew();
                client.DownloadString(address);
                sw.Stop();
                return sw.ElapsedMilliseconds;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Scans TCP port range to discover which TCP ports are open on your target host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="startPort">The start port.</param>
        /// <param name="endPort">The end port.</param>
        /// <param name="timeout">The timeout (100 ms by default).</param>
        /// <returns></returns>
        [MessageCallback]
        public Dictionary<int, bool> ScanPort(string host, int startPort, int endPort, int timeout = 100)
        {
            var result = new ConcurrentDictionary<int, bool>();
            Parallel.For(startPort, endPort + 1, port =>
            {
                bool pResult = CheckPort(host, port, timeout) >= 0;
                result.AddOrUpdate(port, pResult, (p, r) => pResult);
            });
            return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Wakes up the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="macAddress">The mac address.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool WakeUp(string host, string macAddress)
        {
            try
            {
                var ip = Dns.GetHostAddresses(host).Where(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault();
                if (ip != null && !string.IsNullOrEmpty(macAddress))
                {
                    WOLClient.SendWakeUpPacket(ip, macAddress);
                    PackageHost.WriteInfo("WOL sent to '{0}' ('{1}').", host, macAddress);
                    return true;
                }
                else
                {
                    PackageHost.WriteError("Unable to wake the host '{0}' with MAC '{1}'. Invalid parameters", host, macAddress);
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error while wake up the host '{0}' with MAC '{1}' : {2}", host, macAddress, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Resolves a host name or IP address.
        /// </summary>
        /// <param name="host">The host or IP address.</param>
        /// <returns></returns>
        [MessageCallback]
        public string[] DnsLookup(string host)
        {
            try
            {
                return Dns.GetHostAddresses(host).Select(i => i.ToString()).ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        private void CheckRessource(string name, string type, dynamic jObject)
        {
            try
            {
                long result = -1;
                bool? state = null;
                var metadatas = new Dictionary<string, object>()
                {
                    ["Type"] = type
                };
                switch (type.ToLower())
                {
                    case "ping":
                        metadatas.Add("Hostname", jObject.Hostname.Value);
                        PingReply reply = new Ping().Send(jObject.Hostname.Value);
                        if (reply.Status == IPStatus.Success)
                        {
                            result = reply.RoundtripTime;
                        }
                        break;
                    case "tcp":
                        result = CheckPort(jObject.Hostname.Value, (int)jObject.Port.Value);
                        metadatas.Add("Hostname", jObject.Hostname.Value);
                        metadatas.Add("Port", jObject.Port.Value);
                        break;
                    case "http":
                        metadatas.Add("Address", jObject.Address.Value);
                        var client = new ExtendedWebClient();
                        try
                        {
                            var sw = Stopwatch.StartNew();
                            var response = client.DownloadString(jObject.Address.Value);
                            sw.Stop();
                            result = sw.ElapsedMilliseconds;
                            if (jObject["Regex"] != null)
                            {
                                metadatas.Add("Regex", jObject.Regex.Value);
                                var rex = new Regex(jObject.Regex.Value);
                                state = rex.IsMatch(response);
                            }
                        }
                        catch { }
                        break;
                    default:
                        PackageHost.WriteWarn("Unknow type : " + type);
                        return;
                }
                PackageHost.PushStateObject<MonitoringResult>(name, new MonitoringResult { ResponseTime = result, State = state.HasValue ? state.Value : result >= 0 }, metadatas: metadatas);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Unable to monitor '{0}' ({1}) : {2}", name, type, ex.Message);
            }
        }
    }
}

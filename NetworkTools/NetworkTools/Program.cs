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
    using System.Threading.Tasks;

    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
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
            PackageHost.WriteInfo("Pinging {0}:", host);
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(host, timeout);
            if (reply.Status == IPStatus.Success)
            {
                PackageHost.WriteInfo($"Reply from {host}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options.Ttl}");
                return reply.RoundtripTime;
            }
            else
            {
                PackageHost.WriteInfo("Requesty failed : " + reply.Status.ToString());
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
                    System.Threading.Thread.Sleep(1);
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

            //PackageHost.WriteWarn($"{host}:{port} is close");
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
    }
}

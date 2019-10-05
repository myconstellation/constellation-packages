/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016-2019 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace ZoneMinder.Interfaces
{
    using Constellation.Package;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// ZoneMinder interface for older versions
    /// </summary>
    /// <seealso cref="ZoneMinder.Interfaces.ZoneMinderBase" />
    public class ZoneMinder129 : ZoneMinderBase
    {
        /// <summary>
        /// Gets the ZoneMinder's monitors.
        /// </summary>
        /// <value>
        /// The ZoneMinder's monitors.
        /// </value>
        public override List<MonitorBase> Monitors
        {
            get
            {
                var result = new List<MonitorBase>();
                dynamic getDiskPercent = this.GetJson("api/host/getDiskPercent.json");
                dynamic datas = this.GetJson("api/monitors.json");
                if (getDiskPercent != null && datas != null)
                {
                    foreach (dynamic m in datas.monitors)
                    {
                        int id = int.Parse(m.Monitor.Id.Value);
                        dynamic status = this.GetJson("index.php?view=request&request=status&entity=monitor&id=" + id.ToString());
                        if (status != null)
                        {
                            result.Add(new Monitor()
                            {
                                Id = id,
                                Name = m.Monitor.Name.Value,
                                Type = m.Monitor.Type.Value,
                                Enabled = m.Monitor.Enabled.Value == "1",
                                Function = (MonitorFunction)Enum.Parse(typeof(MonitorFunction), m.Monitor.Function.Value),
                                Width = int.Parse(m.Monitor.Width.Value ?? "0"),
                                Height = int.Parse(m.Monitor.Height.Value ?? "0"),
                                MaxFPS = decimal.Parse(m.Monitor.MaxFPS.Value ?? "0", CultureInfo.InvariantCulture),
                                SpaceUsed = Math.Round(decimal.Parse(((dynamic)((getDiskPercent.usage as JObject)?.Property(m.Monitor.Name.Value)?.Value))?.space?.Value ?? "0", System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture)),
                                FrameRate = decimal.Parse(status.monitor.FrameRate?.Value ?? "0", CultureInfo.InvariantCulture),
                                AlarmState = status.monitor.Status == null ? AlarmState.Unknown : (AlarmState)int.Parse(status.monitor.Status.Value),
                                MinEventId = int.Parse(status.monitor.MinEventId?.Value ?? "0"),
                                MaxEventId = int.Parse(status.monitor.MaxEventId?.Value ?? "0"),
                                TotalEvents = int.Parse(status.monitor.TotalEvents?.Value ?? "0"),
                            });
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneMinder129"/> class.
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        public ZoneMinder129(string rootUri) : base(rootUri)
        {
        }

        /// <summary>
        /// Authentificate to ZoneMinder.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if access granted
        /// </returns>
        public override bool Authentificate()
        {
            try
            {
                string token = CalculateHashMD5(
                   PackageHost.GetSettingValue("SecretHash") +
                   PackageHost.GetSettingValue("Username") +
                   PackageHost.GetSettingValue("PasswordHash") +
                   DateTime.Now.Hour.ToString() +
                   DateTime.Now.Day.ToString() +
                   (DateTime.Now.Month - 1).ToString() +
                   (DateTime.Now.Year - 1900).ToString());
                string response = this.DoZMRequest($"index.php?auth={token}");
                // Test API access (thrown 401 web exception if access denied)
                this.CheckAccess(this.AuthentificationToken == null);
                // Access granted
                this.AuthentificationToken = token;
                return true;
            }
            catch (WebException ex)
            {
                PackageHost.WriteError($"Authentification failed : {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Cancels the forced alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        public override void CancelForcedAlarm(int monitorId)
        {
            this.Try(() => this.DoZMRequest("/index.php?view=request&request=alarm&id=" + monitorId.ToString() + "&command=cancelForcedAlarm"));
        }

        /// <summary>
        /// Forces the alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        public override void ForceAlarm(int monitorId)
        {
            this.Try(() => this.DoZMRequest("/index.php?view=request&request=alarm&id=" + monitorId.ToString() + "&command=forceAlarm"));
        }

        private static string CalculateHashMD5(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}

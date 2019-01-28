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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;

    /// <summary>
    /// ZoneMinder interface for version 1.32 and above
    /// </summary>
    /// <seealso cref="ZoneMinder.Interfaces.ZoneMinderBase" />
    public class ZoneMinder132 : ZoneMinderBase
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
                dynamic datas = this.GetJson("api/monitors.json");
                if (datas != null)
                {
                    foreach (dynamic m in datas.monitors)
                    {
                        int monitorId = int.Parse(m.Monitor.Id.Value);
                        dynamic alarmState = this.GetJson($"api/monitors/alarm/id:{monitorId}/command:status.json");
                        result.Add(new Monitor2()
                        {
                            Id = monitorId,
                            Name = m.Monitor.Name.Value,
                            Type = m.Monitor.Type.Value,
                            Enabled = m.Monitor.Enabled.Value == "1",
                            Function = (MonitorFunction)Enum.Parse(typeof(MonitorFunction), m.Monitor.Function.Value),
                            Width = int.Parse(m.Monitor.Width.Value),
                            Height = int.Parse(m.Monitor.Height.Value),
                            MaxFPS = decimal.Parse(m.Monitor.MaxFPS.Value, CultureInfo.InvariantCulture),
                            SpaceUsed = m.Monitor.TotalEventDiskSpace == null ? -1 : long.Parse(m.Monitor.TotalEventDiskSpace.Value),
                            State = m.Monitor_Status.Status.Value,
                            AlarmState = alarmState == null ? AlarmState.Unknown : (AlarmState)int.Parse(alarmState.status.Value),
                            FrameRate = decimal.Parse(m.Monitor_Status.CaptureFPS.Value, CultureInfo.InvariantCulture),
                            TotalEvents = int.Parse(m.Monitor.TotalEvents.Value),
                            AnalysisFPS = decimal.Parse(m.Monitor_Status.AnalysisFPS.Value, CultureInfo.InvariantCulture),
                            CaptureBandwidth = long.Parse(m.Monitor_Status.CaptureBandwidth.Value),
                            ArchivedEvents = int.Parse(m.Monitor.ArchivedEvents.Value),
                            CaptureMethod = m.Monitor.Method.Value,
                            CaptureOptions = m.Monitor.Options.Value,
                            CapturePath = m.Monitor.Path.Value,
                            Controllable = m.Monitor.Controllable.Value == "1",
                            DayEvents = int.Parse(m.Monitor.DayEvents.Value),
                            HourEvents = int.Parse(m.Monitor.HourEvents.Value),
                            MonthEvents = int.Parse(m.Monitor.MonthEvents.Value),
                            WeekEvents = int.Parse(m.Monitor.WeekEvents.Value),
                            ZoneCount = int.Parse(m.Monitor.ZoneCount.Value),
                            ServerID = int.Parse(m.Monitor.ServerId.Value),
                            StorageID = int.Parse(m.Monitor.StorageId.Value)
                        });
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneMinder132"/> class.
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        public ZoneMinder132(string rootUri) : base(rootUri)
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
                string strLoginResponse = this.DoZMRequest("api/host/login.json", postdata: $"user={PackageHost.GetSettingValue("Username")}&pass={PackageHost.GetSettingValue("Password")}", method: WebRequestMethods.Http.Post);
                dynamic loginResponse = JsonConvert.DeserializeObject(strLoginResponse) as dynamic;
                if (loginResponse.credentials != null)
                {
                    this.CheckAccess(this.AuthentificationToken == null);
                    this.AuthentificationToken = ((string)loginResponse.credentials).Substring("auth=".Length);
                    return true;
                }
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
            this.Try(() => this.DoZMRequest($"api/monitors/alarm/id:{monitorId}/command:off.json"));
        }

        /// <summary>
        /// Forces the alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        public override void ForceAlarm(int monitorId)
        {
            this.Try(() => this.DoZMRequest($"api/monitors/alarm/id:{monitorId}/command:on.json"));
        }
    }
}

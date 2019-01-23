/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace ZoneMinder
{
    using Constellation.Package;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program : PackageBase
    {
        private CookieContainer cookieContainer = new CookieContainer();
        private int systemRefreshInterval, monitorsRefreshInterval, eventsRefreshInterval;

        private string rootUri = string.Empty;
        private string authentificationHash = string.Empty;
        private DateTime authentificationDate = DateTime.MinValue;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            this.rootUri = PackageHost.GetSettingValue("RootUri");
            this.systemRefreshInterval = PackageHost.GetSettingValue<int>("SystemRefreshInterval");
            this.monitorsRefreshInterval = PackageHost.GetSettingValue<int>("MonitorsRefreshInterval");
            this.eventsRefreshInterval = PackageHost.GetSettingValue<int>("EventsRefreshInterval");

            try
            {
                this.Authentificate();
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Unable to connect to ZoneMinder on '{0}' : {1}", this.rootUri, ex.Message);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        this.Authentificate();

                        dynamic getDiskPercent = this.GetJson("api/host/getDiskPercent.json");
                        dynamic getVersion = getDiskPercent == null ? null : this.GetJson("api/host/getVersion.json");
                        dynamic daemonCheck = getVersion == null ? null : this.GetJson("api/host/daemonCheck.json");
                        dynamic getLoad = daemonCheck == null ? null : this.GetJson("api/host/getLoad.json");
                        if (getLoad != null)
                        {
                            PackageHost.PushStateObject<Host>("Host", new Host()
                            {
                                URI = this.rootUri,
                                Version = getVersion.version.Value,
                                APIVersion = getVersion.apiversion.Value,
                                DaemonStarted = daemonCheck.result == "1",
                                LoadAverageLastMinute = (double)getLoad.load[0].Value,
                                LoadAverageLastFiveMinutes = (double)getLoad.load[1].Value,
                                LoadAverageLastFifteenMinutes = (double)getLoad.load[2].Value,
                                SpaceUsed = Math.Round(decimal.Parse(getDiskPercent.usage.Total.space.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture))
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error to query ZoneMinder : {0}", ex.ToString());
                    }

                    Thread.Sleep(this.systemRefreshInterval);
                }
            });

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        dynamic getDiskPercent = this.GetJson("api/host/getDiskPercent.json");
                        dynamic datas = getDiskPercent == null ? null : this.GetJson("api/monitors.json");
                        if (datas != null)
                        {
                            foreach (dynamic m in datas.monitors)
                            {
                                int id = int.Parse(m.Monitor.Id.Value);
                                dynamic status = this.GetJson("index.php?view=request&request=status&entity=monitor&id=" + id.ToString());
                                if (status != null)
                                {
                                    PackageHost.PushStateObject<Monitor>(m.Monitor.Name.Value, new Monitor()
                                    {
                                        Id = id,
                                        Name = m.Monitor.Name.Value,
                                        Type = m.Monitor.Type.Value,
                                        Enabled = m.Monitor.Enabled.Value == "1",
                                        Function = (MonitorFunction)Enum.Parse(typeof(MonitorFunction), m.Monitor.Function.Value),
                                        StreamPath = this.GetStreamPath(id),
                                        Width = int.Parse(m.Monitor.Width.Value),
                                        Height = int.Parse(m.Monitor.Height.Value),
                                        MaxFPS = decimal.Parse(m.Monitor.MaxFPS.Value, CultureInfo.InvariantCulture),
                                        SpaceUsed = Math.Round(decimal.Parse(((dynamic)((getDiskPercent.usage as JObject).Property(m.Monitor.Name.Value).Value)).space.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture)),
                                        State = (AlarmState)int.Parse(status.monitor.Status.Value),
                                        FrameRate = decimal.Parse(status.monitor.FrameRate.Value, CultureInfo.InvariantCulture),
                                        MinEventId = status.monitor.MinEventId.Value != null ? int.Parse(status.monitor.MinEventId.Value) : -1,
                                        MaxEventId = status.monitor.MaxEventId.Value != null ? int.Parse(status.monitor.MaxEventId.Value) : -1,
                                        TotalEvents = int.Parse(status.monitor.TotalEvents.Value),
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error to query ZoneMinder's monitor : {0}", ex.ToString());
                    }

                    Thread.Sleep(this.systemRefreshInterval);
                }
            });

            Task.Factory.StartNew(() =>
            {
                int lastEventId = -1;
                int lastPage = 1;
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        if (lastEventId < 0)
                        {
                            dynamic events = this.GetJson("api/events.json?page=" + lastPage.ToString());
                            if ((int)events.pagination.pageCount > lastPage)
                            {
                                lastPage = (int)events.pagination.pageCount;
                                events = this.GetJson("api/events.json?page=" + ((int)events.pagination.pageCount).ToString());
                            }
                            var eventLists = events.events as JArray;
                            lastEventId = (eventLists.Count == 0) ? 0 : int.Parse(((dynamic)eventLists.Last).Event.Id.Value);
                            PackageHost.WriteInfo("Last event found #{0}", lastEventId);
                        }
                        else
                        {
                            dynamic events = null;
                            try
                            {
                                events = this.GetJson("api/events.json?page=" + lastPage.ToString(), rethow: true);
                            }
                            catch (WebException ex)
                            {
                                if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                                {
                                    lastEventId = -1;
                                    lastPage = 1;
                                    continue;
                                }
                            }
                            catch { }
                            for (int i = lastPage; i <= (int)events.pagination.pageCount; i++)
                            {
                                if (i > lastPage)
                                {
                                    events = this.GetJson("api/events.json?page=" + lastPage.ToString());
                                    lastPage = i;
                                }
                                foreach (var e in events.events)
                                {
                                    var id = int.Parse(e.Event.Id.Value);
                                    if (id > lastEventId)
                                    {
                                        lastEventId = id;
                                        PackageHost.WriteWarn("New event #{0} detected ({1}) on monitor #{2} (Note: {3})", id, e.Event.Cause.Value, e.Event.MonitorId.Value, e.Event.Notes.Value);
                                        PackageHost.CreateMessageProxy(Constellation.MessageScope.ScopeType.Group, "Motion").Alarm(new
                                        {
                                            Id = id,
                                            Name = e.Event.Name.Value,
                                            MonitorId = int.Parse(e.Event.MonitorId.Value),
                                            Cause = e.Event.Cause.Value,
                                            StartTime = DateTime.ParseExact((string)e.Event.StartTime.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-us")),
                                            EndTime = string.IsNullOrEmpty((string)e.Event.EndTime.Value) ? DateTime.MinValue : DateTime.ParseExact((string)e.Event.EndTime.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-us")),
                                            Length = decimal.Parse(e.Event.Length.Value, CultureInfo.InvariantCulture),
                                            Notes = e.Event.Notes.Value,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error to query ZoneMinder's events : {0}", ex.ToString());
                    }

                    Thread.Sleep(this.eventsRefreshInterval);
                }
            });

            PackageHost.WriteInfo("ZoneMinder '{0}' started", this.rootUri);
        }

        /// <summary>
        /// Changes the state of ZoneMinder.
        /// </summary>
        /// <param name="state">The new state.</param>
        [MessageCallback]
        public void ChangeState(string state)
        {
            this.Try(() => this.PostHttp("api/states/change/" + state + ".json"));
        }

        /// <summary>
        /// Restarts ZoneMinder.
        /// </summary>
        [MessageCallback]
        public void Restart()
        {
            this.ChangeState("restart");
        }

        /// <summary>
        /// Forces the alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        [MessageCallback]
        public void ForceAlarm(int monitorId)
        {
            this.Try(() => this.GetHttp("/index.php?view=request&request=alarm&id=" + monitorId.ToString() + "&command=forceAlarm"));
        }

        /// <summary>
        /// Cancels the forced alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        [MessageCallback]
        public void CancelForcedAlarm(int monitorId)
        {
            this.Try(() => this.GetHttp("/index.php?view=request&request=alarm&id=" + monitorId.ToString() + "&command=cancelForcedAlarm"));
        }

        /// <summary>
        /// Sets the monitor function.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        /// <param name="function">The function.</param>
        [MessageCallback]
        public void SetMonitorFunction(int monitorId, MonitorFunction function)
        {
            this.Try(() => this.PostHttp("api/monitors/" + monitorId.ToString() + ".json", "Monitor[Function]=" + function.ToString()));
        }

        private dynamic GetJson(string path, int tries = 3, bool rethow = false)
        {
            dynamic result = null;
            try
            {
                this.Try(() => { result = JsonConvert.DeserializeObject(Utils.StripTagsCharArray(GetHttp(path)).Trim()); });
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Unable to get response : '{0}' : {1}", path, ex.Message);
                if (rethow)
                {
                    throw ex;
                }
            }
            return result;
        }

        private void Try(Action action, int tries = 3)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (--tries == 0)
                {
                    throw ex;
                }
                else
                {
                    this.Try(action, tries);
                }
            }
        }

        private void Authentificate()
        {
            if (DateTime.Now.Subtract(this.authentificationDate).TotalMinutes > 90)
            {
                this.GetHttp("index.php?auth=" + this.GenerateHash());
            }
        }

        private string GetStreamPath(int monitor, int scale = 0, int maxfps = 0)
        {
            return string.Format("/cgi-bin/nph-zms?mode=jpeg&monitor={0}&auth={1}&rand={2}{3}{4}", monitor, this.GenerateHash(), DateTime.Now.Ticks, scale > 0 ? scale.ToString() : "", maxfps > 0 ? maxfps.ToString() : "");
        }

        private string GenerateHash()
        {
            this.authentificationDate = DateTime.Now;
            return Utils.CalculateMD5Hash(
                PackageHost.GetSettingValue<string>("SecretHash") +
                PackageHost.GetSettingValue<string>("Username") +
                PackageHost.GetSettingValue<string>("PasswordHash") +
                this.authentificationDate.Hour.ToString() +
                this.authentificationDate.Day.ToString() +
                (this.authentificationDate.Month - 1).ToString() +
                (this.authentificationDate.Year - 1900).ToString());
        }

        private string GetHttp(string path)
        {
            return Utils.DoRequest(Utils.FormatUri(this.rootUri, path), cookieContainer: this.cookieContainer);
        }

        private string PostHttp(string path, string postdata = "")
        {
            return Utils.DoRequest(Utils.FormatUri(this.rootUri, path), "POST", postdata, this.cookieContainer);
        }
    }
}

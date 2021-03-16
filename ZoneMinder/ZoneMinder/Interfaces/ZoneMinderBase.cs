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
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Represents a ZoneMinder base interface
    /// </summary>
    public abstract class ZoneMinderBase
    {
        private const int RENEW_TOKEN_INTERVAL_IN_MINUTES = 10;

        private string authentificationToken = null;
        private DateTime authentificationTokenDate = DateTime.MinValue;
        private CookieContainer cookieContainer = new CookieContainer();

        private int lastKnownEventId = -1;
        private int lastKnownEventPageId = 1;
        private Dictionary<int, int> eventsInProgress = new Dictionary<int, int>();

        /// <summary>
        /// Occurs when a new event is started.
        /// </summary>
        public event EventHandler<ZoneMinderEvent> EventStarted;

        /// <summary>
        /// Occurs when a new event record is terminated.
        /// </summary>
        public event EventHandler<ZoneMinderEvent> EventTerminated;

        /// <summary>
        /// Gets the ZoneMinder's monitors.
        /// </summary>
        /// <value>
        /// The ZoneMinder's monitors.
        /// </value>
        public abstract List<MonitorBase> Monitors { get; }

        /// <summary>
        /// Gets the ZoneMinder's host description.
        /// </summary>
        /// <value>
        /// The ZoneMinder's host description.
        /// </value>
        public Host Host
        {
            get
            {
                dynamic getDiskPercent = this.GetJson("api/host/getDiskPercent.json");
                dynamic getVersion = this.GetJson("api/host/getVersion.json");
                dynamic daemonCheck = this.GetJson("api/host/daemonCheck.json");
                dynamic getLoad = this.GetJson("api/host/getLoad.json");
                if (getDiskPercent != null && getVersion != null && daemonCheck != null && getLoad != null)
                {
                    return new Host()
                    {
                        URI = this.RootUri,
                        AuthentificationToken = this.AuthentificationToken,
                        Version = getVersion.version.Value,
                        APIVersion = getVersion.apiversion.Value,
                        DaemonStarted = daemonCheck.result == "1",
                        LoadAverageLastMinute = (double)getLoad.load[0].Value,
                        LoadAverageLastFiveMinutes = (double)getLoad.load[1].Value,
                        LoadAverageLastFifteenMinutes = (double)getLoad.load[2].Value,
                        SpaceUsed = Math.Round(decimal.Parse(getDiskPercent.usage.Total.space.Value, NumberStyles.Float, CultureInfo.InvariantCulture))
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the root URI.
        /// </summary>
        /// <value>
        /// The root URI.
        /// </value>
        public string RootUri { get; private set; }

        /// <summary>
        /// Gets the authentification token.
        /// </summary>
        /// <value>
        /// The authentification token.
        /// </value>
        public string AuthentificationToken
        {
            get { return this.authentificationToken; }
            set
            {
                this.authentificationTokenDate = DateTime.Now;
                this.authentificationToken = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneMinderBase"/> class.
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        public ZoneMinderBase(string rootUri)
        {
            this.RootUri = rootUri;
        }

        /// <summary>
        /// Checks the access to ZoneMinder.
        /// </summary>
        /// <param name="verbose"><c>true</c> to write log.</param>
        public void CheckAccess(bool verbose = true)
        {
            var version = JsonConvert.DeserializeObject(this.DoZMRequest("api/host/getVersion.json")) as dynamic;
            if (version != null && verbose)
            {
                PackageHost.WriteInfo($"API access done. ZoneMinder {version.version} API {version.apiversion}");
            }
        }

        /// <summary>
        /// Renews the Authentification token.
        /// </summary>
        public void RenewToken()
        {
            if (DateTime.Now.Subtract(this.authentificationTokenDate).TotalMinutes >= RENEW_TOKEN_INTERVAL_IN_MINUTES)
            {
                this.Authentificate();
            }
        }

        /// <summary>
        /// Clears the Authentification token.
        /// </summary>
        public void ClearToken()
        {
            this.authentificationTokenDate = DateTime.MinValue;
            this.authentificationToken = null;
        }

        /// <summary>
        /// Authentificate to ZoneMinder.
        /// </summary>
        /// <returns><c>true</c> if access granted</returns>
        public abstract bool Authentificate();

        /// <summary>
        /// Cancels the forced alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        public abstract void CancelForcedAlarm(int monitorId);

        /// <summary>
        /// Forces the alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        public abstract void ForceAlarm(int monitorId);

        /// <summary>
        /// Changes the state of ZoneMinder.
        /// </summary>
        /// <param name="state">The new state.</param>
        public virtual void ChangeState(State state)
        {
            this.Try(() => this.DoZMRequest($"api/states/change/{state.ToString().ToLower()}.json", method: WebRequestMethods.Http.Post));
        }

        /// <summary>
        /// Sets the monitor function.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        /// <param name="function">The function.</param>
        /// <param name="enabled"><c>true</c> to enabled, otherwise the monitor is disabled.</param>
        public void SetMonitorFunction(int monitorId, MonitorFunction function, bool enabled = true)
        {
            this.Try(() => this.DoZMRequest($"api/monitors/{monitorId}.json", postdata: $"Monitor[Function]={function}&Monitor[Enabled]={(enabled ? "1" : "0")}", method: WebRequestMethods.Http.Post));
        }

        /// <summary>
        /// Sets the monitor property.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void SetMonitorProperty(int monitorId, string property, string value)
        {
            this.Try(() => this.DoZMRequest($"api/monitors/{monitorId}.json", postdata: $"Monitor[{property}]={value}", method: WebRequestMethods.Http.Put));
        }

        /// <summary>
        /// Generates the streaming URI for live or pre-recorded streams.
        /// </summary>
        /// <param name="id">The identifier (monitorId if source=live or eventId if source=event).</param>
        /// <param name="source">The streaming source (live or event).</param>
        /// <param name="mode">The streaming mode (video or snapshot).</param>
        /// <param name="options">The streaming options.</param>
        /// <returns>The URI to the specified stream</returns>
        public string GenerateStreamingURI(int id, StreamingOptions.Source source = StreamingOptions.Source.Live, StreamingOptions.Mode mode = StreamingOptions.Mode.MJPEG, StreamingOptions options = null)
        {
            var @params = new Dictionary<string, object>();
            // Set the mode (Snapshot or Video)
            switch (mode)
            {
                case StreamingOptions.Mode.JPEG: // Snapshot = single
                    @params["mode"] = "single";
                    break;
                case StreamingOptions.Mode.MJPEG: // Video = jpeg
                    @params["mode"] = "jpeg";
                    break;
            }
            // Set the source (live or event)
            switch (source)
            {
                case StreamingOptions.Source.Live:
                    @params["monitor"] = id;
                    break;
                case StreamingOptions.Source.Event:
                    @params["source"] = "event";
                    @params["replay"] = "none";
                    @params["event"] = id;
                    break;
            }
            // Set the streaming options
            if (options?.Scale > 0)
            {
                @params["scale"] = options.Scale;
            }
            if (options?.Width > 0)
            {
                @params["width"] = options.Width + "px";
            }
            if (options?.Height > 0)
            {
                @params["height"] = options.Height + "px";
            }
            if (options?.Buffer > 0)
            {
                @params["buffer"] = options.Buffer;
            }
            if (options?.MaxFPS > 0)
            {
                @params["maxfps"] = options.MaxFPS;
            }
            if (!string.IsNullOrEmpty(options?.ConnKey))
            {
                @params["connkey"] = options.ConnKey;
            }
            if (options?.IncludeAuthentificationToken ?? true)
            {
                @params["auth"] = this.AuthentificationToken;
            }
            return ((options?.IncludeRootURI ?? false) ? this.RootUri.TrimEnd('/') : "") + "/cgi-bin/nph-zms?" + string.Join("&", @params.Select(v => $"{v.Key}={v.Value}").ToArray());
        }

        /// <summary>
        /// Searches events with multiple criterias.
        /// </summary>
        /// <param name="searchCriterias">The search criterias.</param>
        /// <returns>List of events</returns>
        public List<Event> SearchEvents(params EventSearchCriterias[] searchCriterias)
        {
            var result = new List<Event>();
            if (searchCriterias.Length == 1 && searchCriterias[0].Field == EventSearchCriterias.Criteria.EventId && searchCriterias[0].Condition == EventSearchCriterias.ConditionOperator.Equal)
            {
                result.Add(Event.CreateFromJSON(this.GetJson("api/events/" + searchCriterias[0].Value + ".json").@event));
            }
            else
            {
                foreach (dynamic zmEvent in this.GetJson("api/events/index/" + string.Join("/", searchCriterias.Select(c => c.ToString()).ToArray()) + ".json").@events)
                {
                    result.Add(Event.CreateFromJSON(zmEvent));
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the last event ID.
        /// </summary>
        /// <returns>ID of the last event</returns>
        public int FindLastEventId()
        {
            dynamic events = this.GetJson("api/events.json?page=" + lastKnownEventPageId.ToString());
            if (events == null || events.events == null || events.pagination == null)
            {
                return -1;
            }
            if ((int)events.pagination.pageCount > lastKnownEventPageId)
            {
                lastKnownEventPageId = (int)events.pagination.pageCount;
                events = this.GetJson("api/events.json?page=" + ((int)events.pagination.pageCount).ToString());
            }
            var eventLists = events.events as JArray;
            this.lastKnownEventId = (eventLists.Count == 0) ? 0 : int.Parse(((dynamic)eventLists.Last).Event.Id.Value);
            PackageHost.WriteInfo("Last event found #{0}", lastKnownEventId);
            return lastKnownEventId;
        }

        /// <summary>
        /// Pools the events API to get the last events and raise event.
        /// </summary>
        public void PoolEvents()
        {
            if (this.lastKnownEventId < 0)
            {
                this.FindLastEventId();
            }
            else
            {
                dynamic events = null;
                try
                {
                    events = this.GetJson($"api/events.json?page={this.lastKnownEventPageId}", rethow: true);
                    if (events == null || events.events == null || events.pagination == null)
                    {
                        return;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        this.lastKnownEventId = -1;
                        this.lastKnownEventPageId = 1;
                    }
                    else
                    {
                        PackageHost.WriteError($"Invalid HTTP response ({ex.Status}) from API for the event page #{this.lastKnownEventPageId} : {ex.Message}");
                    }
                    return;
                }
                for (int i = this.lastKnownEventPageId; i <= (int)events.pagination.pageCount; i++)
                {
                    if (i > this.lastKnownEventPageId)
                    {
                        events = this.GetJson($"api/events.json?page={this.lastKnownEventPageId}");
                        if (events == null || events.events == null || events.pagination == null)
                        {
                            return;
                        }
                        // Set the last page only if there is no events in progress for the current page
                        if (this.eventsInProgress.Count(e => e.Value == this.lastKnownEventPageId) == 0)
                        {
                            this.lastKnownEventPageId = i;
                        }
                    }
                    foreach (var zmEvent in events.events)
                    {
                        var arg = new ZoneMinderEvent { Event = Event.CreateFromJSON(zmEvent) };
                        if (arg.Event.EventId > this.lastKnownEventId) // New event
                        {
                            if (!arg.Event.Terminated) // Recording in progress
                            {
                                this.eventsInProgress.Add(arg.Event.EventId, i);
                                this.EventStarted?.Invoke(this, arg);
                            }
                            else // Event already terminated
                            {
                                this.EventTerminated?.Invoke(this, arg);
                            }
                            this.lastKnownEventId = arg.Event.EventId;
                        }
                        else if (this.eventsInProgress.ContainsKey(arg.Event.EventId) && arg.Event.Terminated) // Event terminated
                        {
                            this.eventsInProgress.Remove(arg.Event.EventId);
                            // Get all data about this event
                            var eventDatas = this.GetJson("api/events/" + arg.Event.EventId + ".json")?.@event;
                            this.EventTerminated?.Invoke(this, eventDatas != null ? new ZoneMinderEvent { Event = Event.CreateFromJSON(eventDatas) } : arg);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Edits the event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="propertiesUpdated">The properties (key/value) updated.</param>
        public void EditEvent(int eventId, Dictionary<string, string> propertiesUpdated)
        {
            this.DoZMRequest($"api/events/{eventId}.json",
                method: WebRequestMethods.Http.Put,
                postdata: string.Join("&", propertiesUpdated.Select(p => $"Event[{p.Key}]={p.Value}").ToArray()));
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        public void DeleteEvent(int eventId)
        {
            this.DoZMRequest($"api/events/{eventId}.json", method: "DELETE");
        }

        /// <summary>
        /// Gets JSON response.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="tries">The tries.</param>
        /// <param name="rethow">if set to <c>true</c> to rethow exception.</param>
        /// <returns></returns>
        protected dynamic GetJson(string path, int tries = 3, bool rethow = false)
        {
            dynamic result = null;
            try
            {
                this.Try(() =>
                {
                    string apiResponse = StripTagsCharArray(DoZMRequest(path)).Trim();
                    result = JsonConvert.DeserializeObject(apiResponse);
                    if (result == null)
                    {
                        PackageHost.WriteError($"Invalid JSON response from API for the request '{path}'. The content was : {apiResponse}");
                    }
                });
            }
            catch (WebException ex)
            {
                if (rethow)
                {
                    throw ex;
                }
                else
                {
                    PackageHost.WriteError($"Invalid HTTP response ({ex.Status}) from API for the request '{path}' : {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                if (rethow)
                {
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// Does the HTTP request to ZM.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="method">The method.</param>
        /// <param name="postdata">The postdata.</param>
        /// <returns>HTTP response</returns>
        protected string DoZMRequest(string path, string method = "", string postdata = "")
        {
            return DoHttpRequest(FormatUri(this.RootUri, path), method: method, postdata: postdata, cookieContainer: this.cookieContainer);
        }

        /// <summary>
        /// Tries the specified action and ensure to renew the authentification token.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="tries">The tries.</param>
        protected void Try(Action action, int tries = 3)
        {
            try
            {
                this.RenewToken();
                action();
            }
            catch (Exception ex)
            {
                if (ex is WebException webException && webException.Status == WebExceptionStatus.ProtocolError && webException.Response is HttpWebResponse webResponse && webResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    this.ClearToken();
                }

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

        private static string FormatUri(string rootUri, string path)
        {
            return $"{rootUri.TrimEnd('/')}/{path.TrimEnd('/')}";
        }

        private static string DoHttpRequest(string url, string method = "", string postdata = "", CookieContainer cookieContainer = null)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = cookieContainer;
            if (!string.IsNullOrEmpty(method))
            {
                webRequest.Method = method;
            }

            if (!string.IsNullOrEmpty(postdata))
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";
                byte[] data = Encoding.ASCII.GetBytes(postdata);
                webRequest.ContentLength = data.Length;
                using (var newStream = webRequest.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
            }

            using (var response = webRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream))
            {
                return responseReader.ReadToEnd();
            }
        }

        private static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// Represent the class that contains ZoneMinder's event.
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class ZoneMinderEvent : EventArgs
        {
            /// <summary>
            /// Gets or sets the event.
            /// </summary>
            /// <value>
            /// The event.
            /// </value>
            public Event Event { get; set; }
        }
    }
}

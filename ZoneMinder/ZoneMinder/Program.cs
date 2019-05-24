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

namespace ZoneMinder
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ZoneMinder.Interfaces;

    /// <summary>
    /// ZoneMinder package for Constellation
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private ZoneMinderBase zoneMinder = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Create the ZM interface
            if (PackageHost.GetSettingValue<bool>("UseLoginAPI"))
            {
                this.zoneMinder = new ZoneMinder132(PackageHost.GetSettingValue("RootUri"));
            }
            else
            {
                this.zoneMinder = new ZoneMinder129(PackageHost.GetSettingValue("RootUri"));
            }
            PackageHost.WriteInfo($"Using {this.zoneMinder.GetType().Name} interface to connect to {this.zoneMinder.RootUri}");

            // Authentification
            if (!this.zoneMinder.Authentificate())
            {
                throw new Exception($"Unable to authentificate !");
            }

            // When ZoneMinder event changed
            this.zoneMinder.EventStarted += ZoneMinder_OnEventChanged;
            this.zoneMinder.EventTerminated += ZoneMinder_OnEventChanged;

            // Query ZM host task
            this.AddBackgroundTask(
                () => PackageHost.PushStateObject<Host>("Host", this.zoneMinder.Host, lifetime: (PackageHost.GetSettingValue<int>("SystemRefreshInterval") / 500) + 5),
                "SystemRefreshInterval",
                (ex) => PackageHost.WriteError("Error to query ZoneMinder : {0}", ex.ToString()));

            // Query ZM monitors task
            this.AddBackgroundTask(
                () =>
                {
                    foreach (var monitor in this.zoneMinder.Monitors)
                    {
                        PackageHost.PushStateObject(monitor.Name, monitor, lifetime: (PackageHost.GetSettingValue<int>("MonitorsRefreshInterval") / 500) + 5);
                    }
                },
                "MonitorsRefreshInterval",
                (ex) => PackageHost.WriteError("Error to query ZoneMinder's monitors : {0}", ex.ToString()));

            // Query ZM events task
            this.AddBackgroundTask(
                () => this.zoneMinder.PoolEvents(),
                "EventsRefreshInterval",
                (ex) => PackageHost.WriteError("Error to query ZoneMinder's events : {0}", ex.ToString()));

            // Package started !
            PackageHost.WriteInfo("Connected to ZoneMinder '{0}'", this.zoneMinder.RootUri);
        }

        /// <summary>
        /// Changes the state of ZoneMinder.
        /// </summary>
        /// <param name="state">The new state.</param>
        [MessageCallback]
        public void ChangeState(State state)
        {
            PackageHost.WriteInfo($"Setting state to {state} ...");
            this.zoneMinder.ChangeState(state);
        }

        /// <summary>
        /// Restarts ZoneMinder.
        /// </summary>
        [MessageCallback]
        public void Restart()
        {
            PackageHost.WriteWarn($"Restarting ZoneMinder host ...");
            this.ChangeState(State.Restart);
        }

        /// <summary>
        /// Forces the alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        [MessageCallback]
        public void ForceAlarm(int monitorId)
        {
            PackageHost.WriteInfo($"Forcing alarm for monitor #{monitorId}");
            this.zoneMinder.ForceAlarm(monitorId);
        }

        /// <summary>
        /// Cancels the forced alarm.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        [MessageCallback]
        public void CancelForcedAlarm(int monitorId)
        {
            PackageHost.WriteInfo($"Cancelling alarm for monitor #{monitorId}");
            this.zoneMinder.CancelForcedAlarm(monitorId);
        }

        /// <summary>
        /// Sets the monitor function.
        /// </summary>
        /// <param name="monitorId">The monitor identifier.</param>
        /// <param name="function">The function.</param>
        /// <param name="enabled"><c>true</c> to enabled, otherwise the monitor is disabled.</param>
        [MessageCallback]
        public void SetMonitorFunction(int monitorId, MonitorFunction function, bool enabled = true)
        {
            PackageHost.WriteInfo($"Setting monitor #{monitorId} to {function} (enabled:{enabled})");
            this.zoneMinder.SetMonitorFunction(monitorId, function, enabled);
        }

        /// <summary>
        /// Generates the streaming URI.
        /// </summary>
        /// <param name="id">The identifier (monitorId if source=live or eventId if source=event).</param>
        /// <param name="source">The streaming source (live or event).</param>
        /// <param name="mode">The streaming mode (video or snapshot).</param>
        /// <param name="options">The streaming options.</param>
        /// <returns>The URI to the specified stream</returns>
        [MessageCallback]
        public string GenerateStreamingURI(int id, StreamingOptions.Source source = StreamingOptions.Source.Live, StreamingOptions.Mode mode = StreamingOptions.Mode.MJPEG, StreamingOptions options = null)
        {
            return this.zoneMinder.GenerateStreamingURI(id, source, mode, options);
        }

        /// <summary>
        /// Searches events with one criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [MessageCallback]
        public List<Event> SearchEvents(EventSearchCriterias criteria)
        {
            return this.zoneMinder.SearchEvents(criteria);
        }

        /// <summary>
        /// Searches events with criterias.
        /// </summary>
        /// <param name="criterias">The array of criterias.</param>
        [MessageCallback]
        public List<Event> SearchEventsWithMultiCriterias(EventSearchCriterias[] criterias)
        {
            return this.zoneMinder.SearchEvents(criterias);
        }

        /// <summary>
        /// Gets event by ID.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        [MessageCallback]
        public Event GetEvent(int eventId)
        {
            return this.zoneMinder.SearchEvents(new EventSearchCriterias()
            {
                Condition = EventSearchCriterias.ConditionOperator.Equal,
                Field = EventSearchCriterias.Criteria.EventId,
                Value = eventId.ToString()
            })?.FirstOrDefault();
        }

        /// <summary>
        /// Updates the event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="request">The update request.</param>
        [MessageCallback]
        public void UpdateEvent(int eventId, UpdateEventRequest request)
        {
            var props = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(request.Name))
            {
                props[nameof(request.Name)] = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Cause))
            {
                props[nameof(request.Cause)] = request.Cause;
            }
            if (!string.IsNullOrEmpty(request.Notes))
            {
                props[nameof(request.Notes)] = request.Notes;
            }
            if (props.Count > 0)
            {
                PackageHost.WriteInfo($"Updating {string.Join(",", props.Keys)} for event #{eventId}");
                this.zoneMinder.EditEvent(eventId, props);
            }
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        [MessageCallback]
        public void DeleteEvent(int eventId)
        {
            PackageHost.WriteWarn($"Deleteing the event #{eventId}");
            this.zoneMinder.DeleteEvent(eventId);
        }

        private void ZoneMinder_OnEventChanged(object sender, ZoneMinderBase.ZoneMinderEvent e)
        {
            if (!e.Event.Terminated)
            {
                PackageHost.WriteWarn($"New event #{e.Event.EventId} ({e.Event.Cause}) on monitor #{e.Event.MonitorId} detected ({e.Event.Notes})");
            }
            else
            {
                PackageHost.WriteInfo($"Event #{e.Event.EventId} ({e.Event.Cause}) on monitor #{e.Event.MonitorId} is terminated (Lenght:{e.Event.Length}sec - {e.Event.Notes})");
            }
            // Forward ZM event to Constellation group
            if (PackageHost.GetSettingValue<bool>("ForwardEvents"))
            {
                PackageHost.CreateMessageProxy(Constellation.MessageScope.ScopeType.Group, PackageHost.GetSettingValue("EventsGroupName")).OnZoneMinderEvent(e.Event);
            }
        }

        private void AddBackgroundTask(Action action, string intervalSettingName, Action<Exception> onError = null)
        {
            Task.Factory.StartNew(() =>
            {
                var lastExecution = DateTime.MinValue;
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        if (DateTime.Now.Subtract(lastExecution).TotalMilliseconds >= PackageHost.GetSettingValue<int>(intervalSettingName))
                        {
                            action();
                            lastExecution = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke(ex);
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }
}

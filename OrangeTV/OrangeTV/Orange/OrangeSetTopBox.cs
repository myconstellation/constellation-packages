/*
 *	 OrangeTV Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2017 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace OrangeTV
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the Orange's set-top box (STB)
    /// </summary>
    public class OrangeSetTopBox
    {
        /// <summary>
        /// Service operation availables on the Orange STB
        /// </summary>
        private enum ServiceOperation
        {
            /// <summary>
            /// Send key
            /// </summary>
            SendKey = 1,
            /// <summary>
            /// Set the touchpad positon
            /// </summary>
            SetTouchpadPositon = 2,
            /// <summary>
            /// Switch to EPG or Channel
            /// </summary>
            SwitchTo = 9,
            /// <summary>
            /// Get the current state
            /// </summary>
            GetState = 10
        }

        /// <summary>
        /// The event notify cancellation token
        /// </summary>
        private CancellationTokenSource eventNotifyCancellationToken = null;

        /// <summary>
        /// The query state cancellation token
        /// </summary>
        private CancellationTokenSource queryStateCancellationToken = null;

        /// <summary>
        /// The automatic raise event
        /// </summary>
        private AutoResetEvent autoRaiseEvent = new AutoResetEvent(true);

        /// <summary>
        /// Occurs when an event notification is received
        /// </summary>
        public event EventHandler<EventNotificationEventArgs> EventNotificationReceived;

        /// <summary>
        /// Occurs when the current state is updated.
        /// </summary>
        public event EventHandler StateUpdated;

        /// <summary>
        /// Gets the STB URI.
        /// </summary>
        /// <value>
        /// The STB URI.
        /// </value>
        public Uri RootUri { get; private set; }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrangeSetTopBox"/> class.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="port">The port (8080 by default).</param>
        public OrangeSetTopBox(string hostname, int port = 8080)
        {
            this.RootUri = new Uri($"http://{hostname}:{port}");
        }

        /// <summary>
        /// Starts the listening for event notification.
        /// </summary>
        public void StartListening(Action<Exception> onError = null, bool continueOnError = true)
        {
            if (this.eventNotifyCancellationToken == null)
            {
                this.eventNotifyCancellationToken = new CancellationTokenSource();
                // Start the query state loop if Time Shifting
                if (this.CurrentState.TimeShiftingState)
                {
                    StartQueryStateLoop();
                }
                // Event notify loop
                Task.Factory.StartNew(async () =>
                {
                    while (!this.eventNotifyCancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            // Query URL
                            var strResponse = await this.GetWebResponseAsync(new Uri(this.RootUri, "/remoteControl/notifyEvent"));
                            if (!string.IsNullOrEmpty(strResponse))
                            {
                                // Read the JSON response
                                var response = JsonConvert.DeserializeObject<BaseResponse<EventNotification>>(strResponse, OrangeContractResolver.Settings).Result;
                                if (response.Code == ResponseCode.EventNotification)
                                {
                                    // Find the custom notification
                                    var eventType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.GetCustomAttribute<EventNotificationAttribute>()?.EventType == response.Data.EventType);
                                    if (eventType != null)
                                    {
                                        response.Data = (EventNotification)JsonConvert.DeserializeObject(JObject.Parse(strResponse).SelectToken("result.data").ToString(), eventType, OrangeContractResolver.Settings);
                                    }
                                    // Update the current state with the event notification
                                    response.Data.UpdateState(this.CurrentState);
                                    this.StateUpdated?.Invoke(this, EventArgs.Empty);
                                    // Start the loop to query the state in timeshifting mode
                                    if (response.Data is TimeShiftingChanged)
                                    {
                                        this.StartQueryStateLoop();
                                    }
                                    // Query the state if the context changed
                                    if (response.Data is ContextChanged)
                                    {
                                        await this.GetCurrentState();
                                    }
                                    // Raise event notification
                                    if (this.EventNotificationReceived != null)
                                    {
                                        var notification = new EventNotificationEventArgs() { Date = DateTime.Now, Notification = response.Data };
                                        await Task.Factory.StartNew(() => this.EventNotificationReceived(this, notification));
                                    }
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            if (!continueOnError)
                            {
                                throw;
                            }
                            else
                            {
                                onError?.Invoke(ex);
                            }
                        }
                        await Task.Delay(100);
                    }
                }, this.eventNotifyCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Stops the listening.
        /// </summary>
        public void StopListening()
        {
            if (this.eventNotifyCancellationToken != null)
            {
                this.eventNotifyCancellationToken.Cancel();
                this.eventNotifyCancellationToken = null;
            }
            if (this.queryStateCancellationToken != null)
            {
                this.queryStateCancellationToken.Cancel();
                this.queryStateCancellationToken = null;
            }
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <returns></returns>
        public async Task<State> GetCurrentState()
        {
            BaseResult<State> currentState = null;
            this.autoRaiseEvent.WaitOne();
            do
            {
                currentState = await this.ExecuteOperation<State>(ServiceOperation.GetState);
                if (currentState.Code != ResponseCode.OK)
                {
                    await Task.Delay(100);
                }
            }
            while (currentState == null || currentState.Code != ResponseCode.OK);
            this.CurrentState = currentState.Data;
            this.autoRaiseEvent.Set();
            this.StateUpdated?.Invoke(this, EventArgs.Empty);
            return currentState.Data;
        }

        /// <summary>
        /// Switches to channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public async Task<bool> SwitchToChannel(Channel channel)
        {
            return await SwitchTo(channel.GetOrangeServiceId().ToString().PadLeft(3, '0'));
        }

        /// <summary>
        /// Switches to EPG.
        /// </summary>
        /// <param name="epgId">The epg identifier.</param>
        /// <returns></returns>
        public async Task<bool> SwitchTo(string epgId)
        {
            return await this.ExecuteSimpleOperation(ServiceOperation.SwitchTo, new Dictionary<string, object>() { ["epg_id"] = epgId.PadLeft(10, '*'), ["uui"] = 1 });
        }

        /// <summary>
        /// Sends the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public async Task<bool> SendKey(Key key, PressKeyMode mode = PressKeyMode.SinglePress)
        {
            return await this.ExecuteSimpleOperation(ServiceOperation.SendKey, new Dictionary<string, object>() { ["key"] = key.GetOrangeServiceId(), ["mode"] = mode.GetOrangeServiceId() });
        }

        /// <summary>
        /// Sets the touchpad positon.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public async Task<bool> SetTouchpadPositon(double x, double y)
        {
            return await this.ExecuteSimpleOperation(ServiceOperation.SetTouchpadPositon, new Dictionary<string, object>() { ["relx"] = x, ["rely"] = y });
        }

        /// <summary>
        /// Executes the simple operation and return <c>true</c> if OK.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="optionalParameters">The optional parameters.</param>
        /// <returns></returns>
        private async Task<bool> ExecuteSimpleOperation(ServiceOperation operation, Dictionary<string, object> optionalParameters = null)
        {
            return (await ExecuteOperation<object>(operation, optionalParameters)).Code == ResponseCode.OK;
        }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="operation">The operation.</param>
        /// <param name="optionalParameters">The optional parameters.</param>
        /// <returns></returns>
        private async Task<BaseResult<TResult>> ExecuteOperation<TResult>(ServiceOperation operation, Dictionary<string, object> optionalParameters = null)
        {
            var response = await this.GetWebResponseAsync(new Uri(this.RootUri, $"/remoteControl/cmd?operation={((int)operation).ToString("00")}{(optionalParameters != null ? "&" + string.Join("&", optionalParameters.Select(p => string.Concat(p.Key, "=", p.Value))) : string.Empty)}"));
            return JsonConvert.DeserializeObject<BaseResponse<TResult>>(response, OrangeContractResolver.Settings).Result;
        }

        /// <summary>
        /// Gets the web response asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="throwException">if set to <c>true</c> to throw exception.</param>
        /// <returns></returns>
        private async Task<string> GetWebResponseAsync(Uri uri, bool throwException = false)
        {
            try
            {
                // Create the request
                HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
                // Get the response
                Debug.WriteLine(request.RequestUri);
                WebResponse response = await request.GetResponseAsync();
                // Read and return the content
                string content = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                Debug.WriteLine(content);
                return content;
            }
            catch (Exception ex)
            {
                if (throwException && (ex is WebException && ((WebException)ex).Status == WebExceptionStatus.Timeout) == false)
                {
                    throw ex;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Starts the query state loop in case of TimeShifting.
        /// </summary>
        /// <param name="interval">The interval.</param>
        private void StartQueryStateLoop(int interval = 5000)
        {
            if (queryStateCancellationToken == null)
            {
                queryStateCancellationToken = new CancellationTokenSource();
                Task.Factory.StartNew(async () =>
                {
                    while (!queryStateCancellationToken.IsCancellationRequested)
                    {
                            // Query the state and wait interval
                            await this.GetCurrentState();
                        await Task.Delay(interval);
                    }
                }, queryStateCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
            // If TimeShifting off, stop the loop !
            if (!this.CurrentState.TimeShiftingState)
            {
                queryStateCancellationToken.Cancel();
                queryStateCancellationToken = null;
                // Refresh the state 2 seconds after the TimeShifting is off
                Task.Delay(2000).ContinueWith((t) => this.GetCurrentState());
            }
        }

        /// <summary>
        /// Represent data for the EventNotificationReceived event. 
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class EventNotificationEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the date of the event.
            /// </summary>
            /// <value>
            /// The event date.
            /// </value>
            public DateTime Date { get; set; }

            /// <summary>
            /// Gets or sets the event notification data.
            /// </summary>
            /// <value>
            /// The event notification data.
            /// </value>
            public EventNotification Notification { get; set; }
        }
    }
}

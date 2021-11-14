/*
 *	 Pushover Package for Constellation
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

namespace Pushover
{
    using Constellation.Package;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public class Program : PackageBase
    {
        private const string API_ROOT_URI = "https://api.pushover.net/1/";

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Check settings
            if (!PackageHost.ContainsSetting("Token") || !PackageHost.ContainsSetting("UserId"))
            {
                PackageHost.WriteError("Settngs are missing, the package can't start ! Please review the API token & User Id in the package's settings");
                throw new InvalidOperationException();
            }

            // Check service
            var validateRequest = this.DoRequest<PushoverResponse>("users/validate.json", new Dictionary<string, string> { ["user"] = PackageHost.GetSettingValue("UserId") });
            if (validateRequest == null || validateRequest.Status == false)
            {
                PackageHost.WriteError($"The package can't start ! Unable to call the Pushover service" + validateRequest != null ? " : " + string.Join(", ", validateRequest.Errors) : "");
                throw new InvalidOperationException();
            }

            // Check limits
            this.DoRequest<string>("apps/limits.json?token=" + PackageHost.GetSettingValue("Token"), method: WebRequestMethods.Http.Get);

            // Ready!
            PackageHost.WriteInfo("Package started !");
        }

        /// <summary>
        /// Checks user or group identifiers.
        /// </summary>
        /// <param name="userOrGroupId">The user or group identifier to check.</param>
        /// <param name="device">An optional device name. If the device name is supplied, the verification will apply to that user and device.</param>
        [MessageCallback]
        public bool CheckUserOrGroup(string userOrGroupId, string device = null)
        {
            var parameters = new Dictionary<string, string> { ["user"] = userOrGroupId };
            if (!string.IsNullOrEmpty(device))
            {
                parameters["device"] = device;
            }
            var validateRequest = this.DoRequest<PushoverResponse>("users/validate.json", parameters);
            return (validateRequest != null && validateRequest.Status);
        }

        /// <summary>
        /// Gets the status of your emergency notification.
        /// </summary>
        /// <param name="receipt">The receipt identifier.</param>
        [MessageCallback]
        public PushoverReceipt GetNotificationStatus(string receipt)
        {
            return this.DoRequest<PushoverReceipt>("receipts/" + receipt + ".json?token=" + PackageHost.GetSettingValue("Token"), method: WebRequestMethods.Http.Get);
        }

        /// <summary>
        /// Pushes the specified notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The message's title, otherwise the app's name is used.</param>
        /// <param name="image_url">An url to an attachement to the push message. The package will try and download the image in order to send it to pushover.</param>
        /// <param name="url">A supplementary URL to show with your message.</param>
        /// <param name="user">The user/group key (not e-mail address) of your user (or you), viewable when logged into our dashboard. By default we use the "userId" define in settings</param>
        /// <param name="devices">The user's device name to send the message directly to that device, rather than all of the user's devices.</param>
        /// <param name="sound">The name of one of the sounds supported by device clients to override the user's default sound choice.</param>
        /// <param name="priority">The priority of the message.</param>
        /// <param name="timestamp">A Unix timestamp of your message's date and time to display to the user, rather than the time your message is received by our API.</param>
        /// <param name="emergencyOptions">The optional emergency options if the notification is sent with emergency-priority.</param>
        /// <returns></returns>
        [MessageCallback]
        public PushoverResponse PushNotification(string message, string title = null, string image_url = null, Url url = null, string user = null, string[] devices = null, Sound sound = Sound.Pushover, Priority priority = Priority.Normal, int timestamp = 0, EmergencyOptions emergencyOptions = null)
        {
            var parameters = new Dictionary<string, string>
            {
                ["user"] = user ?? PackageHost.GetSettingValue("UserId"),
                ["message"] = message
            };
            if (parameters.Any(p => string.IsNullOrEmpty(p.Value)))
            {
                return new PushoverResponse() { Errors = new string[] { "Some arguments are missing !" } };
            }
            else
            {
                if (title != null)
                {
                    parameters["title"] = title;
                }
                if (image_url != null)
                {
                    parameters["imageurl"] = image_url;
                }
                if (devices != null)
                {
                    parameters["device"] = string.Join(",", devices);
                }
                if (sound != Sound.Pushover)
                {
                    parameters["sound"] = sound.ToString().ToLower();
                }
                if (priority != Priority.Normal)
                {
                    parameters["priority"] = ((int)priority).ToString();
                    if (priority == Priority.Emergency)
                    {
                        parameters["retry"] = emergencyOptions != null && emergencyOptions.Retry > 0 ? emergencyOptions.Retry.ToString() : PackageHost.GetSettingValue("DefaultEmergencyRetry");
                        parameters["expire"] = emergencyOptions != null && emergencyOptions.Expire > 0 ? emergencyOptions.Expire.ToString() : PackageHost.GetSettingValue("DefaultEmergencyExpiration");
                    }
                }
                if (timestamp > 0)
                {
                    parameters["timestamp"] = timestamp.ToString();
                }
                if (url != null && !string.IsNullOrEmpty(url.URL))
                {
                    parameters["url"] = url.URL;
                    if (!string.IsNullOrEmpty(url.Title))
                    {
                        parameters["url_title"] = url.Title;
                    }
                }

                Task<PushoverResponse> response = Task.Run<PushoverResponse>(async () => await this.DoRequestAsync<PushoverResponse>("messages.json", parameters));
                if (response.Result == null || response.Status == TaskStatus.Faulted)
                {
                    PackageHost.WriteError("Unable to send the notification : " + response.Exception.ToString());
                }

                return response.Result;
            }
        }

        private async System.Threading.Tasks.Task<T> DoRequestAsync<T>(string path, Dictionary<string, string> parameters = null) where T : class
        {
            string strResponse = null;

            parameters["token"] = PackageHost.GetSettingValue("Token");
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    MultipartFormDataContent form = new MultipartFormDataContent();

                    // Creating the form with the parameters.
                    foreach (var item in parameters.Where(p => p.Key != "imageurl"))
                    {
                        form.Add(new StringContent(HttpUtility.UrlEncode(item.Value)), "\"" + item.Key + "\"");
                    }


                    WebClient client = null;
                    Stream stream = null;

                    if (parameters.ContainsKey("imageurl") == true)
                    {
                        // Getting the image from the URL.
                        client = new WebClient();
                        stream = client.OpenRead(parameters["imageurl"]);

                        // Adding it to the form.
                        var imageParameter = new StreamContent(stream);
                        imageParameter.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png"); // The type doesn't seem to be important as long as it is an image
                        form.Add(imageParameter, "attachment", "image.png");
                    }

                    // Removing content types.
                    foreach (var param in form)
                        param.Headers.ContentType = null;

                    // Sending request.
                    HttpResponseMessage responseMessage = await httpClient.PostAsync(API_ROOT_URI + path, form);

                    // Closing potential streams and clients after being used.
                    if (stream != null)
                    {
                        stream.Flush();
                        stream.Close();

                    }
                    if (client != null)
                    {
                        client.Dispose();
                    }

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        strResponse = await responseMessage.Content.ReadAsStringAsync();

                        if (responseMessage.Headers.Contains("X-Limit-App-Limit"))
                        {
                            // Updating state object for rates/limits based on headers informations.
                            var t = int.Parse(responseMessage.Headers.GetValues("X-Limit-App-Limit").FirstOrDefault());
                            PackageHost.PushStateObject("RateLimit", new
                            {
                                Limit = int.Parse(responseMessage.Headers.GetValues("X-Limit-App-Limit").FirstOrDefault()),
                                Remaining = int.Parse(responseMessage.Headers.GetValues("X-Limit-App-Remaining").FirstOrDefault()),
                                Reset = int.Parse(responseMessage.Headers.GetValues("X-Limit-App-Reset").FirstOrDefault())
                            }, "Pushover.RateLimit");
                        }
                    }
                    else
                    {
                        PackageHost.WriteError($"Push request failed with status {(int)responseMessage.StatusCode} {responseMessage.StatusCode}: {responseMessage.Content.ReadAsStringAsync().Result}");
                    }
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Push error: {0}", ex.ToString());
            }

            // Process response
            if (string.IsNullOrEmpty(strResponse))
            {
                return default(T);
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return strResponse as T;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(strResponse, PushoverContractResolver.Settings);
                }
            }
        }
    }
}

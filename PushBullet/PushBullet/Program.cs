/*
 *	 PushBullet Package for Constellation
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

namespace PushBullet
{
    using Constellation;
    using Constellation.Package;
    using Newtonsoft.Json;
    using PushBullet.Models;
    using System;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using WebSocketSharp;

    public class Program : PackageBase
    {
        private const string API_ROOT_URI = "https://api.pushbullet.com/v2";
        private const string WS_ROOT_URI = "wss://stream.pushbullet.com/websocket/";

        private WebSocket _webSocket = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            if (string.IsNullOrEmpty(PackageHost.GetSettingValue("token")))
            {
                throw new Exception("Access Token not defined!");
            }

            // Init the Realtime Event Stream
            this._webSocket = new WebSocket(WS_ROOT_URI + PackageHost.GetSettingValue("token"));
            this._webSocket.OnOpen += (s, e) => PackageHost.WriteInfo("Connected to the realtime event stream");
            this._webSocket.OnClose += (s, e) => PackageHost.WriteWarn("Disconnected to the realtime event stream");
            this._webSocket.OnError += (s, e) => PackageHost.WriteWarn("Error on the realtime event stream : " + e.Message);
            this._webSocket.OnMessage += (s, e) =>
            {
                dynamic message = JsonConvert.DeserializeObject(e.Data);
                switch (((string)message.type))
                {
                    case "nop": //Sent every 30 seconds confirming the connection is active.
                        break;
                    case "tickle":
                        switch ((string)message.subtype)
                        {
                            case "push":
                                string pushesGroup = PackageHost.GetSettingValue("SendPushesReceivedToGroup");
                                if (!string.IsNullOrEmpty(pushesGroup))
                                {
                                    Push lastPush = this.GetPushes(DateTime.UtcNow.AddMinutes(-1))?.Pushes?.FirstOrDefault();
                                    PackageHost.CreateMessageProxy(MessageScope.ScopeType.Group, pushesGroup).ReceivePush(lastPush);
                                }
                                break;
                            case "device":
                                this.GetDevices();
                                break;
                        }
                        break;
                    case "push":
                        dynamic data = message.push;
                        string ephemeralsGroup = PackageHost.GetSettingValue("SendEphemeralsReceivedToGroup");
                        if (!string.IsNullOrEmpty(ephemeralsGroup))
                        {
                            PackageHost.CreateMessageProxy(MessageScope.ScopeType.Group, ephemeralsGroup).ReceiveEphemeral(data);
                        }
                        break;
                }
            };
            this._webSocket.ConnectAsync();
            this.GetCurrentUser();
            this.GetDevices();
            this.GetChats();
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            this._webSocket.CloseAsync();
        }

        /// <summary>
        /// Get a list of devices belonging to the current user.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public DevicesList GetDevices()
        {
            var devices = this.DoRequest<DevicesList>("/devices");
            if (PackageHost.GetSettingValue<bool>("PushDevicesAsStateObjects"))
            {
                PackageHost.PushStateObject("Devices", devices);
            }
            return devices;
        }

        /// <summary>
        /// Get a list of chats belonging to the current user.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public ChatsList GetChats()
        {
            var chats = this.DoRequest<ChatsList>("/chats");
            if (PackageHost.GetSettingValue<bool>("PushChatsAsStateObjects"))
            {
                PackageHost.PushStateObject("Chats", chats);
            }
            return chats;
        }

        /// <summary>
        /// Request push history.
        /// </summary>
        /// <param name="modifiedAfter">Request pushes modified after this timestamp.</param>
        /// <returns></returns>
        [MessageCallback]
        public PushesList GetPushes(DateTime? modifiedAfter = null)
        {
            return this.DoRequest<PushesList>("/pushes" + (modifiedAfter != null && modifiedAfter.HasValue ? "?modified_after=" + modifiedAfter.Value.ToUnixTimeStamp() : string.Empty));
        }

        /// <summary>
        /// Gets the currently logged in user.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public User GetCurrentUser()
        {
            var user = this.DoRequest<User>("/users/me");
            if (PackageHost.GetSettingValue<bool>("PushCurrentUserAsStateObject"))
            {
                PackageHost.PushStateObject("CurrentUser", user);
            }
            return user;
        }

        /// <summary>
        /// Sends the SMS from your phone (for Android devices only).
        /// </summary>
        /// <param name="userId">The identifier of the user sending this message.</param>
        /// <param name="deviceId">The identifier of the device corresponding to the phone that should send the SMS.</param>
        /// <param name="to">Phone number to send the SMS to.</param>
        /// <param name="message">The SMS message to send.</param>
        [MessageCallback]
        public void SendSMS(string userId, string deviceId, string to, string message)
        {
            this.PushEphemeral(new
            {
                type = "messaging_extension_reply",
                package_name = "com.pushbullet.android",
                source_user_iden = userId,
                target_device_iden = deviceId,
                conversation_iden = to,
                message = message
            });
        }

        /// <summary>
        /// Copy a String to the Device's Clipboard (PushBullet premium feature).
        /// </summary>
        /// <param name="userId">The identifier of the user sending this message.</param>
        /// <param name="deviceId">The identifier of the device sending this message.</param>
        /// <param name="body">The text to copy to the clipboard.</param>
        [MessageCallback]
        public void CopyToClipboard(string userId, string deviceId, string body)
        {
            this.PushEphemeral(new
            {
                type = "clip",
                source_user_iden = userId,
                source_device_iden = deviceId,
                body = body
            });
        }

        /// <summary>
        /// Push a note to a device or another person.
        /// </summary>
        /// <param name="title">The note's title.</param>
        /// <param name="body">The note's message.</param>
        /// <param name="target">The target type to send the push.</param>
        /// <param name="targetArgument">The target argument to send the push.</param>
        [MessageCallback]
        public void PushNote(string title, string body, PushTargetType target = PushTargetType.Device, string targetArgument = null)
        {
            dynamic data = new ExpandoObject();
            data.type = "note";
            data.title = title;
            data.body = body;
            this.Push(data, target, targetArgument);
        }

        /// <summary>
        /// Push a Link to a device or another person.
        /// </summary>
        /// <param name="title">The link's title.</param>
        /// <param name="body">A message associated with the link.</param>
        /// <param name="url">The url to open.</param>
        /// <param name="target">The target type to send the push.</param>
        /// <param name="targetArgument">The target argument to send the push.</param>
        [MessageCallback]
        public void PushLink(string title, string body, string url, PushTargetType target = PushTargetType.Device, string targetArgument = null)
        {
            dynamic data = new ExpandoObject();
            data.type = "link";
            data.title = title;
            data.body = body;
            data.url = url;
            this.Push(data, target, targetArgument);
        }

        /// <summary>
        /// Upload and Push a file to a device or another person.
        /// </summary>
        /// <param name="fileUri">The URI of the file you want to send.</param>
        /// <param name="body">A message to go with the file.</param>
        /// <param name="target">The target type to send the push.</param>
        /// <param name="targetArgument">The target argument to send the push.</param>
        [MessageCallback]
        public void PushFile(string fileUri, string body, PushTargetType target = PushTargetType.Device, string targetArgument = null)
        {
            var upload = this.UploadFile(fileUri);
            this.PushFile(upload.Filename, upload.FileType, upload.FileURL, body, target, targetArgument);
        }

        private RequestUploadAuthorization UploadFile(string fileUri, RequestUploadAuthorization requestUploadAuthorization = null)
        {
            if (string.IsNullOrEmpty(fileUri))
            {
                throw new ArgumentNullException("FileURI cannot be null", fileUri);
            }
            // Get local file path
            Uri uri = null; string localFilepath = null;
            if (Uri.TryCreate(fileUri, UriKind.RelativeOrAbsolute, out uri))
            {
                if (uri.Scheme == Uri.UriSchemeFile)
                {
                    localFilepath = uri.LocalPath;
                }
                else
                {
                    localFilepath = Path.Combine(Path.GetTempPath(), uri.Segments.Last());
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFile(uri, localFilepath);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("Invalid file URI", fileUri);
            }
            if (!File.Exists(localFilepath))
            {
                throw new FileNotFoundException("File not found", localFilepath);
            }
            // Get the request upload authorization
            if (requestUploadAuthorization == null)
            {
                requestUploadAuthorization = this.GetRequestUploadAuthorization(Path.GetFileName(localFilepath), MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(localFilepath)));
            }
            // Create the MultipartFormDataContent
            var formDataContent = new System.Net.Http.MultipartFormDataContent();
            formDataContent.Add(new System.Net.Http.StreamContent(File.OpenRead(localFilepath)), "file", requestUploadAuthorization.Filename);
            // Post the file
            var httpClient = new System.Net.Http.HttpClient();
            var result = httpClient.PostAsync(requestUploadAuthorization.UploadURL, formDataContent);
            if (result.Result.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw result.Exception ?? new Exception(string.Format("Code {0} : {1}", (int)result.Result.StatusCode, result.Result.ReasonPhrase));
            }
            return requestUploadAuthorization;
        }
        
        private RequestUploadAuthorization GetRequestUploadAuthorization(string filename, string filetype)
        {
            return JsonConvert.DeserializeObject<RequestUploadAuthorization>(this.DoRequest("/upload-request", new { file_name = filename, file_type = filetype }), PushBulletContractResolver.Settings);
        }

        private void PushFile(string file_name, string file_type, string file_url, string body, PushTargetType target = PushTargetType.Device, string targetArgument = null)
        {
            dynamic data = new ExpandoObject();
            data.type = "file";
            data.file_name = file_name;
            data.file_type = file_type;
            data.file_url = file_url;
            data.body = body;
            this.Push(data, target, targetArgument);
        }

        private void PushEphemeral(object data)
        {
            this.DoRequest("/ephemerals", new { type = "push", push = data });
        }

        private void Push(dynamic data, PushTargetType target, string targetArgument = null)
        {
            switch (target)
            {
                case PushTargetType.Channel:
                    data.channel_tag = targetArgument;
                    break;
                case PushTargetType.Client:
                    data.client_iden = targetArgument;
                    break;
                case PushTargetType.Device:
                    data.device_iden = targetArgument;
                    break;
                case PushTargetType.Email:
                    data.email = targetArgument;
                    break;
            }
            this.DoRequest("/pushes", data);
        }

        private T DoRequest<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(DoRequest(path), PushBulletContractResolver.Settings);
        }

        private string DoRequest(string path, object payload)
        {
            return this.DoRequest(path, "POST", JsonConvert.SerializeObject(payload));
        }

        private string DoRequest(string path, string method = "GET", string payload = null)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(API_ROOT_URI + path);
                request.Method = method;
                request.Headers.Add("Access-Token", PackageHost.GetSettingValue("token"));
                if (method == "POST" && !string.IsNullOrEmpty(payload))
                {
                    byte[] payloadData = UTF8Encoding.UTF8.GetBytes(payload);
                    request.ContentType = "application/json";
                    request.ContentLength = payloadData.Length;
                    using (Stream requestBody = request.GetRequestStream())
                    {
                        requestBody.Write(payloadData, 0, payloadData.Length);
                        requestBody.Close();
                    }
                }
                PackageHost.WriteDebug("{0} {1}", request.Method, request.RequestUri.ToString());

                WebResponse response = request.GetResponse();
                PackageHost.PushStateObject("RateLimit", new
                {
                    Limit = int.Parse(response.Headers["X-Ratelimit-Limit"]),
                    Remaining = int.Parse(response.Headers["X-Ratelimit-Remaining"]),
                    Reset = int.Parse(response.Headers["X-Ratelimit-Reset"])
                }, "PushBullet.RateLimits");

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        PackageHost.WriteError("Response error: {0}", reader.ReadToEnd());
                    }
                }
                catch
                {
                    PackageHost.WriteError("Response error: {0}", ex.ToString());
                }
                return string.Empty;
            }
        }
    }
}

/*
 *	 Huawei Mobile Router Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace HuaweiMobileRouter
{
    using HuaweiMobileRouter.Models;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Huawei Mobile Router API
    /// </summary>
    public class Router
    {
        private const int DEFAULT_TIMEOUT = 10000; //ms

        private SessionInformations sessionInfo = null;
        private CookieContainer cookieContainer = new CookieContainer();

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the credential.
        /// </summary>
        /// <value>
        /// The credential.
        /// </value>
        public NetworkCredential Credential { get; private set; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        /// <value>
        /// The request timeout.
        /// </value>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);

        /// <summary>
        /// Gets the monthly statistics.
        /// </summary>
        public MonthlyStatistics MonthlyStatistics => this.GetDatas<MonthlyStatistics>("monitoring/month_statistics");

        /// <summary>
        /// Gets the device information.
        /// </summary>
        public DeviceInformation DeviceInformation => this.GetDatas<DeviceInformation>("device/information");

        /// <summary>
        /// Gets the device signal.
        /// </summary>
        public DeviceSignal DeviceSignal => this.GetDatas<DeviceSignal>("device/signal");

        /// <summary>
        /// Gets the monitoring status.
        /// </summary>
        public MonitoringStatus MonitoringStatus => this.GetDatas<MonitoringStatus>("monitoring/status");

        /// <summary>
        /// Gets the traffic statistics.
        /// </summary>
        public TrafficStatistics TrafficStatistics => this.GetDatas<TrafficStatistics>("monitoring/traffic-statistics");

        /// <summary>
        /// Gets the wlan basic settings.
        /// </summary>
        public WlanBasicSettings WlanBasicSettings => this.GetDatas<WlanBasicSettings>("wlan/basic-settings");

        /// <summary>
        /// Gets the PLMN informations.
        /// </summary>
        public PLMNInformations PLMNInformations => this.GetDatas<PLMNInformations>("net/current-plmn");

        /// <summary>
        /// Gets notifications.
        /// </summary>
        public Notification Notification => this.GetDatas<Notification>("monitoring/check-notifications");

        /// <summary>
        /// Gets the PIN status.
        /// </summary>
        public PinStatus PinStatus => this.GetDatas<PinStatus>("pin/status");

        /// <summary>
        /// Gets the SMS.
        /// </summary>
        public SMSList SMS
        {
            get
            {
                this.RenewToken();
                var task = this.ExecuteWebRequestAsync(new Uri($"http://{this.Host}/api/sms/sms-list"), postData:
                   "<?xml version=\"1.0\" encoding=\"UTF-8\"?><request>" +
                                 "<PageIndex>1</PageIndex>" +
                                 "<ReadCount>50</ReadCount>" +
                                 "<BoxType>1</BoxType>" +
                                 "<SortType>0</SortType>" +
                                 "<Ascending>0</Ascending>" +
                                 "<UnreadPreferred>0</UnreadPreferred>" +
                                 "</request>");
                return this.ParseResponse<SMSList>(task);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Router"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public Router(string host)
            : this(host, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Router"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public Router(string host, string username, string password)
            : this(host, new NetworkCredential(username, password))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Router"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="credential">The credential.</param>
        public Router(string host, NetworkCredential credential)
        {
            this.Host = host;
            this.Credential = credential;
            this.RenewToken();
            if (this.Credential != null)
            {
                this.Login();
            }
        }

        /// <summary>
        /// Renews the token.
        /// </summary>
        public void RenewToken()
        {
            this.sessionInfo = this.GetDatas<SessionInformations>("webserver/SesTokInfo");
            this.cookieContainer.SetCookies(new Uri($"http://{this.Host}/"), this.sessionInfo.SessionId);
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Unable to login without credentials set</exception>
        public bool Login()
        {
            if (this.Credential != null)
            {
                var hashedPassword = ToBase64Encode(SHA256HexHashString(this.Credential.UserName + ToBase64Encode(SHA256HexHashString(this.Credential.Password)) + this.sessionInfo.TokenId));
                return this.PostRequest("user/login",
                        $"<Username>{this.Credential.UserName}</Username>" +
                        $"<Password>{hashedPassword}</Password>" +
                        "<password_type>4</password_type>");
            }
            else
            {
                throw new Exception("Unable to login without credentials set");
            }
        }

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="phones">The phones.</param>
        /// <returns></returns>
        public bool SendSMS(string content, params string[] phones)
        {
            this.RenewToken();
            return this.PostRequest("sms/send-sms",
                "<Index>-1</Index>" +
                "<Phones>" + string.Join("", phones.Select(p => $"<Phone>{p}</Phone>")) + "</Phones>" +
                "<Sca/>" +
                $"<Content>{content}</Content>" +
                $"<Length>{content.Length}</Length>" +
                "<Reserved>1</Reserved>" +
                $"<Date>{DateTime.Now.ToString()}</Date>");
        }

        /// <summary>
        /// Sets the SMS read.
        /// </summary>
        /// <param name="smsIndex">Index of the SMS.</param>
        /// <returns></returns>
        public bool SetSMSRead(int smsIndex)
        {
            this.RenewToken();
            return this.PostRequest("sms/set-read",
                $"<Index>{smsIndex}</Index>");
        }

        /// <summary>
        /// Deletes the SMS.
        /// </summary>
        /// <param name="smsIndex">Index of the SMS.</param>
        /// <returns></returns>
        public bool DeleteSMS(int smsIndex)
        {
            this.RenewToken();
            return this.PostRequest("sms/delete-sms",
                $"<Index>{smsIndex}</Index>");
        }

        private TResponse GetDatas<TResponse>(string path)
        {
            return this.ParseResponse<TResponse>(this.ExecuteWebRequestAsync(new Uri($"http://{this.Host}/api/{path}")));
        }

        private bool PostRequest(string path, string requestContent)
        {
            var task = this.ExecuteWebRequestAsync(new Uri($"http://{this.Host}/api/{path}"),
                postData: $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request>{requestContent}</request>");
            if (task.Wait(this.RequestTimeout))
            {
                return task.Result.Contains("<response>OK</response>");
            }
            else
            {
                throw new TimeoutException();
            }
        }

        private async Task<string> ExecuteWebRequestAsync(Uri uri, bool throwException = false, string postData = "")
        {
            try
            {
                // Create the request
                HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
                request.CookieContainer = this.cookieContainer;
                // Add the verification token
                if (this.sessionInfo != null)
                {
                    request.Headers.Add("__RequestVerificationToken", this.sessionInfo.TokenId);
                }
                // POST data
                if (!string.IsNullOrEmpty(postData))
                {
                    var data = Encoding.UTF8.GetBytes(postData);
                    request.Method = WebRequestMethods.Http.Post;
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
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

        private TResponse ParseResponse<TResponse>(Task<string> task)
        {
            if (!task.Wait(this.RequestTimeout))
            {
                throw new TimeoutException();
            }
            else if (typeof(TResponse) != typeof(Error) && task.Result.Contains("<error>"))
            {
                throw new RouterErrorException(this.ParseResponse<Error>(task));
            }
            else
            {
                var serializer = new XmlSerializer(typeof(TResponse));
                using (TextReader reader = new StringReader(task.Result))
                {
                    return (TResponse)serializer.Deserialize(reader);
                }
            }
        }

        private static string ToBase64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        private static string SHA256HexHashString(string input)
        {
            using (var sha256 = SHA256Managed.Create())
            {
                return ToHex(sha256.ComputeHash(Encoding.Default.GetBytes(input)));
            }
        }

        private static string ToHex(byte[] bytes, bool upperCase = false)
        {
            var result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            }
            return result.ToString();
        }
    }
}

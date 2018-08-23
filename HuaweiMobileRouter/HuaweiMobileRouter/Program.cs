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
    using Constellation;
    using Constellation.Package;
    using HuaweiMobileRouter.Models;
    using System;
    using System.Net;
    using System.Timers;

    /// <summary>
    /// Huawei Mobile Router Constellation Package
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private Timer timer = null;
        private Router router = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Connecting ...
            try
            {
                PackageHost.WriteInfo($"Connecting to {PackageHost.GetSettingValue("Host")} ...");
                NetworkCredential routerCredential = PackageHost.ContainsSetting("Username") && PackageHost.ContainsSetting("Password") ? new NetworkCredential(PackageHost.GetSettingValue("Username"), PackageHost.GetSettingValue("Password")) : null;
                this.router = new Router(PackageHost.GetSettingValue("Host"), routerCredential);
            }
            catch (RouterErrorException ex)
            {
                PackageHost.WriteError(ex.Message);
                throw;
            }
            catch (TimeoutException)
            {
                PackageHost.WriteError("Unable to connect to your Huawei router !");
                throw;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error during the startup : " + ex.ToString());
                throw;
            }

            // Publish StateObject
            this.Update();
            this.PushStateObject<DeviceInformation>(() => this.router.DeviceInformation, false);
            PackageHost.WriteInfo($"Connected!");

            // Starting the loop
            this.timer = new Timer(PackageHost.GetSettingValue<int>("RefreshInterval")) { AutoReset = true, Enabled = true };
            this.timer.Elapsed += (s, e) => this.Update();
            this.timer.Start();
        }

        /// <summary>
        /// Called when the package is shutdown (disconnected from Constellation)
        /// </summary>
        public override void OnShutdown()
        {
            this.timer.Stop();
        }

        /// <summary>
        /// Sends the SMS (one or more phone numbers separated by commas)
        /// </summary>
        /// <param name="phoneNumber">The phone numbers.</param>
        /// <param name="content">The SMS content.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SendSMS(string phoneNumber, string content)
        {
            return this.router.SendSMS(content, phoneNumber.Split(','));
        }

        /// <summary>
        /// Sets the SMS read.
        /// </summary>
        /// <param name="smsIndex">Index of the SMS.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetSMSRead(int smsIndex)
        {
            return this.router.SetSMSRead(smsIndex);
        }

        /// <summary>
        /// Deletes the SMS.
        /// </summary>
        /// <param name="smsIndex">Index of the SMS.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool DeleteSMS(int smsIndex)
        {
            return this.router.DeleteSMS(smsIndex);
        }

        private void Update()
        {
            // Forward incoming SMS ?
            if (PackageHost.ContainsSetting("ForwardIncomingSMSTo") && !string.IsNullOrEmpty(PackageHost.GetSettingValue("ForwardIncomingSMSTo")) && this.router.Notification.UnreadMessage > 0)
            {
                foreach (Message sms in this.router.SMS.List.Messages)
                {
                    if (sms.Status == Message.Smstat.Unread)
                    {
                        PackageHost.WriteInfo($"Forwarding incoming SMS from {sms.Phone}");
                        MessageScope.Create(MessageScope.ScopeType.Group, PackageHost.GetSettingValue("ForwardIncomingSMSTo"))
                            .GetProxy()
                            .IncomingSMS(new { Number = sms.Phone, Text = sms.Content, sms.Date, sms.Priority });
                        if (PackageHost.GetSettingValue<bool>("KeepSMSCopy"))
                        {
                            this.router.SetSMSRead(sms.Index);
                        }
                        else
                        {
                            this.router.DeleteSMS(sms.Index);
                        }
                    }
                }
            }
            // Update StateObjects
            this.PushStateObject<DeviceSignal>(() => this.router.DeviceSignal);
            this.PushStateObject<MonitoringStatus>(() => this.router.MonitoringStatus);
            this.PushStateObject<MonthlyStatistics>(() => this.router.MonthlyStatistics);
            this.PushStateObject<PinStatus>(() => this.router.PinStatus);
            this.PushStateObject<PLMNInformations>(() => this.router.PLMNInformations);
            this.PushStateObject<TrafficStatistics>(() => this.router.TrafficStatistics);
            this.PushStateObject<WlanBasicSettings>(() => this.router.WlanBasicSettings);
            this.PushStateObject<Notification>(() => this.router.Notification);
            this.PushStateObject<SMSList>(() => this.router.SMS);
        }

        private void PushStateObject<TObject>(Func<TObject> func, bool withLifetime = true)
        {
            try
            {
                PackageHost.PushStateObject(typeof(TObject).Name, func(), lifetime: withLifetime ? (PackageHost.GetSettingValue<int>("RefreshInterval") / 1000) * 2 : 0);
            }
            catch { }
        }
    }
}

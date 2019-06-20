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
                PackageHost.WriteInfo($"Connected!");
            }
            catch (RouterErrorException ex)
            {
                PackageHost.WriteError("Huawei error code#" + ex.Message);
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

            // Publish StateObjects
            PackageHost.WriteInfo($"Getting device informations ...");
            this.Update();
            this.PushStateObject<DeviceInformation>(() => this.router.DeviceInformation, false);

            // Starting the loop
            PackageHost.WriteInfo($"Starting polling task ...");
            this.timer = new Timer(PackageHost.GetSettingValue<int>("RefreshInterval")) { AutoReset = true, Enabled = true };
            this.timer.Elapsed += (s, e) => this.Update();
            this.timer.Start();

            // Done
            PackageHost.WriteInfo($"Ready!");
        }

        /// <summary>
        /// Called when the package is shutdown (disconnected from Constellation)
        /// </summary>
        public override void OnShutdown()
        {
            this.timer?.Stop();
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
            try
            {
                PackageHost.WriteInfo($"Sending SMS to {phoneNumber} ...");
                return this.router.SendSMS(content, phoneNumber.Split(','));
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Unable to send SMS : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sets the SMS read.
        /// </summary>
        /// <param name="smsIndex">Index of the SMS.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetSMSRead(int smsIndex)
        {
            try
            {
                PackageHost.WriteInfo($"Setting SMS #{smsIndex} as read ...");
                return this.router.SetSMSRead(smsIndex);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Unable to set SMS read : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes the SMS.
        /// </summary>
        /// <param name="smsIndex">Index of the SMS.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool DeleteSMS(int smsIndex)
        {
            try
            {
                PackageHost.WriteInfo($"Deleting SMS #{smsIndex} ...");
                return this.router.DeleteSMS(smsIndex);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Unable to delete SMS : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reboots the router.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public bool Reboot()
        {
            try
            {
                PackageHost.WriteWarn($"Rebooting the router ...");
                return this.router.Reboot();
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Unable to reboot the router : {ex.Message}");
                return false;
            }
        }

        private void Update()
        {
            try
            {
                // Stop the timer
                this.timer?.Stop();

                // Check login state
                LoginState loginState = null;
                try
                {
                    loginState = this.router.LoginState;
                }
                catch (TimeoutException) { }
                catch (RouterErrorException exception) when
                    (exception.Error.Code == Error.ErrorCode.ERROR_WRONG_SESSION ||
                     exception.Error.Code == Error.ErrorCode.ERROR_WRONG_SESSION_TOKEN ||
                     exception.Error.Code == Error.ErrorCode.ERROR_WRONG_TOKEN)
                {
                    // Do not thrown exception when session expired (so loginState remains null)
                }
                if (this.router.Credential != null && (loginState == null || loginState.State < 0))
                {
                    PackageHost.WriteWarn("Renewing authentification ...");
                    if (this.router.Login())
                    {
                        PackageHost.WriteInfo("Authentification done!");
                    }
                }

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
            catch (TimeoutException)
            {
                PackageHost.WriteError("Connection timeout");
            }
            catch (RouterErrorException ex)
            {
                PackageHost.WriteError($"Error during the polling. Code#{ex.Message}");
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Error during the polling : {ex}");
            }
            finally
            {
                // Restart the timer
                this.timer?.Start();
            }
        }

        private void PushStateObject<TObject>(Func<TObject> func, bool withLifetime = true)
        {
            try
            {
                PackageHost.PushStateObject(typeof(TObject).Name, func(), lifetime: withLifetime ? (PackageHost.GetSettingValue<int>("RefreshInterval") / 1000) * 2 : 0);
            }
            catch (Exception ex)
            {
                PackageHost.WriteDebug($"Unable to push {typeof(TObject).FullName} : {ex}");
            }
        }
    }
}

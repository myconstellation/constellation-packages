/*
 *	 Exchange Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Exchange
{
    using Constellation.Package;
    using Microsoft.Exchange.WebServices.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;

    /// <summary>
    /// Represent the Exchange package
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    [StateObjectKnownTypes(typeof(List<Appointment>))]
    public class Program : PackageBase
    {
        private Timer timer = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            // Create and start thread to retrieve calendars
            this.timer = new Timer(new TimerCallback((o) =>
            {
                int numberOfDaysToInclude = PackageHost.GetSettingValue<int>("NumberOfDaysToInclude");
                foreach (var accountName in PackageHost.GetSettingValue<string>("CalendarAccounts").Split(';'))
                {
                    try
                    {
                        int atIndex = accountName.IndexOf('@');
                        if (atIndex <= 0)
                        {
                            PackageHost.WriteError("Invalid account: " + accountName);
                        }
                        else
                        {
                            var mailboxName = accountName.Substring(0, atIndex);
                            // Find appointements
                            var calendarFolderId = new FolderId(WellKnownFolderName.Calendar, new Mailbox(accountName));
                            var calendarView = new CalendarView(DateTime.Now, DateTime.Now.AddDays(numberOfDaysToInclude));
                            var appointments = this.CreateExchangeService().FindAppointments(calendarFolderId, calendarView).Items.Select(item => new Appointment()
                            {
                                Id = item.Id.UniqueId,
                                Subject = item.Subject,
                                //Body = item.Body.Text,
                                Start = item.Start,
                                End = item.End,
                                Location = item.Location,
                                Duration = item.Duration
                            }).ToList();
                            // Push appointements list
                            PackageHost.PushStateObject("Appointments4" + mailboxName, appointments,
                                lifetime: (int)TimeSpan.FromMinutes(PackageHost.GetSettingValue<int>("RefreshInterval")).TotalSeconds * 2,
                                metadatas: new Dictionary<string, object>()
                                {
                                { "StartDate", calendarView.StartDate },
                                { "EndDate", calendarView.EndDate },
                                { "Mailbox", accountName }
                                });
                            // And log ...
                            PackageHost.WriteInfo("Calendar of '{0}' pushed into the Constellation for the next {1} day(s) ({2} appointement(s)).", accountName, numberOfDaysToInclude, appointments.Count);
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error while getting the calendar of '{0}' : {1}", accountName, ex.ToString());
                    }
                }
            }), null, 0, (int)TimeSpan.FromMinutes(PackageHost.GetSettingValue<int>("RefreshInterval")).TotalMilliseconds);
            
            // Update settings callback
            PackageHost.SettingsUpdated += (s, e) =>
            {
                // Update timer interval
                this.timer.Change(0, (int)TimeSpan.FromMinutes(PackageHost.GetSettingValue<int>("RefreshInterval")).TotalMilliseconds);
            };
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            // Stop the timer
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Sends e-mail message.
        /// </summary>
        /// <param name="mailRequest">The e-mail request.</param>
        [MessageCallback]
        public void SendMail(SendMailRequest mailRequest)
        {
            // Create the Email message
            var message = new EmailMessage(this.CreateExchangeService())
            {
                Importance = mailRequest.IsImportant ? Importance.High : Importance.Normal,
                Subject = mailRequest.Subject,
                Body = mailRequest.Body
            };
            message.ToRecipients.AddRange(mailRequest.To.Split(';'));
            // Send the email message and save a copy.
            message.SendAndSaveCopy();
        }

        private ExchangeService CreateExchangeService()
        {
            // Try parse Exchange version (Exchange2013_SP1 by default)
            ExchangeVersion exVersion = ExchangeVersion.Exchange2013_SP1;
            if (!Enum.TryParse<ExchangeVersion>(PackageHost.GetSettingValue<string>("ExchangeVersion"), out exVersion))
            {
                PackageHost.WriteWarn("Invalid Exchange Version: {0}", PackageHost.GetSettingValue<string>("ExchangeVersion"));
            }
            // Create the Exchange Service class
            return new ExchangeService(exVersion)
            {
                Credentials = new NetworkCredential(PackageHost.GetSettingValue<string>("AccountName"), PackageHost.GetSettingValue<string>("AccountPassword"), PackageHost.GetSettingValue<string>("AccountDomain")),
                Url = new Uri(PackageHost.GetSettingValue<string>("EWSServiceUrl"))
            };
        }
    }
}

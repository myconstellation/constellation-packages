/*
 *	 DayInfo Package for Constellation
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

namespace DayInfo
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Utils;

    public class Program : PackageBase
    {
        private DateTime dateProcessed = DateTime.MinValue;
        private string timeProcessed = "";

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            while (PackageHost.IsRunning)
            {
                // Each day
                if (DateTime.Now.Date != dateProcessed.Date)
                {
                    // Push SunInfo
                    PackageHost.PushStateObject("SunInfo", this.GetSunInfo(DateTime.Now, PackageHost.GetSettingValue<int>("TimeZone"), PackageHost.GetSettingValue<double>("Latitude"), PackageHost.GetSettingValue<double>("Longitude")));
                    // Push NameDay
                    PackageHost.PushStateObject("NameDay", NameDayUtils.GetNameDay(), metadatas: new Dictionary<string, object>() { ["Date"] = DateTime.Now });
                    //Push Date
                    PackageHost.PushStateObject("Date", DateTime.Now.ToString("dddd d MMMM").Replace(" 1 ", " 1er "), lifetime: 86400);
                    // Updated for today !
                    dateProcessed = DateTime.Now;
                    PackageHost.WriteInfo($"StateObjects updated for today ({dateProcessed.ToShortDateString()})");
                }

                // Each Minute
                if (DateTime.Now.ToString("HH:mm") != timeProcessed)
                {
                    string time = DateTime.Now.ToString("HH:mm");
                    // Push Time
                    PackageHost.PushStateObject("Time", time, lifetime: 60);
                    // Updated for the moment !
                    timeProcessed = time;
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Gets the name day for the given day
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        [MessageCallback]
        public string GetNameDay(Date date)
        {
            return NameDayUtils.GetNameDay(new DateTime(date.Year, date.Month, date.Day));
        }

        /// <summary>
        /// Calculate the Universal Coordinated Time (UTC) of sunset and sunrise for the given day at the given location on earth
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="timezone">The timezone.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        [MessageCallback]
        public SunInfo GetSunInfo(Date date, int timezone, double latitude, double longitude)
        {
            return this.GetSunInfo(new DateTime(date.Year, date.Month, date.Day), timezone, latitude, longitude);
        }

        private SunInfo GetSunInfo(DateTime date, int timezone, double latitude, double longitude)
        {
            // Calcul Sunrise & sunset
            double jdDate = NAAUtils.calcJD(date);
            double sunRise = NAAUtils.calcSunRiseUTC(jdDate, latitude, longitude);
            double sunSet = NAAUtils.calcSunSetUTC(jdDate, latitude, longitude);
            bool isDaylightSavingTime = TimeZoneInfo.Local.IsDaylightSavingTime(date);
            // Return result
            return new SunInfo
            {
                Date = DateTime.Now.Date,
                TimeZone = timezone,
                Longitude = longitude,
                Latitude = latitude,
                DayLightSavings = isDaylightSavingTime,
                Sunrise = NAAUtils.getDateTime(sunRise, timezone, DateTime.Now, isDaylightSavingTime).Value.TimeOfDay,
                Sunset = NAAUtils.getDateTime(sunSet, timezone, DateTime.Now, isDaylightSavingTime).Value.TimeOfDay
            };
        }
    }

    /// <summary>
    /// Represents a date.
    /// </summary>
    public class Date
    {
        /// <summary>
        /// Gets or sets the day of the month represented by this instance.
        /// </summary>
        /// <value>
        /// The day of the month represented by this instance..
        /// </value>
        public int Day { get; set; }
        /// <summary>
        /// Gets or sets the month component of the date represented by this instance.
        /// </summary>
        /// <value>
        /// The month component of the date represented by this instance..
        /// </value>
        public int Month { get; set; }
        /// <summary>
        /// Gets or sets the year component of the date represented by this instance.
        /// </summary>
        /// <value>
        /// The month year of the date represented by this instance..
        /// </value>
        public int Year { get; set; }
    }

    /// <summary>
    /// Sunrise and sunset informations
    /// </summary>
    [StateObject]
    public class SunInfo
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public int TimeZone { get; set; }
        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public double Longitude { get; set; }
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public double Latitude { get; set; }
        /// <summary>
        /// Indicates whether a specified date and time falls in the range of daylight saving time for the time zone of the current object.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the specified date and time falls in the range of daylight saving time for the time zone of the current object; otherwise, <c>false</c>.
        /// </value>
        public bool DayLightSavings { get; set; }
        /// <summary>
        /// Gets or sets the sunrise time.
        /// </summary>
        /// <value>
        /// The sunrise time.
        /// </value>
        public TimeSpan Sunrise { get; set; }
        /// <summary>
        /// Gets or sets the sunset time.
        /// </summary>
        /// <value>
        /// The sunset time.
        /// </value>
        public TimeSpan Sunset { get; set; }
    }
}

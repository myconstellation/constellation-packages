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
                    // Updated for today !
                    dateProcessed = DateTime.Now;
                    PackageHost.WriteInfo($"StateObjects updated for today ({dateProcessed.ToShortDateString()})");
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
        public string GetNameDay(Date date) => NameDayUtils.GetNameDay(new DateTime(date.Year, date.Month, date.Day));

        /// <summary>
        /// Calculate the Universal Coordinated Time (UTC) of sunset and sunrise for the given day at the given location on earth
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="timezone">The timezone.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        [MessageCallback]
        public SunInfo GetSunInfo(Date date, int timezone, double latitude, double longitude) => this.GetSunInfo(new DateTime(date.Year, date.Month, date.Day), timezone, latitude, longitude);

        private SunInfo GetSunInfo(DateTime date, int timezone, double latitude, double longitude)
        {
            double jdDate = NAAUtils.calcJD(date);
            bool isDaylightSavingTime = TimeZoneInfo.Local.IsDaylightSavingTime(date);

            // Return result
            return new SunInfo
            {
                Date = DateTime.Now.Date,
                TimeZone = timezone,
                Longitude = longitude,
                Latitude = latitude,
                DayLightSavings = isDaylightSavingTime,
                Sunrise = this.GetSunrise(jdDate, latitude, longitude, timezone, isDaylightSavingTime),
                Sunset = this.GetSunset(jdDate, latitude, longitude, timezone, isDaylightSavingTime),
                CivilianSunrise = this.GetSunrise(jdDate, latitude, longitude, timezone, isDaylightSavingTime, TwilightOffset.Civilian),
                CivilianSunset = this.GetSunset(jdDate, latitude, longitude, timezone, isDaylightSavingTime, TwilightOffset.Civilian),
                NauticalSunrise = this.GetSunrise(jdDate, latitude, longitude, timezone, isDaylightSavingTime, TwilightOffset.Nautical),
                NauticalSunset = this.GetSunset(jdDate, latitude, longitude, timezone, isDaylightSavingTime, TwilightOffset.Nautical),
                AstronomicalSunrise = this.GetSunrise(jdDate, latitude, longitude, timezone, isDaylightSavingTime, TwilightOffset.Astronomical),
                AstronomicalSunset = this.GetSunset(jdDate, latitude, longitude, timezone, isDaylightSavingTime, TwilightOffset.Astronomical)
            };
        }

        private TimeSpan GetSunrise(double jdDate, double latitude, double longitude, int timezone, bool isDaylightSavingTime, TwilightOffset offset = TwilightOffset.None)
        {
            double sunRise = NAAUtils.calcSunRiseUTC(jdDate, latitude, longitude);
            return NAAUtils.getDateTime(sunRise, timezone, DateTime.Now, isDaylightSavingTime).Value.TimeOfDay;
        }

        private TimeSpan GetSunset(double jdDate, double latitude, double longitude, int timezone, bool isDaylightSavingTime, TwilightOffset offset = TwilightOffset.None)
        {
            double sunRise = NAAUtils.calcSunSetUTC(jdDate, latitude, longitude);
            return NAAUtils.getDateTime(sunRise, timezone, DateTime.Now, isDaylightSavingTime).Value.TimeOfDay;
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
        /// <summary>
        /// Gets ot sets the civilian sunrise time
        /// </summary>
        /// <value>
        /// The civilian sunrise time.
        /// </value>
        public TimeSpan CivilianSunrise { get; set; }
        /// <summary>
        /// Gets or sets the civilian sunset time
        /// </summary>
        /// <value>
        /// The civilian sunset time.
        /// </value>
        public TimeSpan CivilianSunset { get; set; }
        /// <summary>
        /// Gets ot sets the nautical sunrise time
        /// </summary>
        /// <value>
        /// The nautical sunrise time.
        /// </value>
        public TimeSpan NauticalSunrise { get; set; }
        /// <summary>
        /// Gets or sets the nautical sunset time
        /// </summary>
        /// <value>
        /// The nautical sunset time.
        /// </value>
        public TimeSpan NauticalSunset { get; set; }
        /// <summary>
        /// Gets ot sets the astronomical sunrise time
        /// </summary>
        /// <value>
        /// The astronomical sunrise time.
        /// </value>
        public TimeSpan AstronomicalSunrise { get; set; }
        /// <summary>
        /// Gets or sets the astronomical sunset time
        /// </summary>
        /// <value>
        /// The astronomical sunset time.
        /// </value>
        public TimeSpan AstronomicalSunset { get; set; }
    }
}

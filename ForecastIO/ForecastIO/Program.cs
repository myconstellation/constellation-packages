/*
 *	 ForecastIO Package for Constellation
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

namespace ForecastIO
{
    using Constellation.Package;
    using ForecastIO.Extensions;
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ForecastIO package
    /// </summary>
    [StateObjectKnownTypes(typeof(ForecastIOResponse))]
    public class Program : PackageBase
    {
        private ForecastIOConfigurationSection configuration = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this.configuration = PackageHost.GetSettingAsConfigurationSection<ForecastIOConfigurationSection>("forecastIOConfigurationSection", true);

            int nbSeconde = 0;
            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (nbSeconde == 0)
                    {
                        foreach (StationElement station in this.configuration.Stations)
                        {
                            PackageHost.WriteInfo("Getting forecast for {0}", station.Name);
                            try
                            {
                                ForecastIOResponse response = new ForecastIORequest(this.configuration.ApiKey, (float)station.Latitude, (float)station.Longitude, this.configuration.Unit, this.configuration.Language).Get();
                                PackageHost.PushStateObject<ForecastIOResponse>(station.Name, response, lifetime: (int)this.configuration.RefreshInterval.TotalSeconds * 2);
                                PackageHost.WriteInfo("Weather for {0} done. Report date @ {1}", station.Name, response.currently.time.ToDateTime().ToLocalTime().ToLongTimeString());
                            }
                            catch (Exception ex)
                            {
                                PackageHost.WriteError("Unable to get the weather for {0} : {1}", station.Name, ex.Message);
                            }
                        }
                    }
                    Thread.Sleep(1000);
                    nbSeconde++;

                    if (nbSeconde == (int)this.configuration.RefreshInterval.TotalSeconds)
                    {
                        nbSeconde = 0;
                    }
                }
            });
        }

        /// <summary>
        /// Gets the weather forecast for a given GPS location.
        /// </summary>
        /// <param name="longitude">The longitude.</param>
        /// <param name="latitude">The latitude.</param>
        [MessageCallback]
        public ForecastIOResponse GetWeatherForecast(float longitude, float latitude)
        {
            if (MessageContext.Current.IsSaga)
            {
                PackageHost.WriteInfo("Getting forecast requested by {0}", MessageContext.Current.Sender.FriendlyName);
                return new ForecastIORequest(this.configuration.ApiKey, latitude, longitude, Unit.si).Get();
            }
            else
            {
                PackageHost.WriteWarn("This is not a saga !");
                return null;              
            }
        }
    }
}

/*
*	 Copyright 2018 Aurélien Chevalier	   	  
*	
*	 Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
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

using Constellation.Package;
using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Timers;
using PrixCarburants.Models;
using static System.Math;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace PrixEssence
{
    /// <summary>
    /// Constellation package to get fuel prices in French gas stations
    /// </summary>
    public class Program : PackageBase
    {
        private static readonly CultureInfo _format = new CultureInfo("en-US");
        private static Timer _timer;
        private static StationsList _list;

        #region Message Callbacks

        /// <summary>
        /// Get the cheapest gas station for a fuel type in a given radius
        /// </summary>
        /// <param name="lat"> User lattitude (WGS84)</param>
        /// <param name="lon"> User longitude (WGS84)</param>
        /// <param name="fuel">Fuel type in French</param>
        /// <param name="range">Range in meters to find the station</param>
        /// <returns>GPS coordinates of the cheapest station, fuel type and price</returns>
        [MessageCallback]
        public Station BestPriceInArea(double lat, double lon, Fuel fuel, int range = 1000)
        {
            List<Station> results = FindInArea(lat, lon, range).FindAll(pdv => pdv.Prices.Any(p => p.Id == fuel));

            return GetCheapestStationForFuelType(results, fuel);
        }

        /// <summary>
        /// Find all Gas station in a given radius
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        [MessageCallback]
        public List<Station> FindInArea(double lat, double lon, int range = 1000)
        {
            return _list.Stations.FindAll(pdv => GetDistance(lat, lon, pdv.Latitude, pdv.Longitude) <= range);
        }

        /// <summary>
        /// Get price of a fuel type in a station
        /// </summary>
        /// <param name="id">ID of the station on Prix-carburants.</param>
        /// <param name="fuel">Car's fuel type</param>
        /// <returns>Fuel price</returns>
        [MessageCallback]
        public double? GetPrice(int id, Fuel fuel)
        {
            return _list.Stations.FirstOrDefault(pdv => pdv.Id == id)?.Prices.FirstOrDefault(prix => prix.Id == fuel)?.Value;
        }

        #endregion Message Callbacks

        #region Init

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            PackageHost.TryGetSettingValue("interval", out int interval, 6);

            Refresh(null, null);

            // Set a timer to refesh prices every n hours
            _timer = new Timer(interval * 3600 * 1000)
            {
                AutoReset = true
            };
            _timer.Elapsed += Refresh;
            _timer.Start();
        }

        /// <summary>
        /// Called when the package is stopped.
        /// </summary>
        public override void OnPreShutdown()
        {
            _timer.Close();
        }

        #endregion Init

        #region private 

        /// <summary>
        /// COmpute distance from geographic coordinates
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        private static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            int earth_radius = 6378137;
            // Convert to radian
            double rlo1 = lng1 * (PI * 2) / 360;
            double rla1 = lat1 * (PI * 2) / 360;
            double rlo2 = lng2 * (PI * 2) / 360;
            double rla2 = lat2 * (PI * 2) / 360;
            double dlo = (rlo2 - rlo1) / 2;
            double dla = (rla2 - rla1) / 2;
            double a = (Sin(dla) * Sin(dla)) + Cos(rla1) * Cos(rla2) * (Sin(dlo) * Sin(dlo));
            double d = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
            return earth_radius * d;
        }

        /// <summary>
        /// Get the cheapest station in a list for a specified fuel
        /// </summary>
        /// <param name="stations"></param>
        /// <param name="fuel"></param>
        /// <returns></returns>
        private static Station GetCheapestStationForFuelType(List<Station> stations, Fuel fuel)
        {
            return stations.FindAll(pdv => pdv.Prices.Any(prix => prix.Id == fuel)) // stations that matches the fuel
                            .OrderBy(pdv => pdv.Prices.FirstOrDefault(prix => prix.Id == fuel)?.Value) // order by price
                            .FirstOrDefault(); // take the first
        }

        /// <summary>
        /// Get data from prix-carburants.gouv.fr
        /// </summary>
        private static StationsList GetData()
        {
            // Get the data provided as a zip archive
            using (WebClient wc = new WebClient())
            {
                byte[] datas = wc.DownloadData("https://donnees.roulez-eco.fr/opendata/instantane");
                using (MemoryStream str = new MemoryStream(datas))
                {
                    using (ZipArchive archive = new ZipArchive(str, ZipArchiveMode.Read))
                    {
                        using (Stream content = archive.Entries[0].Open())
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(StationsList));
                            return xs.Deserialize(content) as StationsList;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gas stations refresh
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void Refresh(object source, ElapsedEventArgs e)
        {
            try
            {
                _list = GetData();
                PackageHost.WriteInfo("Gas stations list refreshed");

                //range search enabled
                if (PackageHost.TryGetSettingValue("latitude", out double latitude)
                    && PackageHost.TryGetSettingValue("longitude", out double longitude)
                    && PackageHost.TryGetSettingValue("range", out int range)
                    )
                {
                    List<Station> areaResults = FindInArea(latitude, longitude, range);
                    PackageHost.PushStateObject("InArea", areaResults);

                    PackageHost.WriteInfo($"{areaResults.Count} gas stations found in specified area");

                    //cheapest station for specified fuel type
                    if (PackageHost.TryGetSettingValue("cheapest-fuel-types", out string fuelTypes))
                    {
                        foreach (string fuelType in fuelTypes.Split(','))
                        {
                            if (Enum.TryParse(fuelType.Trim(), true, out Fuel fuel))
                            {
                                //get the station
                                Station cheapest = GetCheapestStationForFuelType(areaResults, fuel);
                                Dictionary<string, object> metadatas = null;

                                //get the price of the fuel
                                if (cheapest != null)
                                {
                                    Price prix = cheapest.Prices.FirstOrDefault(p => p.Id == fuel);
                                    if (prix != null)
                                    {
                                        metadatas = new Dictionary<string, object>()
                                        {
                                            ["price"] = prix.Value,
                                            ["name"] = prix.Name
                                        };
                                    }
                                }

                                PackageHost.PushStateObject($"Cheapest-{fuelType}", cheapest, metadatas: metadatas);

                                PackageHost.WriteInfo($"Cheapest '{fuelType}' found in specified area");
                            }
                        }
                    }
                }

                //specific id search enabled
                if (PackageHost.TryGetSettingValue("station-ids", out string stationIds))
                {
                    foreach (Station pdv in _list.Stations.FindAll(pdv => stationIds.Contains(pdv.Id.ToString())))
                    {
                        PackageHost.PushStateObject(pdv.Id.ToString(), pdv);

                        PackageHost.WriteInfo($"Gas station '{pdv.Id}' found");
                    }
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"An error ocurred while getting datas : '{ex.Message}'");
            }
        }

        #endregion private
    }
}

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

namespace PrixEssence
{
    public class Program : PackageBase
    {
        private static CultureInfo format = new CultureInfo("en-US");
        private int n;
        private Timer t;
        private XmlDocument doc;

        /// <summary>
        /// Get cheapeast price for a fuel type in a given radius
        /// </summary>
        /// <param name="lat"> User lattitude (WGS84)</param>
        /// <param name="lon"> User longitude (WGS84)</param>
        /// <param name="fuel">Fuel type in French</param>
        /// <param name="range">Range in meters to find the station</param>
        /// <returns>GPS coordinates of the cheapest station, fuel type and price</returns>
        [MessageCallback]
        public BestPrice BestPriceInArea(double lat, double lon, string fuel, int range = 1000)
        {
            List<BestPrice> l = new List<BestPrice>();
            // Let's find the stations in a given range
            var stations = doc.SelectNodes("//pdv");
            foreach (XmlNode s in stations)
            {
                if (s.Attributes["latitude"].Value.Length == 0 || s.Attributes["longitude"].Value.Length == 0)
                    continue;
                int idStation = int.Parse(s.Attributes["id"].Value);
                double latStation, lonStation;
                if (!Double.TryParse(s.Attributes["latitude"].Value, NumberStyles.Any, format, out latStation))
                    latStation = int.Parse(s.Attributes["latitude"].Value);
                if (!Double.TryParse(s.Attributes["longitude"].Value, NumberStyles.Any, format, out lonStation))
                    lonStation = int.Parse(s.Attributes["longitude"].Value);
                // Gas stations should have their coordinates in WGS84*100000 .
                if (GetDistance(lat, lon, latStation / 100000, lonStation / 100000) > range)
                    continue;
                // Now we only have stations in the given range
                var prices = s.SelectNodes("prix[@nom=\"" + fuel + "\"]");
                if (prices.Count == 0)
                    continue;
                l.Add(new BestPrice(latStation / 100000, lonStation / 100000, fuel, GetPrice(idStation, fuel)));
            }
            return l.OrderBy(p => p.Price).ToArray()[0];
        }

        /// <summary>
        /// Get price of a fuel type in a station
        /// </summary>
        /// <param name="id">ID of the station on Prix-carburants.</param>
        /// <param name="carburant">Car's fuel type</param>
        /// <returns>Fuel price</returns>
        [MessageCallback]
        public double GetPrice(int id, string carburant)
        {
            XmlNode node = doc.DocumentElement.SelectSingleNode("pdv[@id=\"" + id + "\"]/prix[@nom=\"" + carburant + "\"]");
            try
            {
                return double.Parse(node.Attributes["valeur"].Value, format);
            }
            catch (Exception)
            {
                PackageHost.WriteError("Invalid value for {0}", carburant);
            }

            return 0.0;
        }

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
        /// Get data from prix-carburants.gouv.fr
        /// </summary>
        private void GetData()
        {
            // Get the data provided as a zip archive
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile("https://donnees.roulez-eco.fr/opendata/instantane", "prices.zip");
            }
            catch (Exception)
            {
                PackageHost.WriteError("Couldn't get prices");
            }
            FileStream zipToOpen = new FileStream("prices.zip", FileMode.Open);
            ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
            Stream content = archive.Entries[0].Open();
            // Getting data in the XML
            XmlDocument doc = new XmlDocument();
            doc.Load(content);
            this.doc = doc;
        }

        /// <summary>
        /// Push prices of the gas station chosen by user
        /// </summary>
        private void GetAllPrices()
        {
            int id = int.Parse(PackageHost.GetSettingValue("station"));
            XmlNode node = doc.DocumentElement.SelectSingleNode("pdv[@id=\"" + id + "\"]");
            var prices = node.SelectNodes("prix");
            foreach (XmlNode p in prices)
            {
                // We push each price
                string nom = p.Attributes["nom"].Value;
                try
                {
                    double valeur = double.Parse(p.Attributes["valeur"].Value, format);
                    PackageHost.PushStateObject(nom, valeur);
                }
                catch (Exception)
                {
                }
            }
            PackageHost.WriteInfo("Prices updated");
        }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            n = (PackageHost.GetSettingValue("interval").Length == 0) ? int.Parse(PackageHost.GetSettingValue("interval")) : 6;
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            GetData();
            try
            {
                GetAllPrices();
            }
            catch { }
            // Set a timer to refesh prices every n hours
            t = new Timer(n * 3600 * 1000)
            {
                AutoReset = true
            };
            t.Elapsed += Refresh;
            t.Start();
        }

        /// <summary>
        /// Called when the package is stopped.
        /// </summary>
        public override void OnPreShutdown()
        {
            t.Close();
        }

        private void Refresh(Object source, ElapsedEventArgs e)
        {
            GetData();
            try
            {
                GetAllPrices();
            }
            catch { }
        }
    }
}

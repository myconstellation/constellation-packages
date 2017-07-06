using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using RainHourForecast.Models;

namespace RainHourForecast
{
    public class Program : PackageBase
    {

        /// <summary>
        /// Refresh rate of the forecast in minutes.
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// Towns' id comma separated.
        /// </summary>
        public string Ville { get; set; }

        /// <summary>
        /// Url from meteo france to get town's id.
        /// </summary>
        public string IdUrl { get; set; }

        /// <summary>
        /// Url from meteo france to get rain forecast.
        /// </summary>
        public string ForecastUrl { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            this.RefreshInterval = PackageHost.GetSettingValue<int>("RefreshInterval");
            this.Ville = PackageHost.GetSettingValue<string>("Towns");
            Ville = Ville.Replace(" ", String.Empty);
            this.IdUrl = PackageHost.GetSettingValue<string>("IdUrl");
            this.ForecastUrl = PackageHost.GetSettingValue<string>("ForecastUrl");
            int wait = RefreshInterval * 60000;
            while (PackageHost.IsRunning)
            {
                var towns = Ville.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string town in towns)
                {
                    int id = Convert.ToInt32(town);
                    Forecast forecast = RainForecast(id);
                    PackageHost.WriteInfo("Getting rain forecast for town with id {0}", id);
                    PackageHost.PushStateObject<Forecast>(town, forecast);
                }   
                Thread.Sleep(wait);
            }
        }

        /// <summary>
        /// Get rain forecast for the next hour.
        /// </summary>
        /// <param name="Town_ID">Town's id.</param>
        /// <returns>Rain forecast for the next hour</returns>
        [MessageCallback(Key = "RainForecast")]
        Forecast RainForecast(int Town_ID)
        {
            try
            {
                string url = String.Format(this.ForecastUrl, Town_ID);
                var syncClient = new WebClient();
                var content = syncClient.DownloadString(url);
                Forecast forecast = JsonConvert.DeserializeObject<Forecast>(content);
                return forecast;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("{0}", ex);
                return null;
            }
        }

        /// <summary>
        /// Find ID for a specific town name / cp.
        /// </summary>
        /// <param name="Postal_Code">Town's postal code.</param>
        /// <returns>Town id, name and postal code</returns>
        [MessageCallback(Key = "FindId")]
        List<Town> FindId(int Postal_Code)
        {
            try
            {
                string url = String.Format(this.IdUrl, Postal_Code);
                var syncClient = new WebClient();
                var content = syncClient.DownloadString(url);
                List<Town> town = JsonConvert.DeserializeObject<List<Town>>(content);
                return town;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("{0}",ex);
                return null;
            }
        }

    }
}

using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RainHourForecast.Models;

namespace RainHourForecast
{
    public class Program : PackageBase
    {

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            int nbSeconde = 0;
            string Towns = null;
            int wait = 60;

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (nbSeconde == 0)
                    {
                        if (!PackageHost.TryGetSettingValue<string>("TownsId", out Towns))
                        {
                            PackageHost.WriteError("Impossible de récupérer le setting 'TownsId' en string");
                        }
                        Towns = Towns.Replace(" ", String.Empty);
                        var towns = Towns.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string TownId in towns)
                        {
                            PackageHost.WriteInfo("Getting rain forecast for town with id {0}", TownId);
                            try
                            {
                                int id = Convert.ToInt32(TownId);
                                Forecast forecast = RainForecast(id);
                                PackageHost.PushStateObject<Forecast>(TownId, forecast);
                                PackageHost.WriteInfo("Rain forecast for town with id {0} done", TownId);
                            }
                            catch (Exception ex)
                            {
                                PackageHost.WriteError("Unable to get rain forecast for town with id {0} : {1}", TownId, ex.Message);
                            }
                        }
                    }
                    Thread.Sleep(1000);
                    nbSeconde++;

                    if (PackageHost.TryGetSettingValue<int>("RefreshInterval", out wait))
                    {
                        wait = wait * 60;
                    }
                    else
                    {
                        PackageHost.WriteError("Impossible de récupérer le setting 'RefreshInterval' en int");
                    }

                    if (nbSeconde == (int)wait)
                    {
                        nbSeconde = 0;
                    }
                }
            });
        }

        /// <summary>
        /// Get rain forecast for the next hour.
        /// </summary>
        /// <param name="TownId">Town's id.</param>
        /// <returns>Rain forecast for the next hour</returns>
        [MessageCallback]
        public Forecast RainForecast(int TownId)
        {
            string ForecastUrl = null;
            if (!PackageHost.TryGetSettingValue<string>("ForecastUrl", out ForecastUrl))
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'ForecastUrl' en string");
            }
            try
            {
                string FinalUrl = String.Format(ForecastUrl, TownId);
                var syncClient = new WebClient();
                var content = syncClient.DownloadString(FinalUrl);
                Forecast forecast = JsonConvert.DeserializeObject<Forecast>(content);
                return forecast;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Erreur : {0}", ex);
                return null;
            }
        }

        /// <summary>
        /// Find ID for a specific town name / cp.
        /// </summary>
        /// <param name="PostalCode">Town's postal code.</param>
        /// <returns>Town id, name and postal code</returns>
        [MessageCallback]
        public List<Town> FindId(int PostalCode)
        {
            string IdUrl = null;
            if (!PackageHost.TryGetSettingValue<string>("IdUrl", out IdUrl))
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'IdUrl' en string");
            }
            try
            {
                string FinalUrl = String.Format(IdUrl, PostalCode);
                var syncClient = new WebClient();
                var content = syncClient.DownloadString(FinalUrl);
                List<Town> Towns = JsonConvert.DeserializeObject<List<Town>>(content);
                return Towns;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Erreur : {0}",ex);
                return null;
            }
        }

    }
}

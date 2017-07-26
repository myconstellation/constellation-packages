using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
            int nbSeconde = 0;

            Task.Factory.StartNew(() =>
            {            
                while (PackageHost.IsRunning)
                {                  
                    if (nbSeconde == 0)
                    {
                        PackageHost.WriteInfo("Getting rain forecast for town with id");
                        if (PackageHost.TryGetSettingAsXmlDocument("TownsId", out XmlDocument XmlTowns))
                        {
                            foreach (XmlNode town in XmlTowns.ChildNodes[0])
                            {
                                PackageHost.WriteInfo("Getting rain forecast for town with id {0}", town.Attributes["id"].Value);
                                try
                                {
                                    int id = Convert.ToInt32(town.Attributes["id"].Value);
                                    Forecast forecast = RainForecast(id);
                                    PackageHost.PushStateObject<Forecast>(town.Attributes["id"].Value, forecast);
                                    PackageHost.WriteInfo("Rain forecast for town with id {0} done", town.Attributes["id"].Value);
                                }
                                catch (Exception ex)
                                {
                                    PackageHost.WriteError("Unable to get rain forecast for town with id {0} : {1}", town.Attributes["id"].Value, ex.Message);
                                }
                            }
                        }
                        else
                        {
                            PackageHost.WriteError("Impossible de récupérer le setting 'TownsId' en xml");
                        }

                    }
                    Thread.Sleep(1000);
                    nbSeconde++;

                    int wait = PackageHost.GetSettingValue<int>("RefreshInterval")*60;

                    if (nbSeconde == wait)
                    {
                        nbSeconde = 0;
                    }
                }
            });
            
            PackageHost.WriteInfo("RainHourForecast is started !");
        }

        /// <summary>
        /// Get rain forecast for the next hour.
        /// </summary>
        /// <param name="townId">Town's id.</param>
        /// <returns>Rain forecast for the next hour</returns>
        [MessageCallback]
        public Forecast RainForecast(int townId)
        {
            string ForecastUrl = null;
            if (!PackageHost.TryGetSettingValue<string>("ForecastUrl", out ForecastUrl))
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'ForecastUrl' en string");
            }
            try
            {
                string FinalUrl = String.Format(ForecastUrl, townId);
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
        /// <param name="postalCode">Town's postal code.</param>
        /// <returns>Town id, name and postal code</returns>
        [MessageCallback]
        public List<Town> FindId(int postalCode)
        {
            string IdUrl = null;
            if (!PackageHost.TryGetSettingValue<string>("IdUrl", out IdUrl))
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'IdUrl' en string");
            }
            try
            {
                string FinalUrl = String.Format(IdUrl, postalCode);
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

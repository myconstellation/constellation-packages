﻿using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Constellation.Package;

namespace Pollution
{
    public class Program : PackageBase
    {
        /// <summary>
        /// Client pour interroger l'api
        /// </summary>
        static HttpClient client = new HttpClient();

        /// <summary>
        /// Identifiant pour se connecter a l'api d'aqicn
        /// </summary>
        static string aqicnId
        {
            get { return PackageHost.GetSettingValue<string>("AqicnIdId"); }
        }

        /// <summary>
        /// Url de l'api
        /// </summary>
        private static readonly string baseUrl = "https://api.waqi.info";

        /// <summary>
        /// Feed
        /// </summary>
        private static readonly string feed = "feed";

        /// <summary>
        /// Feed
        /// </summary>
        private static readonly string map = "map/bounds";

        /// <summary>
        /// search
        /// </summary>
        private static readonly string search = "search";

        /// <summary>
        /// Response error
        /// </summary>
        private static readonly string nug = "nug";

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            Timer syncTimer = new Timer(1000 * 3600 * 3);
            syncTimer.Elapsed += async (source, e) => { await RecupererPollutionIp(); };
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
            syncTimer.Start();
            PackageHost.WriteInfo("Package started");
        }

        #region Feed
        /// <summary>
        /// Récupère la pollution pour une ville
        /// </summary>
        /// <param name="ville">La ville</param>
        /// <returns>la pollution pour une ville</returns>
        [MessageCallback]
        private static async Task RecupererPollutionVille(string ville)
        {
            PushStateObject($"Pollution {ville}", await Request($"{baseUrl}/{feed}/{ville}/?token={aqicnId}"));
        }

        /// <summary>
        /// Récupère la pollution à partir d'une IP
        /// </summary>
        /// <returns>la pollution à partir d'une IP</returns>
        [MessageCallback]
        private static async Task RecupererPollutionIp()
        {
            PushStateObject("Pollution Ip", await Request($"{baseUrl}/{feed}/here/?token={aqicnId}"));
        }

        /// <summary>
        /// Récupère la pollution à partir de coordonnées GPS
        /// </summary>
        /// <param name="latitude">La latitude</param>
        /// <param name="longitude">La longitude</param>
        /// <returns>la pollution à partir de coordonnées GPS</returns>
        [MessageCallback]
        private static async Task RecupererPollutionGeoloc(string latitude, string longitude)
        {
            PushStateObject("Position Geoloc", await Request($"{baseUrl}/{feed}/geo:{latitude};{longitude}/?token={aqicnId}"));
        }
        #endregion

        /// <summary>
        /// Récupère les stations à partir de coordonnées GPS
        /// </summary>
        /// <returns>les stations à partir de coordonnées GPS</returns>
        [MessageCallback]
        private static async Task RecupererStationsGeoloc(string latitudeZone1, string longitudeZone1, string latitudeZone2, string longitudeZone2)
        {
            PushStateObject("Stations Geoloc", await Request($"{baseUrl}/{map}/?latlng={latitudeZone1},{longitudeZone1},{latitudeZone2},{longitudeZone2}&token={aqicnId}"));
        }

        /// <summary>
        /// Récupère une station par son nom
        /// </summary>
        /// <returns>une station par son nom</returns>
        [MessageCallback]
        private static async Task RecupererStationsParNom(string keyword)
        {
            PushStateObject("Stations Nom", await Request($"{baseUrl}/{search}/?token={aqicnId}&keyword={keyword}"));
        }

        /// <summary>
        /// Interrogation de l'api
        /// </summary>
        /// <param name="path">url</param>
        /// <param name="cpt">compteur d'appel</param>
        /// <returns>Réponse de l'api sous forme de chaine</returns>
        private static async Task<object> Request(string path, int cpt = 0)
        {
            object response = null;
            HttpResponseMessage httpResponse = await client.GetAsync(path);
            if (httpResponse.IsSuccessStatusCode)
            {
                string strResponse = await httpResponse.Content.ReadAsStringAsync();
                response = Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
                if (strResponse.Contains(nug) && cpt < 5)
                {
                    await Task.Delay(30000);
                    return await Request(path, cpt++);
                    ;
                }
            }

            return response;
        }

        /// <summary>
        /// Ajout du state object
        /// </summary>
        /// <param name="nom">Nom du SO</param>
        /// <param name="val">Valeur du SO</param>
        private static void PushStateObject(string nom, object val)
        {
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(val);

            if (!val.ToString().Contains(nug))
            {
                if (PackageHost.GetSettingValue<bool>("Verbose"))
                {
                    PackageHost.WriteInfo("Mise à jours Pollution => {0} : {1}", nom, val);
                }
                PackageHost.PushStateObject(nom, val); // todo : lifetime en seconde (*2 interval) + metdata
            }
        }
    }
}

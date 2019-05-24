using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Constellation.Package;

namespace Pollution
{
    public class Program : PackageBase
    {
        /// <summary>
        /// Identifiant pour se connecter a l'api d'aqicn
        /// </summary>
        private static string AqicnId
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

        /// <summary>
        /// Time between refreshs
        /// </summary>
        private static int Refresh
        {
            get { return PackageHost.GetSettingValue<int>("RefreshInterval") * 1000 * 3600; }
        }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Pollution package started");

            RecupererPollutionIp();

            Timer syncTimer = new Timer(Refresh);
            syncTimer.Elapsed += (source, e) => { RecupererPollutionIp(); };
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
            syncTimer.Start();
        }

        #region Feed
        /// <summary>
        /// Récupère la pollution pour une ville
        /// </summary>
        /// <param name="ville">La ville</param>
        /// <returns>la pollution pour une ville</returns>
        [MessageCallback]
        private static void RecupererPollutionVille(string ville)
        {
            PushStateObject($"Pollution {ville}", Request($"{baseUrl}/{feed}/{ville}/?token={AqicnId}").ConfigureAwait(false).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Récupère la pollution à partir d'une IP
        /// </summary>
        /// <returns>la pollution à partir d'une IP</returns>
        [MessageCallback]
        private static void RecupererPollutionIp()
        {
            PushStateObject("Pollution Ip", Request($"{baseUrl}/{feed}/here/?token={AqicnId}").ConfigureAwait(false).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Récupère la pollution à partir de coordonnées GPS
        /// </summary>
        /// <param name="latitude">La latitude</param>
        /// <param name="longitude">La longitude</param>
        /// <returns>la pollution à partir de coordonnées GPS</returns>
        [MessageCallback]
        private static void RecupererPollutionGeoloc(string latitude, string longitude)
        {
            PushStateObject("Position Geoloc", Request($"{baseUrl}/{feed}/geo:{latitude};{longitude}/?token={AqicnId}").ConfigureAwait(false).GetAwaiter().GetResult());
        }
        #endregion

        /// <summary>
        /// Récupère les stations à partir de coordonnées GPS
        /// </summary>
        /// <returns>les stations à partir de coordonnées GPS</returns>
        [MessageCallback]
        private static void RecupererStationsGeoloc(string latitudeZone1, string longitudeZone1, string latitudeZone2, string longitudeZone2)
        {
            PushStateObject("Stations Geoloc", Request($"{baseUrl}/{map}/?latlng={latitudeZone1},{longitudeZone1},{latitudeZone2},{longitudeZone2}&token={AqicnId}").ConfigureAwait(false).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Récupère une station par son nom
        /// </summary>
        /// <returns>une station par son nom</returns>
        [MessageCallback]
        private static void RecupererStationsParNom(string keyword)
        {
            PushStateObject("Stations Nom", Request($"{baseUrl}/{search}/?token={AqicnId}&keyword={keyword}").ConfigureAwait(false).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Interrogation de l'api
        /// </summary>
        /// <param name="path">url</param>
        /// <param name="cpt">compteur d'appel</param>
        /// <returns>Réponse de l'api sous forme de chaine</returns>
        private static async Task<string> Request(string path, int cpt = 0)
        {
            string res = null;
            HttpResponseMessage response = await new HttpClient().GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsStringAsync();
                if (res.Contains(nug) && cpt < 5)
                {
                    await Task.Delay(30000);
                    return await Request(path, cpt++);
                }
            }

            return res;
        }

        /// <summary>
        /// Ajout du state object
        /// </summary>
        /// <param name="nom">Nom du SO</param>
        /// <param name="val">Valeur du SO</param>
        private static void PushStateObject(string nom, string val)
        {
            if (!val.ToString().Contains(nug))
            {
                PackageHost.PushStateObject(nom, Newtonsoft.Json.JsonConvert.DeserializeObject(val), lifetime: Refresh * 2);
            }
        }
    }
}

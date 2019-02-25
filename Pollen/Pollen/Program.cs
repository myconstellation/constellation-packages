using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using Constellation.Package;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Pollen
{
    /// <summary>
    /// https://www.pollens.fr/
    /// https://www.pollenwarndienst.at/index.php?eID=appinterface&action=getHourlyLoadData&type=zip&value=11000&country=FR&lang_id=8&pure_json=1
    /// </summary>
    public class Program : PackageBase
    {
        /// <summary>
        /// Le numéro de département
        /// </summary>
        static int DepNum
        {
            get { return PackageHost.GetSettingValue<int>("DepartementNumber"); }
        }

        /// <summary>
        /// Frequence de rafraichissement
        /// </summary>
        static int RefreshInterval
        {
            get { return PackageHost.GetSettingValue<int>("RefreshInterval"); }
        }

        /// <summary>
        /// Frequence de rafraichissement
        /// </summary>
        static bool Log
        {
            get { return PackageHost.GetSettingValue<bool>("Log"); }
        }

        /// <summary>
        /// Liste des risques
        /// </summary>
        public static List<Risque> Risques = new List<Risque>(5);

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            Risques.Add(new Risque() { id = 0, couleur = "#FFFFFF" });
            Risques.Add(new Risque() { id = 1, couleur = "#74E46C" });
            Risques.Add(new Risque() { id = 2, couleur = "#048000" });
            Risques.Add(new Risque() { id = 3, couleur = "#F2EA1A" });
            Risques.Add(new Risque() { id = 4, couleur = "#FF7F29" });
            Risques.Add(new Risque() { id = 5, couleur = "#FF0200" });

            // Execution maintenant
            SetPollen();
            // puis a frequence régulière
            if (Log) PackageHost.WriteInfo($"Mise à jours des données toutes les {RefreshInterval} secondes");
            Timer syncTimer = new Timer(RefreshInterval * 1000);
            syncTimer.Elapsed += (source, e) =>
            {
                SetPollen();
            };
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
        }

        private static void SetPollen()
        {
            if (Log) PackageHost.WriteInfo("Mise à jours des données sur les risques du pollen");

            // Récupération contenu page
            WebRequest request = WebRequest.Create("https://pollens.fr/load_vigilance_map");
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            reader.Close();
            response.Close();

            // Extraction data
            var data = JObject.Parse(JObject.Parse(content).SelectToken("vigilanceMapCounties").ToString()).SelectToken(DepNum.ToString());
            
            // Récup risque global
            int.TryParse(((JValue)data.SelectToken("riskLevel")).Value.ToString(), out int riskLevel);

            // Traitement HTML
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(data.SelectToken("riskSummary").ToString());

            // Récup couleurs
            HtmlNodeCollection couleurCollec = document.DocumentNode.SelectNodes("//g[rect]/rect");
            // Récup nom
            HtmlNodeCollection nomCollec = document.DocumentNode.SelectNodes("//tspan[@text-anchor='end']/text()");

            // Construction données
            string couleur;
            List<Vegetal> vegetaux = new List<Vegetal>(couleurCollec.Count);
            for (int i = 0; i < couleurCollec.Count; i++)
            {
                // TODO : gerer la correspondance, via Y ? (couleurCollec[i].Attributes)
                couleur = GetColor(couleurCollec[i].Attributes["style"].Value);
                vegetaux.Add(new Vegetal(((HtmlTextNode)nomCollec[i]).Text.Trim(), couleur, Risques.Single(r => r.couleur.Equals(couleur)).id));
            }

            PackageHost.PushStateObject("Pollens", vegetaux, metadatas: new Dictionary<string, object> { ["Departement"] = DepNum }, lifetime: RefreshInterval * 2);

            PackageHost.PushStateObject("Risque", Risques.Single(r => r.id.Equals(riskLevel)), metadatas: new Dictionary<string, object> { ["Departement"] = DepNum }, lifetime: RefreshInterval * 2);
        }

        /// <summary>
        /// Récupère la couleur a partir dans la css
        /// </summary>
        /// <param name="css"></param>
        /// <returns></returns>
        public static string GetColor(string css)
        {
            var color = string.Empty;
            css.Split(';').ToList().ForEach(attr =>
            {
                if (attr.Contains("fill"))
                {
                    color = attr.Split(':')[1].Trim();
                }
            });

            return color.ToUpper();
        }
    }
}

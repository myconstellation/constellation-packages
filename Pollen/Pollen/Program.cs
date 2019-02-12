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
    /// https://www.polleninfo.org/FR/fr/aktuelle-belastung/allergierisiko.html?tx_scload_dailyload%5B__referrer%5D%5B%40extension%5D=ScLoad&tx_scload_dailyload%5B__referrer%5D%5B%40vendor%5D=Screencode&tx_scload_dailyload%5B__referrer%5D%5B%40controller%5D=Load&tx_scload_dailyload%5B__referrer%5D%5B%40action%5D=dailyload&tx_scload_dailyload%5B__referrer%5D%5Barguments%5D=YTowOnt931f91268ac9faaadcce4fa5ba74b5e8759ba6f42&tx_scload_dailyload%5B__referrer%5D%5B%40request%5D=a%3A4%3A%7Bs%3A10%3A%22%40extension%22%3Bs%3A6%3A%22ScLoad%22%3Bs%3A11%3A%22%40controller%22%3Bs%3A4%3A%22Load%22%3Bs%3A7%3A%22%40action%22%3Bs%3A9%3A%22dailyload%22%3Bs%3A7%3A%22%40vendor%22%3Bs%3A10%3A%22Screencode%22%3B%7D32f5a7fd4e25cbc93511ddade72f9e6e1b7e0980&tx_scload_dailyload%5B__trustedProperties%5D=a%3A2%3A%7Bs%3A7%3A%22country%22%3Bi%3A1%3Bs%3A3%3A%22zip%22%3Bi%3A1%3B%7D2b0ebfc1842648d7e112553e4e592b09a1aadc87&tx_scload_dailyload%5Bcountry%5D=FR&tx_scload_dailyload%5Bzip%5D=62136#breadcrumb
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

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            // Execution maintenant
            SetPollen();
            // puis a frequence régulière
            Timer syncTimer = new Timer(RefreshInterval);
            syncTimer.Elapsed += (source, e) => { SetPollen(); };
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
        }

        private static void SetPollen()
        {
            if (Log) PackageHost.WriteInfo("Mise à jours des données sur les risques du pollen");

            // Récupération contenu page
            WebRequest request = WebRequest.Create("https://www.pollens.fr/");
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            reader.Close();
            response.Close();

            // Extraction data
            string vigilanceMapCounties = "var vigilanceMapCounties = ";
            int start = content.IndexOf(vigilanceMapCounties) + vigilanceMapCounties.Length;
            var res = content.Substring(start, content.IndexOf("var franceMap = ") - start);//content.IndexOf("}}") - start
            var data = JObject.Parse(res).SelectToken(DepNum.ToString()).SelectToken("riskSummary");

            // Traitement HTML
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(data.ToString());

            // Récup couleurs
            HtmlNodeCollection couleurCollec = document.DocumentNode.SelectNodes("//g[rect]/rect");
            // Récup nom
            HtmlNodeCollection nomCollec = document.DocumentNode.SelectNodes("//tspan[@text-anchor='end']/text()");

            // Construction données
            List<Vegetal> vegetaux = new List<Vegetal>(couleurCollec.Count);
            for (int i = 0; i < couleurCollec.Count; i++)
            {
                // TODO : gerer la correspondance, via Y ? (couleurCollec[i].Attributes)
                vegetaux.Add(new Vegetal(((HtmlTextNode)nomCollec[i]).Text.Trim(), GetColor(couleurCollec[i].Attributes["style"].Value)));
            }

            PackageHost.PushStateObject("Pollens", vegetaux, metadatas: new Dictionary<string, object> { ["Departement"] = DepNum }, lifetime: RefreshInterval * 2);
        }

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

            return color;
        }
    }
}

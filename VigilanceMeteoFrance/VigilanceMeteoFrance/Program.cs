using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using VigilanceMeteoFrance.Models;

namespace VigilanceMeteoFrance
{
    public class Program : PackageBase
    {
        /// <summary>
        /// Refresh rate of the vigilance.
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// Departements' number to watch.
        /// </summary>
        public string Departements { get; set; }

        /// <summary>
        /// Url from meteo france to get vigilances.
        /// </summary>
        public string Url { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            this.RefreshInterval = PackageHost.GetSettingValue<int>("RefreshInterval");
            this.Departements = PackageHost.GetSettingValue<string>("Departements");
            Departements = Departements.Replace(" ", String.Empty);
            this.Url = PackageHost.GetSettingValue<string>("Url");
            int wait = RefreshInterval * 60000;
            while (PackageHost.IsRunning)
            {
                var departements = Departements.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string departement in departements)
                {
                    int id = Convert.ToInt32(departement);
                    Vigilance vigilance = GetVigilance(id);
                    PackageHost.WriteInfo("Getting vigilance for departement {0}", id);
                    PackageHost.PushStateObject<Vigilance>(departement, vigilance);
                }
                Thread.Sleep(wait);
            }
        }

        /// <summary>
        /// Get vigilance for a departement.
        /// </summary>
        /// <param name="Departement">Departement number.</param>
        /// <returns>Vigilances for the departement</returns>
        [MessageCallback(Key = "GetVigilance")]
        Vigilance GetVigilance(int Departement)
        {
            string url = this.Url;
            Vigilance vigilance = new Vigilance();
            string risque = "";

            if (Departement == 92 || Departement == 93 || Departement == 94)
            {
                Departement = 75;
            }

            var departement = Departement.ToString();
            var doc = XDocument.Load(url);
            var depnode = doc.Descendants("risque").Where(c => c.Parent.Attribute("dep").Value == departement);

            if (depnode.Any())
            {
                List<VigilanceMeteoFrance.Models.Type> Type = new List<VigilanceMeteoFrance.Models.Type>();
                foreach (var x in depnode)
                {
                    switch (x.Attribute("val").Value)
                    {
                        case "1":
                            risque = "vent";
                            break;
                        case "2":
                            risque = "pluie-inondation";
                            break;
                        case "3":
                            risque = "orages";
                            break;
                        case "4":
                            risque = "inondations";
                            break;
                        case "5":
                            risque = "neige-verglas";
                            break;
                        case "6":
                            risque = "canicule";
                            break;
                        case "7":
                            risque = "grand-froid";
                            break;
                        case "8":
                            risque = "avalanches";
                            break;
                        case "9":
                            risque = "vagues-submersion";
                            break;
                        default:
                            risque = "aucun";
                            break;
                    }
                    Type.Add(new VigilanceMeteoFrance.Models.Type() { Name = risque });
                }
                return vigilance;
            }
            else
            {
                var bookNodes = doc.Descendants("DV").Where(c => c.Attribute("dep").Value == departement);
                List<VigilanceMeteoFrance.Models.Type> Type = new List<VigilanceMeteoFrance.Models.Type>();
                return vigilance;
            }
        }
    }
}
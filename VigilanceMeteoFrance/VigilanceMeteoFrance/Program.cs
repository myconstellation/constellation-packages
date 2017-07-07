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

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            
            int nbSeconde = 0;
            string Departements = null;
            int wait = 60;

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (nbSeconde == 0)
                    {
                        if (!PackageHost.TryGetSettingValue<string>("Departements", out Departements))
                        {
                            PackageHost.WriteError("Impossible de récupérer le setting 'Departements' en string");
                        }
                        Departements = Departements.Replace(" ", String.Empty);
                        var departements = Departements.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string departement in departements)
                        {
                            PackageHost.WriteInfo("Getting vigilances for departement {0}", departement);
                            try
                            {
                                int id = Convert.ToInt32(departement);
                                Vigilance vigilance = GetVigilance(id);
                                PackageHost.PushStateObject<Vigilance>(departement, vigilance);
                                PackageHost.WriteInfo("Vigilances for departement {0} done", departement);
                            }
                            catch (Exception ex)
                            {
                                PackageHost.WriteError("Unable to get vigilances for departement {0} : {1}", departement, ex.Message);
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
        /// Get vigilance for a departement.
        /// </summary>
        /// <param name="Departement">Departement number.</param>
        /// <returns>Vigilances for the departement</returns>
        [MessageCallback]
        public Vigilance GetVigilance(int Departement)
        {
            string Url = null;
            if (!PackageHost.TryGetSettingValue<string>("Url", out Url))
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'Url' en string");
            }
            Vigilance vigilance = new Vigilance();
            string risque = "";

            if (Departement == 92 || Departement == 93 || Departement == 94)
            {
                Departement = 75;
            }

            var departement = Departement.ToString();
            var doc = XDocument.Load(Url);
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
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
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
            int nbSeconde = 0;

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (nbSeconde == 0)
                    {
                        if (PackageHost.TryGetSettingAsXmlDocument("Departements", out XmlDocument Departements))
                        {
                            foreach (XmlNode departement in Departements.ChildNodes[0])
                            {                               
                                if (departement.Name == "departement")
                                {
                                    PackageHost.WriteInfo("Getting vigilances for departement {0}", departement.Attributes["number"].Value);
                                    try
                                    {
                                        int id = Convert.ToInt32(departement.Attributes["number"].Value);
                                        Vigilance vigilance = GetVigilance(id);
                                        PackageHost.PushStateObject<Vigilance>(departement.Attributes["number"].Value, vigilance);
                                        PackageHost.WriteInfo("Vigilances for departement {0} done", departement.Attributes["number"].Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        PackageHost.WriteError("Unable to get vigilances for departement {0} : {1}", departement.Attributes["number"].Value, ex.Message);
                                    }
                                }
                            }                          
                        }
                        else
                        {
                            PackageHost.WriteError("Impossible de récupérer le setting 'Departements' en xml");
                        }
                    }
                    Thread.Sleep(1000);
                    nbSeconde++;

                    int wait = PackageHost.GetSettingValue<int>("RefreshInterval") * 60;

                    if (nbSeconde == wait)
                    {
                        nbSeconde = 0;
                    }
                }
            });

            PackageHost.WriteInfo("VigilanceMeteoFrance is started !");
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
                int parent = 0;
                foreach (var x in depnode)
                {

                    parent = Int32.Parse(x.Parent.Attribute("coul").Value);
                    
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
                vigilance.Type = Type;
                vigilance.Level = parent;
                return vigilance;
            }
            else
            {
                var bookNodes = doc.Descendants("DV").Where(c => c.Attribute("dep").Value == departement);
                List<VigilanceMeteoFrance.Models.Type> Type = new List<VigilanceMeteoFrance.Models.Type>();
                vigilance.Type = Type;
                return vigilance;
            }
        }
    }
}
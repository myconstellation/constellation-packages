using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public string Departement { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            this.RefreshInterval = PackageHost.GetSettingValue<int>("RefreshInterval");
            this.Departement = PackageHost.GetSettingValue<string>("Departement");
            if (Departement!="00")
            {
                Task.Factory.StartNew(() =>
                {
                    while (PackageHost.IsRunning)
                    {
                        var elements = Departement.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string items in elements)
                        {
                            Vigilance Test = GetVigilance(items);
                            PackageHost.PushStateObject<Vigilance>(items, Test);
                        }
                        int wait = RefreshInterval * 60000;
                        Thread.Sleep(wait);
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Get vigilance for a departement.
        /// </summary>
        [MessageCallback]
        public Vigilance GetVigilance(string Departement)
        {
            string url = @"http://vigilance.meteofrance.com/data/NXFR33_LFPW_.xml";
            Vigilance Test = new Vigilance();
            string risque = "";
            if (Departement == "92" || Departement == "93" || Departement == "94")
            {
                Departement = "75";
            }
            var doc = XDocument.Load(url);
            var depnode = doc.Descendants("risque").Where(c => c.Parent.Attribute("dep").Value == Departement);
            if (depnode.Any())
            {
                List<Type> Type = new List<Type>();
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
                    Type.Add(new Type() { Name = risque });
                    Test.Level = Int32.Parse(x.Parent.Attribute("coul").Value);
                    PackageHost.WriteInfo("Vigilance {0} niveau {1} pour le département {2}", risque, Test.Level, Departement);
                }
                Test.Type = Type;

                return Test;
            }
            else
            {
                var bookNodes = doc.Descendants("DV").Where(c => c.Attribute("dep").Value == Departement);
                List<Type> Type = new List<Type>();
                foreach (var x in bookNodes)
                {
                    Test.Level = Int32.Parse(x.Attribute("coul").Value);
                }
                Test.Type = null;
                PackageHost.WriteInfo("Vigilance niveau {0} pour le département {1}", Test.Level, Departement);
                return Test;
            }
        }

        /// <summary>
        /// Vigilance.
        /// </summary>
        [StateObject]
        public class Vigilance
        {
            /// <summary>
            /// Level for the departement.
            /// </summary>
            public int Level { get; set; }
            /// <summary>
            /// List of the vigilance.
            /// </summary>
            public List<Type> Type { get; set; }
        }

        /// <summary>
        /// Vigilance list.
        /// </summary>
        [StateObject]
        public class Type
        {
            /// <summary>
            /// Name of the vigilance
            /// </summary>
            public string Name { get; set; }
        }
    }
}

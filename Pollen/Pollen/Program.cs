using Constellation.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Timers;

namespace Pollen
{
    public class Program : PackageBase
    {
        /// <summary>
        /// Le numéro de département
        /// </summary>
        static int depNum
        {
            get { return PackageHost.GetSettingValue<int>("DepartementNumber"); }
        }

        /// <summary>
        /// Source d'information
        /// </summary>
        static string urlSource
        {
            get { return $"http://internationalragweedsociety.org/vigilance/d%20{depNum}.gif"; }
        }

        /// <summary>
        /// Nom du fichier
        /// </summary>
        static private string fileName
        {
            get { return $"d{depNum}.gif"; }
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
            // puis toutes les 3h
            Timer syncTimer = new Timer(1000 * 3600 * 3);
            syncTimer.Elapsed += (source, e) => { SetPollen(); };
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
        }

        private static void SetPollen()
        {
            // Suppression du fichier s'il existe
            if (File.Exists(fileName))
            {
                PackageHost.WriteInfo("Fichier existant : on le supprime");
                File.Delete(fileName);
            }

            // Telechargement du fichier
            WebClient client = new WebClient();
            Uri uri = new Uri(urlSource);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
            client.DownloadFileAsync(uri, fileName);
        }

        /// <summary>
        /// Lecture de l'image et ajout des infos dans un state object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            PackageHost.WriteInfo("Mise à jours des données sur les risques du pollen");

            List<Vegetal> vegetaux = new List<Vegetal>(19);

            // Construction des données
            Bitmap img = new Bitmap(fileName);
            vegetaux.Add(new Vegetal("Cupressacees", img.GetPixel(126, 46).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Noisetier", img.GetPixel(126, 67).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Aulne", img.GetPixel(126, 87).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Peuplier", img.GetPixel(126, 107).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Saule", img.GetPixel(126, 126).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Frene", img.GetPixel(126, 147).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Charme", img.GetPixel(126, 165).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Bouleau", img.GetPixel(126, 186).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Platane", img.GetPixel(126, 205).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Chene", img.GetPixel(126, 226).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Oliver", img.GetPixel(126, 246).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Tilleul", img.GetPixel(126, 265).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Chataignier", img.GetPixel(126, 286).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Rumex", img.GetPixel(126, 303).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Graminees", img.GetPixel(126, 323).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Plantain", img.GetPixel(126, 345).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Articacees", img.GetPixel(126, 364).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Armoises", img.GetPixel(126, 382).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Ambroisie", img.GetPixel(126, 402).Name.Remove(0, 2)));
            img.Dispose();

            //vegetaux.ForEach(delegate (Vegetal veg)
            //{
            //    Console.WriteLine(veg.ToString());
            //});

            PackageHost.PushStateObject("Pollens", vegetaux);
        }
    }
}

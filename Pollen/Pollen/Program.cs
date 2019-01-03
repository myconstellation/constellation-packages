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
        /// Source d'information
        /// </summary>
        static string UrlSource
        {
            get { return $"http://internationalragweedsociety.org/vigilance/d%20{DepNum}.gif"; }
        }

        /// <summary>
        /// Nom du fichier
        /// </summary>
        static private string FileName
        {
            get { return $"d{DepNum}.gif"; }
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
            // Suppression du fichier s'il existe
            if (File.Exists(FileName))
            {
                if (Log) PackageHost.WriteInfo("Fichier existant : on le supprime");
                File.Delete(FileName);
            }

            // Telechargement du fichier
            WebClient client = new WebClient();
            Uri uri = new Uri(UrlSource);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
            client.DownloadFileAsync(uri, FileName);
        }

        /// <summary>
        /// Lecture de l'image et ajout des infos dans un state object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (Log) PackageHost.WriteInfo("Mise à jours des données sur les risques du pollen");

            List<Vegetal> vegetaux = new List<Vegetal>(19);

            int x = 126;
            int y = 46;
            decimal i = 19;

            Bitmap img = new Bitmap(FileName);

            var imageWidth = img.Width;
            if (imageWidth < 150)
            {
                x = 80;
                y = 30;
                i = 12.5M;
            }

            vegetaux.Add(new Vegetal("Cupressacees", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Noisetier", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Aulne", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Peuplier", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Saule", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Frene", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Charme", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Bouleau", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Platane", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Chene", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Oliver", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Tilleul", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Chataignier", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Rumex", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Graminees", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Plantain", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Articacees", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Armoises", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));
            vegetaux.Add(new Vegetal("Ambroisie", img.GetPixel(x, (int)Math.Ceiling(y + i * vegetaux.Count)).Name.Remove(0, 2)));

            img.Dispose();

            PackageHost.PushStateObject("Pollens", vegetaux, metadatas: new Dictionary<string, object> { ["Departement"] = DepNum }, lifetime: RefreshInterval * 2);
        }
    }
}

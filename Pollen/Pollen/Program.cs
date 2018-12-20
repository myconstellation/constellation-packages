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

            int x = 126;
            int y = 46;
            decimal i = 19;
            
            Bitmap img = new Bitmap(fileName);

            var imageWidth = img.Width;
            if(imageWidth < 150)
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


            //vegetaux.Add(new Vegetal("Cupressacees", img.GetPixel(x, 46).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Noisetier", img.GetPixel(x, 67).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Aulne", img.GetPixel(x, 87).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Peuplier", img.GetPixel(x, 107).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Saule", img.GetPixel(x, 126).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Frene", img.GetPixel(x, 147).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Charme", img.GetPixel(x, 165).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Bouleau", img.GetPixel(x, 186).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Platane", img.GetPixel(x, 205).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Chene", img.GetPixel(x, 226).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Oliver", img.GetPixel(x, 246).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Tilleul", img.GetPixel(x, 265).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Chataignier", img.GetPixel(x, 286).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Rumex", img.GetPixel(x, 303).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Graminees", img.GetPixel(x, 323).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Plantain", img.GetPixel(x, 345).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Articacees", img.GetPixel(x, 364).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Armoises", img.GetPixel(x, 382).Name.Remove(0, 2)));
            //vegetaux.Add(new Vegetal("Ambroisie", img.GetPixel(x, 402).Name.Remove(0, 2)));
            img.Dispose();

            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(vegetaux);

            PackageHost.PushStateObject("Pollens", vegetaux);
        }
    }
}

using Constellation.Package;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EcoPellets
{
    public class Program : PackageBase
    {
        /// <summary>
        /// Url de l'api
        /// </summary>
        static string baseUrl = "https://www.ecopellets.fr/";

        /// <summary>
        /// Client pour interroger l'api
        /// </summary>
        static HttpClient client = new HttpClient();

        /// <summary>
        /// Identifiant pour se connecter a l'api d'ecopellets
        /// </summary>
        static string uniqId
        {
            get { return PackageHost.GetSettingValue<string>("UniqueId"); }
        }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            RefreshData();
        }

        #region MessageCallback

        [MessageCallback]
        static void RefreshData()
        {
            StockCourant().GetAwaiter().GetResult();
            ConsommationMoisCourant().GetAwaiter().GetResult();
            ConsommationMoisPrecedent().GetAwaiter().GetResult();
            Tendance().GetAwaiter().GetResult();
            NombreJoursAvantEpuisementStock().GetAwaiter().GetResult();
        }

        [MessageCallback]
        /// <summary>
        /// Ajoute la consommation d'un ou plusieur sacs sac
        /// </summary>
        static void ConsoSac()
        {
            ConsoSacAsync().GetAwaiter().GetResult();
            PackageHost.WriteInfo("EcoPellets : 1 sac consommé");
        }

        [MessageCallback]
        /// <summary>
        /// Ajoute une commande de sacs
        /// </summary>
        /// <param name="nbSac">Nombre de sacs</param>
        /// <param name="prix">Prix DU sac</param>
        static void CommandeSacs(int nbSac, float prix)
        {
            CommandeSacsAsync(nbSac, prix).GetAwaiter().GetResult();
            PackageHost.WriteInfo("EcoPellets : {0} Sacs commandés à {1}", nbSac, prix);
        }

        [MessageCallback]
        /// <summary>
        /// Ajoute une netoyage de l'insert
        /// </summary>
        static void AjoutNetoyage()
        {
            AjoutNetoyageAsync().GetAwaiter().GetResult();
            PackageHost.WriteInfo("EcoPellets : Merci d'avoir netoyé votre brasero ;)");
        }

        #endregion

        #region PRIVATE

        #region SET

        /// <summary>
        /// Ajout un sac consommé
        /// </summary>
        private static async Task ConsoSacAsync()
        {
            await Request($"{baseUrl}consopellet.php?uniqid={uniqId}&consopellet=1");
        }

        /// <summary>
        /// Ajoute les infos d'une commandes de sacs
        /// </summary>
        /// <param name="nb">Nombre de sacs</param>
        /// <param name="prix">Prix DU sac</param>
        private static async Task CommandeSacsAsync(int nb, float prix)
        {
            await Request($"{baseUrl}addpellet.php?uniqid={uniqId}&nombre={nb}&prix={prix}");
        }

        /// <summary>
        /// Ajoute une netoyage de l'insert
        /// </summary>
        private static async Task AjoutNetoyageAsync()
        {
            await Request($"{baseUrl}entretien.php?uniqid={uniqId}");
        }

        #endregion

        #region GET

        /// <summary>
        /// Récupère le stock courant
        /// </summary>
        /// <returns>le stock courant</returns>
        private static async Task StockCourant()
        {
            PushStateObject("StockCourant", await Request($"{baseUrl}recup.php?uniqid={uniqId}&info=qtestock"));
        }

        /// <summary>
        /// Donne la consommation du mois courant
        /// </summary>
        /// <returns>la consommation du mois courant</returns>
        private static async Task ConsommationMoisCourant()
        {
            PushStateObject("ConsommationMoisCourant", await Request($"{baseUrl}recup.php?uniqid={uniqId}&info=qtemois"));
        }

        /// <summary>
        /// Donne la consommation du mois précédent
        /// </summary>
        /// <returns>la consommation du mois précédent</returns>
        private static async Task ConsommationMoisPrecedent()
        {
            PushStateObject("ConsommationMoisPrecedent", await Request($"{baseUrl}recup.php?uniqid={uniqId}&info=qtelastmonth"));
        }

        /// <summary>
        /// Donne la tendance de consommation des 7 derniers jours
        /// </summary>
        /// <returns>la tendance de consommation des 7 derniers jours</returns>
        private static async Task Tendance()
        {
            PushStateObject("Tendance", await Request($"{baseUrl}recup.php?uniqid={uniqId}&info=tendance"));
        }

        /// <summary>
        /// Donne une estimation du nombre de jours avant l'épuisement du stock
        /// </summary>
        /// <returns>une estimation du nombre de jours avant l'épuisement du stock</returns>
        private static async Task NombreJoursAvantEpuisementStock()
        {
            PushStateObject("NbJoursAvantEpuisementStock", await Request($"{baseUrl}recup.php?uniqid={uniqId}&info=stock0"));
        }

        #endregion

        /// <summary>
        /// Interrogation de l'api
        /// </summary>
        /// <param name="path">url</param>
        /// <returns>Réponse de l'api sous forme de chaine</returns>
        private static async Task<string> Request(string path)
        {
            string res = string.Empty;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsStringAsync();
            }

            return res;
        }

        /// <summary>
        /// Ajout du state object
        /// </summary>
        /// <param name="nom">Nom du SO</param>
        /// <param name="val">Valeur du SO</param>
        private static void PushStateObject(string nom, object val)
        {
            PackageHost.WriteInfo("Mise à jours EcoPellets => {0} : {1}", nom, val);
            PackageHost.PushStateObject(nom, val);
        }

        #endregion
    }
}

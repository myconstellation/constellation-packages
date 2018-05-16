using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetaSeries
{
    /// <summary>
    /// BetaSeries constellation Package
    /// </summary>
    public class Program : PackageBase
    {
        /// <summary>
        /// connection state
        /// </summary>
        private static bool _connected = false;

        /// <summary>
        /// user connection token
        /// </summary>
        private static string _userToken = null;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// On Start
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            string developerKey = PackageHost.GetSettingValue<string>("DeveloperKey");
            string login = PackageHost.GetSettingValue<string>("Login");
            string password = PackageHost.GetSettingValue<string>("Password");

            //register the developer key to be used by the API
            Net.RestHelper.RegisterDeveloperKey(developerKey);

            Task.Factory.StartNew(async () =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        dynamic auth = null;

                        if (!_connected)
                        {
                            //connection
                            auth = await BetaSeries.Net.Models.MEMBERS.Auth.Connect(login, password);

                            if (auth != null)
                            {
                                _connected = true;
                                _userToken = auth.user.id.ToString();
                                PackageHost.WriteInfo("Connection to BetaSeries established.");
                            }
                        }

                        if (_connected)
                        {
                            dynamic planning = GetPlanning();

                            if(planning != null)
                            {
                                //push state object
                                PackageHost.PushStateObject("Planning", planning);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        PackageHost.WriteError($"An error occurred while fetching BetaSeries planning : {ex.Message}");
                        PackageHost.WriteError(ex);
                    }

                    //wait
                    await Task.Delay(PackageHost.GetSettingValue<int>("PlanningPullInterval") * 60 * 1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Get the user's planning
        /// </summary>
        [MessageCallback]
        public dynamic GetPlanning(bool unseenOnly = false)
        {
            dynamic parameters = null;

            if (unseenOnly)
            {
                parameters = new { id = _userToken, unseen = true };
            }
            else
            {
                parameters = new { id = _userToken};
            }

            dynamic planning = Net.Models.PLANNING.Member.Get(parameters).Result;

            PackageHost.WriteInfo("BetaSeries planning fetched.");

            return planning?.episodes;
        }

        /// <summary>
        /// Mark an Episode as Seen
        /// </summary>
        /// <param name="id">Id of the episode</param>
        /// <param name="bulk">Mark all previous episodes as seen (default: true)</param>
        /// <returns></returns>
        [MessageCallback]
        public bool MarkEpisodeAsSeen(string id, bool bulk = true)
        {
            try
            {
                var response = Net.Models.EPISODES.Watched.Post(new { id, bulk }).Result;

                PackageHost.WriteInfo($"Episode '{id}' marked as seen.");

                return response != null && response.errors.Count == 0;
            }
            catch(Exception ex)
            {
                PackageHost.WriteError($"An error occurred while seeing episode '{id}' : {ex.Message}");
            }

            return false;
        }
    }
}

using Constellation.Package;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Waze.Models;
using Newtonsoft.Json.Serialization;

namespace Waze
{
    /// <summary>
    /// Waze Constellation Package
    /// </summary>
    public class Program : PackageBase
    {
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private const string COORDINATESFORMAT = "0.0000000";

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Package Start
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            //GetTraffic(3.066667, 50.633333, 2.3522219000000177, 48.856614);
        }

        [MessageCallback]
        List<Route> GetTraffic(double start_longitude, double start_latitude, double finish_longitude, double finish_latitude)
        {
            string baseUrl = "https://www.waze.com/row-RoutingManager/routingRequest";
            string startLongitude = start_longitude.ToString(COORDINATESFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            string startLatitude = start_latitude.ToString(COORDINATESFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            string finishLongitude = finish_longitude.ToString(COORDINATESFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            string finishLatitude = finish_latitude.ToString(COORDINATESFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            string addressUrl = $"{baseUrl}?from=x%3A{startLongitude}+y%3A{startLatitude}&to=x%3A{finishLongitude}+y%3A{finishLatitude}&at=0&returnJSON=true&timeout=60000&nPaths=3&options=AVOID_TRAILS%3At";
            RoutingRequestResponse response = null;

            //get response from waze
            using (WebClient webClient = new WebClient())
            {
                //user-agent, required to avoid a 403 HTTP Respopnse from waze
                webClient.Headers.Add("user-agent", "Mozilla/5.0");
                //referer, required to avoid a 403 HTTP Respopnse from waze
                webClient.Headers.Add("Referer", "https://www.waze.com");
                //force encoding in order to avoid accents problems
                webClient.Encoding = System.Text.Encoding.UTF8;

                string result = webClient.DownloadString(addressUrl);
                response = JsonConvert.DeserializeObject<RoutingRequestResponse>(result, this.jsonSerializerSettings);
            }

            //compute routes
            List<Route> routes = new List<Route>();
            foreach (var pair in response.Alternatives)
            {
                int sumTime = 0;
                int sumRealTime = 0;

                foreach (Result result in pair.Response.Results)
                {
                    sumTime += result.CrossTimeWithoutRealTime;
                    sumRealTime += result.CrossTime;
                }

                Route route = new Route()
                {
                    Path = pair.Response.RouteName,
                    Time = sumTime / 60,
                    RealTime = sumRealTime / 60,
                    RouteType = pair.Response.RouteType[0]
                };

                PackageHost.WriteInfo("{0} : {1}", pair.Response.RouteName, route.RealTime);

                routes.Add(route);
            }

            return routes;

        }


    }
}

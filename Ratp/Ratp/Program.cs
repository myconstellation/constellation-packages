using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ratp
{
    public class Program : PackageBase
    {

        public class Traffic
        {
            public Response_traffic response { get; set; }
            public Meta _meta { get; set; }
        }

        public class Response_traffic
        {
            public string line { get; set; }
            public string slug { get; set; }
            public string title { get; set; }
            public string message { get; set; }
        }

        public class Meta
        {
            public string version { get; set; }
            public string date { get; set; }
            public string call { get; set; }
        }

        public class Horaires
        {
            public Response_horaires response { get; set; }
            public Meta _meta { get; set; }
        }

        public class Response_horaires
        {
            public Informations informations { get; set; }
            public List<Schedule> schedules { get; set; }
        }

        public class Informations
        {
            public Destination destination { get; set; }
            public string line { get; set; }
            public string type { get; set; }
            public Station station { get; set; }
        }

        public class Schedule
        {
            public string id { get; set; }
            public string destination { get; set; }
            public string message { get; set; }
        }

        public class Destination
        {
            public string id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Station
        {
            public string id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
        }

        [MessageCallback]
        Response_traffic GetTraffic(string type, string line)
        {

            string AddressURL = "http://api-ratp.pierre-grimaud.fr/v2/traffic/" + type + "s/" + line;
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Only a test!");
            var result = webClient.DownloadString(AddressURL);
            var root = JsonConvert.DeserializeObject<Traffic>(result);
            string traffic = root.response.message;
            PackageHost.WriteInfo(traffic);
            return root.response;

        }

        [MessageCallback]
        List<Schedule> GetSchedule(string type, string line, string station, string direction)
        {

            string AddressURL = "http://api-ratp.pierre-grimaud.fr/v2/" + type + "s/" + line + "/stations/" + station + "?destination=" + direction;
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Only a test!");
            var result = webClient.DownloadString(AddressURL);
            var root = JsonConvert.DeserializeObject<Horaires>(result);
            foreach (var train in root.response.schedules)
            {
                PackageHost.WriteInfo("{0} {1} en direction de {2} : {3}", type, line, train.destination, train.message);
            }
            return root.response.schedules;

        }

    }
}

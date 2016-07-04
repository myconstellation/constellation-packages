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

namespace Waze
{
    public class Program : PackageBase
    {

        public class Path
        {
            public int segmentId { get; set; }
            public int nodeId { get; set; }
            public double x { get; set; }
            public double y { get; set; }
        }

        public class Instruction
        {
            public string opcode { get; set; }
            public int arg { get; set; }
            public object instructionText { get; set; }
            public object name { get; set; }
            public object tts { get; set; }
        }

        public class Result
        {
            public Path path { get; set; }
            public int street { get; set; }
            public object altStreets { get; set; }
            public int distance { get; set; }
            public int length { get; set; }
            public int crossTime { get; set; }
            public int crossTimeWithoutRealTime { get; set; }
            public object tiles { get; set; }
            public object clientIds { get; set; }
            public Instruction instruction { get; set; }
            public bool knownDirection { get; set; }
            public int penalty { get; set; }
            public int roadType { get; set; }
            public object additionalInstruction { get; set; }
            public bool isToll { get; set; }
            public object naiveRoute { get; set; }
            public int detourSavings { get; set; }
            public int detourSavingsNoRT { get; set; }
            public bool useHovLane { get; set; }
        }

        public class Response
        {
            public List<Result> results { get; set; }
            public List<string> streetNames { get; set; }
            public List<object> tileIds { get; set; }
            public List<object> tileUpdateTimes { get; set; }
            public object geom { get; set; }
            public double fromFraction { get; set; }
            public double toFraction { get; set; }
            public bool sameFromSegment { get; set; }
            public bool sameToSegment { get; set; }
            public object astarPoints { get; set; }
            public object wayPointIndexes { get; set; }
            public object wayPointFractions { get; set; }
            public int tollMeters { get; set; }
            public int preferedRouteId { get; set; }
            public bool isInvalid { get; set; }
            public bool isBlocked { get; set; }
            public string serverUniqueId { get; set; }
            public bool displayRoute { get; set; }
            public int astarVisited { get; set; }
            public string astarResult { get; set; }
            public object astarData { get; set; }
            public bool isRestricted { get; set; }
            public string avoidStatus { get; set; }
            public object dueToOverride { get; set; }
            public List<string> routeType { get; set; }
            public List<object> routeAttr { get; set; }
            public int astarCost { get; set; }
            public object segGeoms { get; set; }
            public string routeName { get; set; }
            public List<int> routeNameStreetIds { get; set; }
        }

        public class Coord
        {
            public double x { get; set; }
            public double y { get; set; }
            public string z { get; set; }
        }

        public class Alternative
        {
            public Response response { get; set; }
            public List<Coord> coords { get; set; }
            public object segCoords { get; set; }
        }

        public class RootObject
        {
            public List<Alternative> alternatives { get; set; }
        }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
            //PackageHost.Start<Demo>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
        }

        [MessageCallback]
        List<Trajets> GetTraffic(double start_longitude, double start_latitude, double finish_longitude, double finish_latitude)
        {

            string AddressURL = "https://www.waze.com/row-RoutingManager/routingRequest?from=x%3A" + start_longitude.ToString("0.0000000", System.Globalization.CultureInfo.InvariantCulture) + "+y%3A" + start_latitude.ToString("0.0000000", System.Globalization.CultureInfo.InvariantCulture) + "&to=x%3A" + finish_longitude.ToString("0.0000000", System.Globalization.CultureInfo.InvariantCulture) + "+y%3A" + finish_latitude.ToString("0.0000000", System.Globalization.CultureInfo.InvariantCulture) + "&at=0&returnJSON=true&timeout=60000&nPaths=3&options=AVOID_TRAILS%3At";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Only a test!");
            var result = webClient.DownloadString(AddressURL);
            var root = JsonConvert.DeserializeObject<RootObject>(result);

            List<Trajets> trajet = new List<Trajets>();
            foreach (var pair in root.alternatives)
            {
                int sumTime = 0;
                int sumRealTime = 0;
                foreach (var test in pair.response.results)
                {
                    sumTime += test.crossTimeWithoutRealTime;
                    sumRealTime += test.crossTime;
                }
                int totalTime = sumTime / 60;
                int totalRealTime = sumRealTime / 60;
                trajet.Add(new Trajets() { Path = pair.response.routeName, Time = totalTime, RealTime = totalRealTime, RouteType = pair.response.routeType[0] });

                PackageHost.WriteInfo("{0} : {1}", pair.response.routeName, totalRealTime);

            }

            return trajet;

        }

        public class Trajets
        {
            public string Path { get; set; }

            public int Time { get; set; }

            public int RealTime { get; set; }

            public string RouteType { get; set; }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{
    internal class Response
    {
        public List<Result> Results { get; set; }
        public List<string> StreetNames { get; set; }
        public List<object> TileIds { get; set; }
        public List<object> TileUpdateTimes { get; set; }
        public object Geom { get; set; }
        public double FromFraction { get; set; }
        public double ToFraction { get; set; }
        public bool SameFromSegment { get; set; }
        public bool SameToSegment { get; set; }
        public object AstarPoints { get; set; }
        public object WayPointIndexes { get; set; }
        public object WayPointFractions { get; set; }
        public int TollMeters { get; set; }
        public int PreferedRouteId { get; set; }
        public bool IsInvalid { get; set; }
        public bool IsBlocked { get; set; }
        public string ServerUniqueId { get; set; }
        public bool DisplayRoute { get; set; }
        public int AstarVisited { get; set; }
        public string AstarResult { get; set; }
        public object AstarData { get; set; }
        public bool IsRestricted { get; set; }
        public string AvoidStatus { get; set; }
        public object DueToOverride { get; set; }
        public List<string> RouteType { get; set; }
        public List<object> RouteAttr { get; set; }
        public int AstarCost { get; set; }
        public object SegGeoms { get; set; }
        public string RouteName { get; set; }
        public List<int> RouteNameStreetIds { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{
    public class Route
    {
        public string Path { get; set; }

        public int Time { get; set; }

        public int RealTime { get; set; }

        public string RouteType { get; set; }

    }
}

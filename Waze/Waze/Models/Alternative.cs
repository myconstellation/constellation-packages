using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{
    internal class Alternative
    {
        public Response Response { get; set; }
        public List<Coord> Coords { get; set; }
        public object SegCoords { get; set; }
    }
}

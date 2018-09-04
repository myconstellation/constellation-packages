using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{
    internal class Path
    {
        public int SegmentId { get; set; }
        public int NodeId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{

    internal class RoutingRequestResponse
    {
        public List<Alternative> Alternatives { get; set; }
    }
}

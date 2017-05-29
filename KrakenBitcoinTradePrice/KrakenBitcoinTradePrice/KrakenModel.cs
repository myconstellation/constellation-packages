using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBitcoinTradePrice
{
    public class KrakenRootObject
    {
        public KrakenResult result { get; set; }
    }

    public class KrakenResult
    {
        public Bitcoin XXBTZEUR { get; set; }
    }

    public class Bitcoin
    {
        public List<string> c { get; set; }
    }
}

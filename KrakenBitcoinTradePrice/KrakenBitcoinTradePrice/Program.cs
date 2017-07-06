using Constellation.Package;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace KrakenBitcoinTradePrice
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            int nbSeconde = 0;
            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (nbSeconde == 0)
                    {
                        try
                        {
                            using (var wc = new WebClient())
                            {
                                var json = wc.DownloadString("https://api.kraken.com/0/public/Ticker?pair=XXBTZEUR");
                                var currentTrade = JsonConvert.DeserializeObject<KrakenRootObject>(json);
                                decimal currentTradePrice;
                                if (decimal.TryParse(currentTrade?.result?.XXBTZEUR?.c?.First(), NumberStyles.Currency, CultureInfo.InvariantCulture, out currentTradePrice))
                                {
                                    PackageHost.WriteDebug("BitcoinCurrentTradePrice {0}", currentTradePrice);
                                    PackageHost.PushStateObject("BitcoinCurrentTradePrice", currentTradePrice, lifetime: PackageHost.GetSettingValue<int>("Interval") * 60);
                                }
                                else
                                {
                                    PackageHost.WriteWarn("Unable to retrieve value in {0}", json);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            PackageHost.WriteError(ex);
                        }

                    }
                    Thread.Sleep(1000);
                    nbSeconde++;

                    if (nbSeconde == (int)PackageHost.GetSettingValue<int>("Interval") * 60)
                    {
                        nbSeconde = 0;
                    }
                }
            });
        }
    }
}

using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using System.Globalization;

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
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            int interval = PackageHost.GetSettingValue<int>("Interval") * 60;
            while (PackageHost.IsRunning)
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        var json = wc.DownloadString("https://api.kraken.com/0/public/Ticker?pair=XXBTZEUR");
                        var currentTrade = JsonConvert.DeserializeObject<KrakenRootObject>(json);
                        decimal currentTradePrice;
                        if(decimal.TryParse(currentTrade?.result?.XXBTZEUR?.c?.First(), NumberStyles.Currency, CultureInfo.InvariantCulture,out currentTradePrice))
                        {
                            PackageHost.WriteDebug("BitcoinCurrentTradePrice {0}", currentTradePrice);
                            PackageHost.PushStateObject("BitcoinCurrentTradePrice", currentTradePrice, lifetime: interval);
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
                Thread.Sleep(interval * 1000);

            }
        }
    }
}

using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPLinkSmartHome
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

            IEnumerable<TPLinkConfig> configs = PackageHost.GetSettingAsJsonObject<IEnumerable<TPLinkConfig>>("setting");


            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        foreach (TPLinkConfig config in configs)
                        {
                            if (config.Type == TPLink.SmartHome.SystemType.PlugWithEnergyMeter)
                            {
                                TPLink.SmartHome.PlugWithEnergyMeterClient plug = new TPLink.SmartHome.PlugWithEnergyMeterClient(config.HostName);
                                TPLink.SmartHome.ConsumptionInfo consumption = plug.GetConsumption();

                                PackageHost.PushStateObject($"consumption-{config.HostName}", consumption);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError(ex);
                    }
                    finally
                    {
                        Thread.Sleep(10000);
                    }
                }
            }, TaskCreationOptions.LongRunning);


        }

    }

    public class TPLinkConfig
    {
        public string HostName { get; set; }
        public TPLink.SmartHome.SystemType Type { get; set; }

    }
}

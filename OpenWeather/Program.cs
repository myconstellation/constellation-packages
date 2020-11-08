using Constellation;
using Constellation.Package;
using OpenWeatherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenWeather
{
    public class Program : PackageBase
    {
        private OpenWeatherSection configuration = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            this.configuration = PackageHost.GetSettingAsConfigurationSection<OpenWeatherSection>("openWeatherSection", true);
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            int nbSeconde = 0;
            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    if (nbSeconde == 0)
                    {
                        foreach (StationElement station in this.configuration.Stations)
                        {
                            PackageHost.WriteInfo("Getting forecast for {0} ...", station.Name);
                            try
                            {

                                var api = new OpenWeatherAPI.API(this.configuration.ApiKey, this.configuration.Language);
                                var result = api.QueryWeather((float)station.Longitude, (float)station.Latitude);
                                PackageHost.PushStateObject<WeatherInfo>(station.Name, result, lifetime: (int)this.configuration.RefreshInterval.TotalSeconds * 2);
                                PackageHost.WriteInfo("Weather for {0} updated.", station.Name);

                                if (result != null && result.LastEx != null)
                                    throw result.LastEx;
                            }
                            catch (Exception ex)
                            {
                                PackageHost.WriteError("Unable to get the weather for {0} : {1}", station.Name, ex.ToString());
                            }
                        }
                    }
                    Thread.Sleep(1000);
                    nbSeconde++;

                    if (nbSeconde == (int)this.configuration.RefreshInterval.TotalSeconds)
                    {
                        nbSeconde = 0;
                    }
                }
            });
        }
    }
}

using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPLinkSmartHome.Models;

namespace TPLinkSmartHome
{
    /// <summary>
    /// TPLinkSmartHome Package
    /// </summary>
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// OnStart
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            if (PackageHost.TryGetSettingAsJsonObject<IEnumerable<TPLinkConfig>>("devices", out IEnumerable<TPLinkConfig> configs))
            {
                Task.Factory.StartNew(async () =>
                {
                    //pool to get devices informations
                    while (PackageHost.IsRunning)
                    {
                        int soLifeTime = Math.Max(PackageHost.GetSettingValue<int>("poolingInterval") * 2, 30000) / 1000;

                        foreach (TPLinkConfig config in configs)
                        {
                            try
                            {
                                if (config.Type == TPLink.SmartHome.SystemType.PlugWithEnergyMeter)
                                {
                                    TPLink.SmartHome.PlugWithEnergyMeterClient plug = new TPLink.SmartHome.PlugWithEnergyMeterClient(config.HostName);
                                    TPLink.SmartHome.ConsumptionInfo consumption = await plug.GetConsumptionAsync();
                                    TPLink.SmartHome.SystemInfo systemInfos = await plug.GetSystemInfoAsync();
                                    TPLink.SmartHome.OutputState state = await plug.GetOutputAsync();

                                    PlugWithEnergyMeterInformations plugInfos = PlugWithEnergyMeterInformations.CreateFromSystemInfosAndOutputStateAndConsumption(systemInfos, state, consumption);

                                    PackageHost.PushStateObject($"TPLink-{config.HostName}", plugInfos, lifetime: soLifeTime);
                                }
                                else if (config.Type == TPLink.SmartHome.SystemType.Plug)
                                {
                                    TPLink.SmartHome.PlugClient plug = new TPLink.SmartHome.PlugClient(config.HostName);
                                    TPLink.SmartHome.SystemInfo systemInfos = await plug.GetSystemInfoAsync();
                                    TPLink.SmartHome.OutputState state = await plug.GetOutputAsync();

                                    PlugInformations plugInfos = PlugInformations.CreateFromSystemInfosAndOutputState(systemInfos, state);

                                    PackageHost.PushStateObject($"TPLink-{config.HostName}", plugInfos, lifetime: soLifeTime);
                                }
                            }
                            catch (TimeoutException ex)
                            {
                                PackageHost.WriteError($"Connection timeout for TPLink device '{config?.HostName}' : {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                PackageHost.WriteError($"An unknown error has occurred for TPLink device '{config?.HostName}' : {ex}");
                            }
                        }

                        await Task.Delay(PackageHost.GetSettingValue<int>("poolingInterval"));
                    }

                }, TaskCreationOptions.LongRunning);

            }
        }

        /// <summary>
        /// Gets the daily conumption stats of a PlugWithEnergyMeter        
        /// </summary>
        /// <param name="hostname">hostname</param>
        /// <param name="year">year (default : current year)</param>
        /// <param name="month">month (default : current month)</param>
        /// <returns></returns>
        [MessageCallback]
        public dynamic GetDailyStat(string hostname, int? year = null, int? month = null)
        {
            if (!year.HasValue) year = DateTime.Now.Year;
            if (!month.HasValue) month = DateTime.Now.Month;

            return new TPLink.SmartHome.PlugWithEnergyMeterClient(hostname).ExecuteAsync(
                        "emeter",
                        "get_daystat",
                        new Newtonsoft.Json.Linq.JProperty("year", year),
                        new Newtonsoft.Json.Linq.JProperty("month", month)
                    ).Result;
        }

        /// <summary>
        /// Gets the monthly conumption stats of a PlugWithEnergyMeter
        /// </summary>
        /// <param name="hostname">hostname</param>
        /// <param name="year">year (default : current year)</param>
        /// <returns></returns>
        [MessageCallback]
        public dynamic GetMonthStat(string hostname, int? year = null)
        {
            if (!year.HasValue) year = DateTime.Now.Year;

            return new TPLink.SmartHome.PlugWithEnergyMeterClient(hostname).ExecuteAsync(
                        "emeter",
                        "get_monthstat",
                        new Newtonsoft.Json.Linq.JProperty("year", year)
                    ).Result;
        }

        /// <summary>
        /// Sets the output state of a Plug (turns on or turns off)
        /// </summary>
        /// <param name="hostname">hostname</param>
        /// <param name="state">true to turn on, false otherwise</param>
        [MessageCallback]
        public void SetOutputState(string hostname, bool state)
        {
            new TPLink.SmartHome.PlugClient(hostname).SetOutput(state ? TPLink.SmartHome.OutputState.On : TPLink.SmartHome.OutputState.Off);
        }

    }
}

namespace TPLinkSmartHome.Models
{
    /// <summary>
    /// Informations concerning a TPLink Plug with energy meter
    /// </summary>
    public class PlugWithEnergyMeterInformations : PlugInformations
    {
        /// <summary>
        /// Current consumption
        /// </summary>
        public decimal Power { get; private set; }

        /// <summary>
        /// Current voltage
        /// </summary>
        public decimal Voltage { get; private set; }

        /// <summary>
        /// Consumption of the day (in kw/h)
        /// </summary>
        public decimal ConsumptionOfTheDay { get; private set; }

        internal static PlugWithEnergyMeterInformations CreateFromSystemInfosAndOutputStateAndConsumption(TPLink.SmartHome.SystemInfo sysInfo, TPLink.SmartHome.OutputState state, TPLink.SmartHome.ConsumptionInfo consumptionInfo)
        {
            var infos = new PlugWithEnergyMeterInformations();
            infos.FillWithSystemInfosAndOutputStateAndConsumption(sysInfo, state, consumptionInfo);

            return infos;
        }

        internal void FillWithSystemInfosAndOutputStateAndConsumption(TPLink.SmartHome.SystemInfo sysInfo, TPLink.SmartHome.OutputState state, TPLink.SmartHome.ConsumptionInfo consumptionInfo)
        {
            Power = consumptionInfo.Power;
            Voltage = consumptionInfo.Voltage;
            ConsumptionOfTheDay = consumptionInfo.Current;
            this.FillWithSystemInfosAndOutputState(sysInfo, state);
        }
    }
}

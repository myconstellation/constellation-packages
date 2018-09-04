using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Capteur température et humidité
    /// </summary>
    [StateObject]
    public class Weather : Equipment
    {
        /// <summary>
        /// Temperature
        /// </summary>
        public int? Temperature { get; set; }

        /// <summary>
        /// Humidity
        /// </summary>
        public int? Humidity { get; set; }

        /// <summary>
        /// Pressure
        /// </summary>
        public int? Pressure { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Weather()
        {
            base.Battery = BatteryType.CR1632;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            base.Update(data, cmdType);

            Weather curData = data as Weather;

            if (curData.Temperature.HasValue)
            {
                this.Temperature = curData.Temperature;
            }

            if (curData.Humidity.HasValue)
            {
                this.Humidity = curData.Humidity;
            }

            if (curData.Pressure.HasValue)
            {
                this.Pressure = curData.Pressure;
            }
        }
    }
}

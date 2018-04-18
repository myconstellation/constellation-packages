using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Humidity / Temperature sensor
    /// </summary>
    [StateObject]
    public class SensorHT : Equipment
    {
        /// <summary>
        /// Temperature level in ºC.
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// Humidity level in %.
        /// </summary>
        public int Humidity { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public SensorHT()
        {
            base.Battery = BatteryType.CR2032;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data)
        {
            SensorHT curData = data as SensorHT;
            if (curData.Voltage != default(int))
            {
                this.Voltage = curData.Voltage;
                this.BatteryLevel = base.ParseVoltage(curData.Voltage);
            }

            this.Status = curData.Status;
            this.Temperature = curData.Temperature;
            this.Humidity = curData.Humidity;
        }
    }
}

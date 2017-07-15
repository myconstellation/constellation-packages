using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Humidity / Temperature sensor
    /// </summary>
    [StateObject, XiaomiEquipement("sensor_ht")]
    public class SensorHT
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "sensor_ht";

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public string Battery { get; set; } = "CR2032";

        /// <summary>
        /// Battery level.
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public SensorHTReport Report { get; set; }
    }

    /// <summary>
    /// Humidity / Temperature sensor last report
    /// </summary>
    [StateObject, XiaomiEquipement("sensor_ht_report")]
    public class SensorHTReport
    {
        /// <summary>
        /// Voltage left.
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Temperature level in ºC.
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// Humidity level in %.
        /// </summary>
        public int Humidity { get; set; }

    }
}

using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Humidity / Temperature sensor
    /// </summary>
    [StateObject, XiaomiEquipement("weather.v1")]
    public class SensorHTPAq
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "weather.v1";

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string ShortId { get; set; }

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
        public SensorHTPAqReport Report { get; set; }
    }

    /// <summary>
    /// Humidity / Temperature sensor last report
    /// </summary>
    [StateObject, XiaomiEquipement("weather.v1_report")]
    public class SensorHTPAqReport
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

        /// <summary>
        /// Humidity level in %.
        /// </summary>
        public int Pressure { get; set; }
    }
}

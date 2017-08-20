using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Magnet sensor
    /// </summary>
    [StateObject, XiaomiEquipement("sensor_magnet.aq2")]
    public class MagnetAq
    {
        /// <summary>
        /// Model type
        /// </summary>
        public string Model { get; set; } = "sensor_magnet.aq2";

        /// <summary>
        /// SID (mac adress)
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type
        /// </summary>
        public string Battery { get; set; } = "CR1632";

        /// <summary>
        /// Battery level
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report
        /// </summary>
        public MagnetAqReport Report { get; set; }
    }

    /// <summary>
    /// Magnet sensor last report
    /// </summary>
    [StateObject, XiaomiEquipement("sensor_magnet.aq2_report")]
    public class MagnetAqReport
    {
        /// <summary>
        /// Voltage left
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Magnet sensor state
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        public string NoClose { get; set; }
    }
}
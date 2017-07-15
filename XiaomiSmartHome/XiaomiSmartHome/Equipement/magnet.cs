using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Magnet sensor
    /// </summary>
    [StateObject, XiaomiEquipement("magnet")]
    public class Magnet
    {
        /// <summary>
        /// Model type
        /// </summary>
        public string Model { get; set; } = "magnet";

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
        public MagnetReport Report { get; set; }

    }

    /// <summary>
    /// Magnet sensor last report
    /// </summary>
    [StateObject, XiaomiEquipement("magnet_report")]
    public class MagnetReport
    {
        /// <summary>
        /// Voltage left
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Magnet sensor state
        /// </summary>
        public string Status { get; set; }
       

    }
}

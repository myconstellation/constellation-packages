using Constellation;
using Constellation.Package;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Magnet sensor
    /// </summary>
    [StateObject]
    public class magnet
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "magnet";

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public string Battery { get; set; } = "CR1632";

        /// <summary>
        /// Battery level.
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public magnet_report Report { get; set; }

    }

    /// <summary>
    /// Magnet sensor last report
    /// </summary>
    [StateObject]
    public class magnet_report
    {
        /// <summary>
        /// Voltage left.
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Magnet sensor state.
        /// </summary>
        public string Status { get; set; }
       

    }
}

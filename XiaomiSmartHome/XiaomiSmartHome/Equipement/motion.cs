using Constellation;
using Constellation.Package;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Motuon sensor
    /// </summary>
    [StateObject]
    public class motion
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "motion";

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public string Battery { get; set; } = "CR2450";

        /// <summary>
        /// Battery level.
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public motion_report Report { get; set; }

    }

    /// <summary>
    /// Motion sensor last report
    /// </summary>
    [StateObject]
    public class motion_report
    {
        /// <summary>
        /// Voltage left.
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        public string Status { get; set; }
        

    }
}

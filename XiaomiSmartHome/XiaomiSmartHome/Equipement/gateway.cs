using Constellation.Package;
using Constellation;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Mon StateObject complexe
    /// </summary>
    [StateObject]
    public class gateway
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "gateway";

        /// <summary>
        /// Model type.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Model type.
        /// </summary>
        public string Battery { get; set; } = "Sector";

        /// <summary>
        /// Model type.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public gateway_report Report { get; set; }
    }

    /// <summary>
    /// Mon StateObject complexe2
    /// </summary>
    [StateObject]
    public class gateway_report
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int Rgb { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int Illumination { get; set; }

    }

    public class gateway_heartbeat
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public string IP { get; set; }


    }
}

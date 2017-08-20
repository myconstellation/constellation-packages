using Constellation.Package;
using Constellation;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Mon StateObject complexe
    /// </summary>
    [StateObject, XiaomiEquipement("gateway")]
    public class Gateway
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "gateway";

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public string Battery { get; set; } = "Sector";

        /// <summary>
        /// Last token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public GatewayReport Report { get; set; }
    }

    /// <summary>
    /// Gateway last report
    /// </summary>
    [StateObject, XiaomiEquipement("gateway_report")]
    public class GatewayReport
    {
        /// <summary>
        /// Led color in rgb.
        /// </summary>
        public int Rgb { get; set; }

        /// <summary>
        /// Illumination in lux.
        /// </summary>
        public int Illumination { get; set; }
    }

    public class GatewayHeartbeat
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public string IP { get; set; }
    }
}

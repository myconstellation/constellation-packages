using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Magnet sensor
    /// </summary>
    public abstract class Equipement
    {
        /// <summary>
        /// Model type
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// SID (mac adress)
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type
        /// </summary>
        public string Battery { get; set; }

        /// <summary>
        /// Battery level
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report
        /// </summary>
        public dynamic Report { get; set; }
    }
}
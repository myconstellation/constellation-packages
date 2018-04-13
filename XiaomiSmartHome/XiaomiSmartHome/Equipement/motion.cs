using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Motion sensor
    /// </summary>
    [StateObject, XiaomiEquipement(Constants.MOTION)]
    public class Motion
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = Constants.MOTION;

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Short id
        /// </summary>
        [JsonProperty("short_id")]
        public int ShortId { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public string Battery { get; set; } = Constants.CR2450;

        /// <summary>
        /// Battery level.
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public MotionReport Report { get; set; }
    }

    /// <summary>
    /// Motion sensor last report
    /// </summary>
    /// <example>
    /// {"cmd":"report","model":"motion","sid":"xxxx","short_id":xxx,"data":"{\"status\":\"motion\"}"}
    /// {"cmd":"report","model":"motion","sid":"xxxx","short_id":xxx,"data":"{\"no_motion\":\"120\"}"}
    /// ...
    /// </example>
    [StateObject, XiaomiEquipement("motion_report")]
    public class MotionReport
    {
        /// <summary>
        /// Voltage left.
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// No motion since.
        /// </summary>
        [JsonProperty("no_motion")]
        public string NoMotion { get; set; }
    }
}

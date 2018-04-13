using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Wall plug
    /// </summary>
    [StateObject, XiaomiEquipement(Constants.PLUG)]
    public class Plug
    {
        /// <summary>
        /// Model type
        /// </summary>
        public string Model { get; set; } = Constants.PLUG;

        /// <summary>
        /// SID (mac adress)
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Short id
        /// </summary>
        [JsonProperty("short_id")]
        public int ShortId { get; set; }

        /// <summary>
        /// Battery type
        /// </summary>
        public string Battery { get; set; } = Constants.SECTOR;

        /// <summary>
        /// Battery level
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report
        /// </summary>
        public PlugReport Report { get; set; }
    }

    /// <summary>
    /// Plug last report
    /// </summary>
    /// <example>
    /// {"cmd":"report","model":"plug","sid":"xxxxx","short_id":xxx,"data":"{\"status\":\"on\"}"}
    /// {"cmd":"heartbeat","model":"plug","sid":"xxxxx","short_id":xxx,"data":"{\"voltage\":3600,\"status\":\"on\",\"inuse\":\"1\",\"power_consumed\":\"20482\",\"load_power\":\"0.97\"}"}
    /// </example>
    [StateObject, XiaomiEquipement("plug_report")]
    public class PlugReport
    {
        /// <summary>
        /// Voltage left
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Plug state
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// In use
        /// </summary>
        public int InUse { get; set; }

        /// <summary>
        /// Total power consumed
        /// </summary>
        [JsonProperty("power_consumed")]
        public int PowerConsumed { get; set; }

        /// <summary>
        /// Current load power
        /// </summary>
        [JsonProperty("load_power")]
        public float LoadPower { get; set; }
    }
}
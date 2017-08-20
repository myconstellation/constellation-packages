using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Motuon sensor
    /// </summary>
    [StateObject, XiaomiEquipement("plug")]
    public class Plug
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "plug";

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
        public PlugReport Report { get; set; }
    }

    /// <summary>
    /// Motion sensor last report
    /// </summary>
    [StateObject, XiaomiEquipement("plug_report")]
    public class PlugReport
    {
        /// <summary>
        /// Voltage left.
        /// </summary>
        [JsonProperty("voltage")]
        public int Voltage { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        [JsonProperty("inuse")]
        public int InUse { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        [JsonProperty("power_consumed")]
        public float Power_Consume { get; set; }

        /// <summary>
        /// Motion sensor state.
        /// </summary>
        [JsonProperty("load_power")]
        public float Load_Power { get; set; }
    }
}

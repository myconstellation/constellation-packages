using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Windows door sensor
    /// </summary>
    [StateObject, XiaomiEquipement(Constants.MAGNET)]
    public class Magnet
    {
        /// <summary>
        /// Model type
        /// </summary>
        public string Model { get; set; } = Constants.MAGNET;

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
        public string Battery { get; set; } = Constants.CR1632;

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
    /// <example>
    /// {"cmd":"report","model":"magnet","sid":"xxxxx","short_id":xxxx,"data":"{\"status\":\"open\"}"}
    /// {"cmd":"report","model":"magnet","sid":"xxxxx","short_id":xxxx,"data":"{\"status\":\"close\"}"}
    /// {"cmd":"report","model":"magnet","sid":"xxxxx","short_id":xxxx,"data":"{\"no_close\":\"60\"}"}
    /// </example>
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

        /// <summary>
        /// Time since door / window is open
        /// </summary>
        [JsonProperty("no_close")]
        public string NoClose { get; set; }
    }
}
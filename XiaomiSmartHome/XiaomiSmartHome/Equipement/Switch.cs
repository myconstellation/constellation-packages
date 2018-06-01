using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Smart Wireless Switch
    /// </summary>
    [StateObject, XiaomiEquipement(Constants.SWITCH)]
    public class Switch
    {
        /// <summary>
        /// Model type
        /// </summary>
        public string Model { get; set; } = Constants.SWITCH;

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
        public string Battery { get; set; } = Constants.CR2450;

        /// <summary>
        /// Battery level
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report
        /// </summary>
        public SwitchReport Report { get; set; }
    }

    /// <summary>
    /// Smart Wireless Switch last report
    /// </summary>
    /// <example>
    /// {"cmd":"report","model":"switch","sid":"xxxx","short_id":xxx,"data":"{\"status\":\"click\"}"}
    /// {"cmd":"report","model":"switch","sid":"xxxx","short_id":xxx,"data":"{\"status\":\"double_click\"}"}
    /// {"cmd":"report","model":"switch","sid":"xxxx","short_id":xxx,"data":"{\"status\":\"long_click_press\"}"}
    /// {"cmd":"report","model":"switch","sid":"xxxx","short_id":xxx,"data":"{\"status\":\"long_click_release\"}"}
    /// </example>
    [StateObject, XiaomiEquipement("switch_report")]
    public class SwitchReport
    {
        /// <summary>
        /// Voltage left
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Smart Wireless Switch state
        /// </summary>
        public string Status { get; set; }
    }
}
using Constellation.Package;
using Constellation;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Drawing;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Xiaomi gateway
    /// </summary>
    [StateObject, XiaomiEquipement(Constants.GATEWAY)]
    public class Gateway
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = Constants.GATEWAY;

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
        public string Battery { get; set; } = Constants.SECTOR;

        /// <summary>
        /// Battery level
        /// </summary>
        public int BatteryLevel { get; set; }

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
    /// <example>
    /// {"cmd":"report","model":"gateway","sid":"xxxx","short_id":x,"data":"{\"rgb\":1677727487,\"illumination\":1292}"}
    /// {"cmd":"heartbeat","model":"gateway","sid":"xxxx","short_id":"x","token":"xxxx","data":"{\"ip\":\"x.x.x.x\"}"}
    /// </example>
    [StateObject, XiaomiEquipement("gateway_report")]
    public class GatewayReport
    {

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Led color in rgb.
        /// </summary>
        public int Rgb { get; set; }

        /// <summary>
        /// Illumination in lux.
        /// </summary>
        public int Illumination { get; set; }
    }
}

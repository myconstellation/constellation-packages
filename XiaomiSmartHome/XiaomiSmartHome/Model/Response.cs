using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Model
{
    /// <summary>
    /// Response from the gateway
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Response
    {
        /// <summary>
        /// Command name
        /// </summary>
        [JsonProperty("cmd")]
        public CommandType Cmd { get; set; }

        /// <summary>
        /// Model type
        /// </summary>
        [JsonProperty("model")]
        public EquipmentType Model { get; set; }

        /// <summary>
        /// SID (mac adress)
        /// </summary>
        [JsonProperty("sid")]
        public string Sid { get; set; }

        /// <summary>
        /// Short ID
        /// </summary>
        [JsonProperty("short_id")]
        public int Short_id { get; set; }

        /// <summary>
        /// Last token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Last equipement values
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}

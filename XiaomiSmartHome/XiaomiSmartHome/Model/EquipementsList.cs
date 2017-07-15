using Newtonsoft.Json;

namespace XiaomiSmartHome.Model
{

    /// <summary>
    /// Response from the gateway
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EquipementsList
    {
        /// <summary>
        /// Command name
        /// </summary>
        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        /// <summary>
        /// SID (mac adress)
        /// </summary>
        [JsonProperty("sid")]
        public string Sid { get; set; }

        /// <summary>
        /// Last token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// List of equipements Sid
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

    }

}

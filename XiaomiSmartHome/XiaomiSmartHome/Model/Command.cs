using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XiaomiSmartHome.Model
{
    /// <summary>
    /// Command to the gateway
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Command
    {
        /// <summary>
        /// Command name
        /// </summary>
        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        /// <summary>
        /// Model type
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// SID (mac adress)
        /// </summary>
        [JsonProperty("sid")]
        public string Sid { get; set; }

        /// <summary>
        /// Short ID
        /// </summary>
        [JsonProperty("short_id")]
        public int? Short_id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }
        
        /// <summary>
        /// params
        /// </summary>
        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; set; }
    }
}

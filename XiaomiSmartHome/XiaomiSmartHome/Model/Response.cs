using System;
using Newtonsoft.Json;

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
  
        /// <summary>
        /// Get type by model name in attribute
        /// </summary>
        public class XiaomiEquipementAttribute : Attribute
        {
            /// <summary>
            /// Model name
            /// </summary>
            public string Model { get; set; }
            /// <summary>
            /// Attribute value
            /// </summary>
            public XiaomiEquipementAttribute(string model)
            {
                this.Model = model;
            }
        }
    }
}

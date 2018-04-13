using Newtonsoft.Json;
using System.ComponentModel;

namespace SonyBravia
{
    public class BraviaIRCCConfig
    {
        public string Hostname { get; set; }

        [DefaultValue(80), JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int Port { get; set; }

        [DefaultValue("0000"), JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string PinCode { get; set; }
    }
}
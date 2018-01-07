using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yeelight
{
    /// <summary>
    /// Represents a bulb's configuration
    /// </summary>
    public class Bulb
    {
        /// <summary>
        /// Hostname or IP adress
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Port of the bulb (default 55443) : optional
        /// </summary>
        [System.ComponentModel.DefaultValue(55443)]
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
        public int Port { get; set; }

        /// <summary>
        /// Name of the bulb
        /// </summary>
        public string Name { get; set; }
    }
}

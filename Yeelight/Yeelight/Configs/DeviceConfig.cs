namespace Yeelight.Configs
{
    /// <summary>
    /// Configuration for a device
    /// </summary>
    internal class DeviceConfig
    {
        #region Public Properties

        /// <summary>
        /// Hostname or IP adress
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Port of the device (default 55443) : optional
        /// </summary>
        [System.ComponentModel.DefaultValue(55443)]
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
        public int Port { get; set; }

        #endregion Public Properties
    }
}
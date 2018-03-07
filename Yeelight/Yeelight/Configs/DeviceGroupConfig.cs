using System.Collections.Generic;

namespace Yeelight.Configs
{
    /// <summary>
    /// Configuration for a group of devices
    /// </summary>
    internal class DeviceGroupConfig
    {
        #region Public Properties

        /// <summary>
        /// Devices of the group
        /// </summary>
        public List<string> Devices { get; set; }

        /// <summary>
        /// name of the group
        /// </summary>
        public string Name { get; set; }

        #endregion Public Properties
    }
}
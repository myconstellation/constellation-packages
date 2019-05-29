namespace TPLinkSmartHome.Models
{
    /// <summary>
    /// Informations concerning a TPLink device
    /// </summary>
    public class PublicSystemInfo
    {
        /// <summary>
        /// Create from a native SystemInfo object
        /// </summary>
        /// <param name="sysInfo"></param>
        /// <returns></returns>
        internal static PublicSystemInfo CreateFromSystemInfos(TPLink.SmartHome.SystemInfo sysInfo)
        {
            PublicSystemInfo infos = new PublicSystemInfo();
            infos.FillWithSystemInfo(sysInfo);

            return infos;
        }
        internal void FillWithSystemInfo(TPLink.SmartHome.SystemInfo sysInfo)
        {
            Name = sysInfo.Name;
            Id = sysInfo.Id;
            FirmwareVersion = sysInfo.FirmwareVersion;
            Model = sysInfo.Model;
            Type = sysInfo.Type;
            LocationLat = sysInfo.LocationLat;
            LocationLon = sysInfo.LocationLon;
        }
        /// <summary>
        /// Name of the device (as defined in Kasa SmartHome application)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Type of the device
        /// </summary>
        public TPLink.SmartHome.SystemType Type { get; private set; }

        /// <summary>
        /// Model
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Firmware version
        /// </summary>
        public string FirmwareVersion { get; private set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public decimal LocationLat { get; private set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public decimal LocationLon { get; private set; }
    }
}

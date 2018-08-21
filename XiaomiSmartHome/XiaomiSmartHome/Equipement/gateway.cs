using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Xiaomi gateway
    /// </summary>
    [StateObject]
    public class Gateway : Equipment
    {
        /// <summary>
        /// Last token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Led color in rgb.
        /// </summary>
        public int? Rgb { get; set; }

        /// <summary>
        /// Illumination in lux.
        /// </summary>
        public int? Illumination { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Gateway()
        {
            base.Battery = BatteryType.SECTOR;
            base.BatteryLevel = 100;
            base.Voltage = 3600;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data)
        {
            Gateway curData = data as Gateway;
            if (curData.IP != default(string))
            {
                this.IP = curData.IP;
            }

            if (curData.Rgb.HasValue)
            {
                this.Rgb = curData.Rgb;
            }

            this.Illumination = curData.Illumination;
        }
    }
}

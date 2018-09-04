using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Windows door sensor
    /// </summary>
    [StateObject]
    public class Magnet : Equipment
    {
        /// <summary>
        /// Time since door / window is open
        /// </summary>
        [JsonProperty("no_close")]
        public string NoClose { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Magnet()
        {
            base.Battery = BatteryType.CR1632;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            base.Update(data, cmdType);

            Magnet curData = data as Magnet;
            this.NoClose = curData.NoClose;
        }
    }
}
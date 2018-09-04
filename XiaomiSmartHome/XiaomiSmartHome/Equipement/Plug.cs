using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Wall plug
    /// </summary>
    [StateObject]
    public class Plug : Equipment
    {
        /// <summary>
        /// In use
        /// </summary>
        public int InUse { get; set; }

        /// <summary>
        /// Total power consumed
        /// </summary>
        [JsonProperty("power_consumed")]
        public int PowerConsumed { get; set; }

        /// <summary>
        /// Current load power
        /// </summary>
        [JsonProperty("load_power")]
        public float LoadPower { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Plug()
        {
            base.Battery = BatteryType.SECTOR;
            base.BatteryLevel = 100;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            base.Update(data, cmdType);

            Plug curData = data as Plug;

            if (curData.PowerConsumed != default(int))
            {
                this.PowerConsumed = curData.PowerConsumed;
            }

            if (!string.IsNullOrWhiteSpace(curData.Status) && curData.Status.Equals("off"))
            {
                this.InUse = 0;
                this.LoadPower = 0;
            }
            else
            {
                this.InUse = curData.InUse;
                if (curData.LoadPower != default(int))
                {
                    this.LoadPower = curData.LoadPower;
                }
            }
        }
    }
}
using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Smart Wireless Switch
    /// </summary>
    [StateObject]
    public class WallSwitchOneButon : Equipment
    {
        /// <summary>
        /// Action
        /// </summary>
        [JsonProperty("channel_0")]
        public new string Status { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
        public WallSwitchOneButon()
        {
            base.Battery = BatteryType.CR2450;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data)
        {
            WallSwitchOneButon curData = data as WallSwitchOneButon;
            if (curData.Voltage != default(int))
            {
                this.Voltage = curData.Voltage;
                this.BatteryLevel = base.ParseVoltage(curData.Voltage);
            }

            this.Status = curData.Status;
        }
    }
}
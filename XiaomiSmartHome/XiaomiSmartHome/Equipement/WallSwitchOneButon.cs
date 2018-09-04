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
        [JsonProperty("channel_0")]
        private string _status { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        new public string Status { get { return _status; } }


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
        public override void Update(object data, string cmdType)
        {
            var curData = data as WallSwitchOneButon;
            this._status = curData.Status;
            base.Update(data, cmdType);
            // Peu etre les mettre après le base.update ?
        }

        /// <summary>
        /// Force channel_0 to be serialized as Status
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerialize_status()
        {
            return false;
        }
    }
}

using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Motion sensor
    /// </summary>
    [StateObject]
    public class Motion : Equipment
    {
        /// <summary>
        /// No motion since.
        /// </summary>
        [JsonProperty("no_motion")]
        public string NoMotion { get; set; }
        
        /// <summary>
        /// Ctor
        /// </summary>
        public Motion()
        {
            base.Battery = BatteryType.CR2450;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data)
        {
            Motion curData = data as Motion;
            if (curData.Voltage != default(int))
            {
                this.Voltage = curData.Voltage;
                base.BatteryLevel = base.ParseVoltage(curData.Voltage);
            }

            this.Status = curData.Status;
            this.NoMotion = curData.NoMotion;
        }
    }
}

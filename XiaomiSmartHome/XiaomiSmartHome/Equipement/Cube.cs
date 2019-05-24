using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Magic cube
    /// </summary>
    [StateObject]
    public class Cube : Equipment
    {
        /// <summary>
        /// Rotation angle, the unit is degree(°). Positive numbers indicate clockwise rotations and negative numbers indicate counterclockwise rotations.
        /// </summary>
        [JsonProperty("rotate")]
        public decimal? Rotate { get; set; }
        
        /// <summary>
        /// Ctor
        /// </summary>
        public Cube()
        {
            base.Battery = BatteryType.CR2450;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            base.Update(data, cmdType);

            var curData = data as Cube;
            this.Rotate = curData.Rotate;
        }
    }
}

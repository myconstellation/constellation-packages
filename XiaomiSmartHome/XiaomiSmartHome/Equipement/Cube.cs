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

        [JsonProperty("cube_status")]
        private string _status { get; set; }

        /// <summary>
        /// cube_status	flip90/flip180/move/tap_twice/shake_air/swing/alert/free_fall/rotate
        /// </summary>
        new public string Status { get { return _status; } }

        /// <summary>
        /// rotate_degree	Rotation angle, the unit is degree(°) . Positive numbers indicate clockwise rotations and negative numbers indicate counterclockwise rotations.
        /// </summary>
        [JsonProperty("rotate_degree")]
        public int? RotateDegree { get; set; }

        /// <summary>
        /// /detect_time	Rotation time, the unit is ms.
        /// </summary>
        [JsonProperty("detect_time")]
        public int? DetectTime { get; set; }

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
            var curData = data as Cube;
            this._status = curData.Status;
            base.Update(data, cmdType);

            if (curData.RotateDegree.HasValue)
            {
                this.RotateDegree = curData.RotateDegree;
            }

            if (curData.DetectTime.HasValue)
            {
                this.DetectTime = curData.DetectTime;
            }
        }

        /// <summary>
        /// Force cube_status to be serialized as Status
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerialize_status()
        {
            return false;
        }
    }
}

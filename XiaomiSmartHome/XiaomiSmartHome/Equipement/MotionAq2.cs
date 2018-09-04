using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Motion sensor
    /// </summary>
    [StateObject]
    public class MotionAq2 : Motion
    {
        /// <summary>
        /// Illumination in lux.
        /// </summary>
        public int? Lux { get; set; }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            MotionAq2 curData = data as MotionAq2;
            
            base.Update(curData, cmdType);

            if (curData.Lux.HasValue)
            {
                this.Lux = curData.Lux;
            }
        }
    }
}

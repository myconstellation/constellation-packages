using Constellation;
using Constellation.Package;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Smart Wireless Switch
    /// </summary>
    [StateObject]
    public class Switch : Equipment
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Switch()
        {
            base.Battery = BatteryType.CR1632;
        }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            base.Update(data, cmdType);
        }
    }
}

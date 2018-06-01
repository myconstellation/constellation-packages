using Constellation;
using Constellation.Package;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Windows door sensor v2
    /// </summary>
    [StateObject]
    public class MagnetAq2 : Magnet
    {
        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data)
        {
            base.Update(data);
        }
    }
}
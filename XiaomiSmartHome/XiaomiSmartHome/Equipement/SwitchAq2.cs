using Constellation.Package;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    [StateObject]
    public class SwitchAq2 : Switch
    {
        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public override void Update(object data, string cmdType)
        {
            base.Update(data, cmdType);
        }
    }
}

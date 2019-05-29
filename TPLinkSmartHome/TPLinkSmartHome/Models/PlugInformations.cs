namespace TPLinkSmartHome.Models
{
    /// <summary>
    /// Informations concerning a TPLink Plug
    /// </summary>
    public class PlugInformations : PublicSystemInfo
    {
        internal static PlugInformations CreateFromSystemInfosAndOutputState(TPLink.SmartHome.SystemInfo sysInfo, TPLink.SmartHome.OutputState state)
        {
            var infos = new PlugInformations();
            infos.FillWithSystemInfosAndOutputState(sysInfo, state);
            
            return infos;
        }

        internal void FillWithSystemInfosAndOutputState(TPLink.SmartHome.SystemInfo sysInfo, TPLink.SmartHome.OutputState state)
        {
            IsPowered = state == TPLink.SmartHome.OutputState.On;
            this.FillWithSystemInfo(sysInfo);
        }

        /// <summary>
        /// indicates the power state of the device ('on' or 'off')
        /// </summary>
        public bool IsPowered { get; private set; }
    }
}

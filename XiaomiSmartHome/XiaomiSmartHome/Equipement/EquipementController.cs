using System.Collections.Generic;
using System.Drawing;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Send command for equipement
    /// </summary>
    public class EquipementController
    {
        /// <summary>
        /// Equipement manager
        /// </summary>
        EquipementManager equipementManager;

        #region CTOR
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="eqManager">The equipement manager</param>
        public EquipementController(EquipementManager eqManager)
        {
            equipementManager = eqManager;
        }
        #endregion

        #region GATEWAY

        /// <summary>
        /// Turn the gateway light on
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="brightness">Brightness</param>
        public void TurnGatewayLightOn(int r, int g, int b, int? brightness = null)
        {
            Gateway gw = equipementManager.gateway;
            int curBrightness = brightness ?? 255;
            if (!brightness.HasValue && gw?.Report?.Rgb > 0)
            {
                Color curColor = Color.FromArgb(gw.Report.Rgb);
                brightness = curColor.A;
            }

            Color needColor = Color.FromArgb(curBrightness, r, g, b);
            int argb = needColor.ToArgb();

            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "rgb", argb }
            };

            equipementManager.SendCommand(Constants.WRITE, gw.Sid, lParam);
        }

        /// <summary>
        /// Turn the gateway light off
        /// </summary>
        public void TurnGatewayLightOff()
        {
            Gateway gw = equipementManager.gateway;
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "rgb", 0 }
            };

            equipementManager.SendCommand(Constants.WRITE, gw.Sid, lParam);
        }

        /// <summary>
        /// Start playing sound on gateway
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="vol"></param>
        public void PlayGatewaySound(int mid, int vol)
        {
            //if ring_id in [9, 14-19]: not defined in gateway

            Gateway gw = equipementManager.gateway;
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "mid", mid },
                { "vol", vol }
            };

            equipementManager.SendCommand(Constants.WRITE, gw.Sid, lParam);
        }

        /// <summary>
        /// Stop playing sound on gateway
        /// </summary>
        public void StopGatewaySound()
        {
            Gateway gw = equipementManager.gateway;
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "mid", 10000 }
            };

            equipementManager.SendCommand(Constants.WRITE, gw.Sid, lParam);
        }

        #endregion

        #region PLUG

        /// <summary>
        /// Turn plug on
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        public void TurnPlugOn(string sid)
        {
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "status", "on" }
            };

            equipementManager.SendCommand(Constants.WRITE, sid, lParam);
        }

        /// <summary>
        /// Turn plug off
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        public void TurnPlugOff(string sid)
        {
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "status", "off" }
            };

            equipementManager.SendCommand(Constants.WRITE, sid, lParam);
        }

        #endregion

        #region SWITCH

        /// <summary>
        /// Simulate click on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        public void Click(string sid)
        {
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "status", "click" }
            };

            equipementManager.SendCommand(Constants.WRITE, sid, lParam);
        }

        /// <summary>
        /// Simulate DoubleClick on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        public void DoubleClick(string sid)
        {
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "status", "double_click" }
            };

            equipementManager.SendCommand(Constants.WRITE, sid, lParam);
        }

        /// <summary>
        /// Simulate LongClickPress on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        public void LongClickPress(string sid)
        {
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "status", "long_click_press" }
            };

            equipementManager.SendCommand(Constants.WRITE, sid, lParam);
        }

        /// <summary>
        /// Simulate LongClickRelease on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        public void LongClickRelease(string sid)
        {
            Dictionary<string, object> lParam = new Dictionary<string, object>
            {
                { "status", "long_click_release" }
            };

            equipementManager.SendCommand(Constants.WRITE, sid, lParam);
        }

        #endregion
    }
}

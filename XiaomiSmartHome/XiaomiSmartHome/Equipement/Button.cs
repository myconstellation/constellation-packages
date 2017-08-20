using Constellation;
using Constellation.Package;
using Newtonsoft.Json;
using static XiaomiSmartHome.Model.Response;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Humidity / Temperature sensor
    /// </summary>
    [StateObject, XiaomiEquipement("86sw2")]
    public class Button
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public string Model { get; set; } = "weather.v1";

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public string Battery { get; set; } = "CR2032";

        /// <summary>
        /// Battery level.
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Last report.
        /// </summary>
        public ButtonReport Report { get; set; }
    }

    /// <summary>
    /// Humidity / Temperature sensor last report
    /// </summary>
    [StateObject, XiaomiEquipement("86sw2_report")]
    public class ButtonReport
    {
        /// <summary>
        /// Voltage left
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Voltage left.
        /// </summary>
        public string Channel0 { get; set; }

        /// <summary>
        /// Temperature level in ºC.
        /// </summary>
        public string Channel1 { get; set; }
    }
}

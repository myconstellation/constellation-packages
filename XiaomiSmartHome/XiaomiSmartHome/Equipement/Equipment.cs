using Constellation.Package;
using LiteDB;
using Newtonsoft.Json;
using System;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Base equipment class
    /// </summary>
    [StateObject]
    public class Equipment
    {
        /// <summary>
        /// Model type.
        /// </summary>
        public EquipmentType Model { get; set; }

        /// <summary>
        /// SID (mac adress).
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Short id
        /// </summary>
        [JsonProperty("short_id")]
        public int ShortId { get; set; }

        /// <summary>
        /// Voltage left
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// Battery type.
        /// </summary>
        public BatteryType Battery { get; set; }

        /// <summary>
        /// Battery level
        /// </summary>
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Sensor state
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        public virtual void Update(object data) { }

        /// <summary>
        /// Get percent battery left
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        internal int ParseVoltage(int voltage)
        {
            int maxVolt = 3000;
            int minVolt = 2500;
            if (voltage > maxVolt) voltage = maxVolt;
            if (voltage < minVolt) voltage = minVolt;
            return (int)Math.Round((((decimal)(voltage - minVolt) / (decimal)(maxVolt - minVolt)) * 100));
        }
    }
}

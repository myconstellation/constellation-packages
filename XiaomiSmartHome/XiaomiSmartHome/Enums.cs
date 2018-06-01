using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace XiaomiSmartHome
{
    /// <summary>
    /// Enumerations
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Cache for enums
        /// </summary>
        private static readonly ConcurrentDictionary<Enum, string> _realNames = new ConcurrentDictionary<Enum, string>();

        /// <summary>
        /// Enumeration of availlable commands
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CommandType
        {
            /// <summary>
            /// Commande name for get id list
            /// </summary>
            [EnumMember(Value = "get_id_list")]
            GetIdList = 0,

            /// <summary>
            /// Commande name for get id list acknowledgement
            /// </summary>
            [EnumMember(Value = "get_id_list_ack")]
            GetIdListAck = 1,

            /// <summary>
            /// Commande name for read
            /// </summary>
            [EnumMember(Value = "read")]
            Read = 2,

            /// <summary>
            /// Commande name for read acknowledgement
            /// </summary>
            [EnumMember(Value = "read_ack")]
            ReadAck = 3,

            /// <summary>
            /// Commande name for write
            /// </summary>
            [EnumMember(Value = "write")]
            Write = 4,

            /// <summary>
            /// Commande name for write acknowledgement
            /// </summary>
            [EnumMember(Value = "write_ack")]
            WriteAck = 5,

            /// <summary>
            /// Commande name for heartbeat
            /// </summary>
            [EnumMember(Value = "heartbeat")]
            Heartbeat = 6,

            /// <summary>
            /// Commande name for report
            /// </summary>
            [EnumMember(Value = "report")]
            Report = 7
        };

        /// <summary>
        /// Equipment battery type
        /// </summary>
        public enum BatteryType
        {
            /// <summary>
            /// Battery type sector
            /// </summary>
            [EnumMember(Value = "Sector")]
            SECTOR = 0,

            /// <summary>
            /// Battery type CR1632
            /// </summary>
            [EnumMember(Value = "CR1632")]
            CR1632 = 1,

            /// <summary>
            /// Battery type CR2450
            /// </summary>
            [EnumMember(Value = "CR2450")]
            CR2450 = 2,

            /// <summary>
            /// Battery type CR2032
            /// </summary>
            [EnumMember(Value = "CR2032")]
            CR2032 = 3
        }

        /// <summary>
        /// Equipment type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum EquipmentType
        {
            /// <summary>
            /// Gateway model name in report or heartbeat
            /// </summary>
            [EnumMember(Value = "gateway")]
            Gateway = 0,

            /// <summary>
            /// Equipement type Smart Wireless Switch
            /// </summary>
            [EnumMember(Value = "switch")]
            Switch = 1,

            /// <summary>
            /// Equipement type temperatuer / humidity sensor
            /// </summary>
            [EnumMember(Value = "sensor_ht")]
            SensorHt = 2,

            /// <summary>
            /// Equipement type wall plug
            /// </summary>
            [EnumMember(Value = "plug")]
            Plug = 3,

            /// <summary>
            /// Equipement type motion detector
            /// </summary>
            [EnumMember(Value = "motion")]
            Motion = 4,

            /// <summary>
            /// Equipement type windows door sensor
            /// </summary>
            [EnumMember(Value = "magnet")]
            Magnet = 5,

            /// <summary>
            /// Equipement type motion detector v2
            /// </summary>
            [EnumMember(Value = "sensor_motion.aq2")]
            MotionAq2 = 6,

            /// <summary>
            /// Equipement type windows door sensor v2
            /// </summary>
            [EnumMember(Value = "sensor_magnet.aq2")]
            MagnetAq2 = 7,

            /// <summary>
            /// Equipement type capteur température et humidité
            /// </summary>
            [EnumMember(Value = "weather.v1")]
            Weather = 8,

            /// <summary>
            /// Equipement type switch mural un bouton
            /// </summary>
            [EnumMember(Value = "86sw1")]
            WallSwitchOneButon = 9,
        }

        /// <summary>
        /// Get value seen by xiaomi ;)
        /// </summary>
        /// <param name="enumValue">Selected enum value</param>
        /// <returns></returns>
        public static string GetRealName(this Enum enumValue)
        {
            if (_realNames.ContainsKey(enumValue))
            {
                // get from the cache
                return _realNames[enumValue];
            }

            //read the attribute
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            EnumMemberAttribute[] attributes = (EnumMemberAttribute[])fi.GetCustomAttributes(typeof(EnumMemberAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                // add to cache
                _realNames.TryAdd(enumValue, attributes[0].Value);
                return attributes[0].Value;
            }

            return null;
        }
    }
}

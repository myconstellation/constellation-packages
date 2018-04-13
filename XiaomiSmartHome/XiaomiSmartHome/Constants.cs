namespace XiaomiSmartHome
{
    /// <summary>
    /// Constants of package
    /// </summary>
    public class Constants
    {
        #region SETTINGS

        /// <summary>
        /// Setting name for gateway ip
        /// </summary>
        public const string GATEWAY_IP = "GatewayIP";

        /// <summary>
        /// Setting name for gateway multicast ip
        /// </summary>
        public const string GATEWAY_MULTICAST_IP = "GatewayMulticastIP";

        /// <summary>
        /// Setting name for gateway multicast port
        /// </summary>
        public const string GATEWAY_MULTICAST_PORT = "GatewayMulticastPort";

        /// <summary>
        /// Setting name for gateway password
        /// </summary>
        public const string GATEWAY_PASSWORD = "GatewayPassword";

        /// <summary>
        /// Setting name for activation of heartbeat log
        /// </summary>
        public const string HEARTBEAT_LOG = "HeartbeatLog";

        /// <summary>
        /// Setting name for activation of report log
        /// </summary>
        public const string REPORT_LOG = "ReportLog";

        #endregion

        #region COMMANDS

        /// <summary>
        /// Commande name for get id list
        /// </summary>
        public const string GET_ID_LISTE = "get_id_list";

        /// <summary>
        /// Commande name for get id list acknowledgement
        /// </summary>
        public const string GET_ID_LISTE_ACK = "get_id_list_ack";

        /// <summary>
        /// Commande name for read
        /// </summary>
        public const string READ = "read";

        /// <summary>
        /// Commande name for read acknowledgement
        /// </summary>
        public const string READ_ACK = "read_ack";

        /// <summary>
        /// Commande name for write
        /// </summary>
        public const string WRITE = "write";

        /// <summary>
        /// Commande name for write acknowledgement
        /// </summary>
        public const string WRITE_ACK = "write_ack";

        /// <summary>
        /// Commande name for heartbeat
        /// </summary>
        public const string HEARTBEAT = "heartbeat";

        /// <summary>
        /// Commande name for report
        /// </summary>
        public const string REPORT = "report";
        
        #endregion

        #region BATTERY TYPE

        /// <summary>
        /// Battery type sector
        /// </summary>
        public const string SECTOR = "Sector";

        /// <summary>
        /// Battery type CR1632
        /// </summary>
        public const string CR1632 = "CR1632";

        /// <summary>
        /// Battery type CR2450
        /// </summary>
        public const string CR2450 = "CR2450";

        /// <summary>
        /// Battery type CR2032
        /// </summary>
        public const string CR2032 = "CR2032";

        #endregion

        #region EQUIPEMENT TYPE

        /// <summary>
        /// Equipement type Smart Wireless Switch
        /// </summary>
        public const string SWITCH = "switch";

        /// <summary>
        /// Equipement type temperatuer / humidity sensor
        /// </summary>
        public const string SENSOR_HT = "sensor_ht";

        /// <summary>
        /// Equipement type wall plug
        /// </summary>
        public const string PLUG = "plug";

        /// <summary>
        /// Equipement type motion detector
        /// </summary>
        public const string MOTION = "motion";

        /// <summary>
        /// Equipement type windows door sensor
        /// </summary>
        public const string MAGNET = "magnet";

        /// <summary>
        /// Gateway model name in report or heartbeat
        /// </summary>
        public const string GATEWAY = "gateway";


        #endregion
    }
}

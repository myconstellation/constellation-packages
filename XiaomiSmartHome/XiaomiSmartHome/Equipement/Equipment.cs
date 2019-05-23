using System;
using Constellation.Package;
using Newtonsoft.Json;
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
        /// Last command type
        /// </summary>
        public string CmdType { get; set; }

        /// <summary>
        /// Last status change date
        /// </summary>
        public DateTime LastSatusChange { get; set; }

        /// <summary>
        /// Update equipment with last data
        /// </summary>
        /// <param name="data">The reported data</param>
        /// <param name="cmdType">Commande type</param>
        public virtual void Update(object data, string cmdType)
        {
            Equipment curData = data as Equipment;

            #region data sample
            /* 
                {"cmd":"read_ack","model":"plug","sid":"158d0001bbfe49","short_id":40845,"data":"{\"voltage\":3600,\"status\":\"on\",\"inuse\":\"1\",\"power_consumed\":\"17530\",\"load_power\":\"191.40\"}"}
                {"cmd":"report","model":"plug","sid":"158d0001bbfe49","short_id":40845,"data":"{\"status\":\"on\"}"}
                {"cmd":"heartbeat","model":"plug","sid":"158d0001bbfe49","short_id":40845,"data":"{\"voltage\":3600,\"status\":\"on\",\"inuse\":\"1\",\"power_consumed\":\"20482\",\"load_power\":\"0.97\"}"}

                {"cmd":"read_ack","model":"switch","sid":"158d0002371a0c","short_id":18313,"data":"{\"voltage\":3072}"}
                {"cmd":"report","model":"switch","sid":"158d0002371a0c","short_id":18313,"data":"{\"status\":\"click\"}"}
                {"cmd":"report","model":"switch","sid":"158d0002371a0c","short_id":18313,"data":"{\"status\":\"double_click\"}"}
                {"cmd":"report","model":"switch","sid":"158d0002371a0c","short_id":18313,"data":"{\"status\":\"long_click_press\"}"}
                {"cmd":"report","model":"switch","sid":"158d0002371a0c","short_id":18313,"data":"{\"status\":\"long_click_release\"}"}

                {"cmd":"read_ack","model":"magnet","sid":"158d00020e8344","short_id":37400,"data":"{\"voltage\":3005,\"status\":\"close\"}"}
                {"cmd":"report","model":"magnet","sid":"158d00020e8344","short_id":37400,"data":"{\"status\":\"open\"}"}
                {"cmd":"report","model":"magnet","sid":"158d00020e8344","short_id":37400,"data":"{\"status\":\"close\"}"}
                {"cmd":"report","model":"magnet","sid":"158d00020e8344","short_id":37400,"data":"{\"no_close\":\"60\"}"}

                {"cmd":"read_ack","model":"motion","sid":"158d00023218ed","short_id":40870,"data":"{\"voltage\":3015}"}
                {"cmd":"report","model":"motion","sid":"158d00023218ed","short_id":40870,"data":"{\"status\":\"motion\"}"}
                {"cmd":"report","model":"motion","sid":"158d00023218ed","short_id":40870,"data":"{\"no_motion\":\"120\"}"}
                {"cmd":"report","model":"motion","sid":"158d00023218ed","short_id":40870,"data":"{\"no_motion\":\"180\"}"}
                {"cmd":"report","model":"motion","sid":"158d00023218ed","short_id":40870,"data":"{\"no_motion\":\"300\"}"}


                {"cmd":"get_id_list_ack","sid":"7811dcdf0ae6","token":"4cUUY0X95fhfmGU7","data":"[\"158d0001bbfe49\",\"158d00023218ed\",\"158d00020e8344\",\"158d0002371a0c\"]"}
                {"cmd":"write_ack","model":"gateway","sid":"xxxxx","short_id":0,"data":"{\"rgb\":0,\"illumination\":1292,\"proto_version\":\"1.0.9\"}"}
                {"cmd":"report","model":"gateway","sid":"7811dcdf0ae6","short_id":0,"data":"{\"rgb\":0,\"illumination\":1292}"}
                {"cmd":"report","model":"gateway","sid":"7811dcdf0ae6","short_id":0,"data":"{\"rgb\":1677727487,\"illumination\":1292}"}
                {"cmd":"heartbeat","model":"gateway","sid":"7811dcdf0ae6","short_id":"0","token":"InXucBigQWh5KFet","data":"{\"ip\":\"192.168.1.26\"}"}
              */
            #endregion
            // Le statut est la seul donnée intéréssante, sinon ce sont des temps … no close, no motion …
            if (curData != null && this.Status != curData.Status && cmdType == CommandType.Report.ToString())
            {
                this.LastSatusChange = DateTime.Now;
            }

            this.CmdType = cmdType;
            this.Status = curData.Status;

            if (curData.Voltage != default(int))
            {
                this.Voltage = curData.Voltage;
                this.BatteryLevel = this.ParseVoltage(curData.Voltage);
            }
        }

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

using System;

namespace RFLinkNet
{
    public class RFLinkSettings
    {
        public bool SetRF433 { get; set; }
        public bool SetNodoNRF { get; set; }
        public bool SetMiLight { get; set; }
        public bool SetLivingColors { get; set; }
        public bool SetAnsluta { get; set; }
        public bool SetGPIO { get; set; }
        public bool SetBLE { get; set; }
        public bool SetMySensors { get; set; }
        public string Version { get; set; }
        public string Rev { get; set; }
        public string Build { get; set; }

        /// <summary>
        /// Get protocol activation
        /// </summary>
        /// <param name="data">>received data</param>
        public void ProcessStatusResponse(RFData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Fields["Protocol"] != "STATUS" ||
                   !data.Fields.ContainsKey("setRF433") ||
                   !data.Fields.ContainsKey("setNodoNRF") ||
                   !data.Fields.ContainsKey("setMilight") ||
                   !data.Fields.ContainsKey("setLivingColors") ||
                   !data.Fields.ContainsKey("setAnsluta") ||
                   !data.Fields.ContainsKey("setGPIO") ||
                   !data.Fields.ContainsKey("setMysensors") ||
                   !data.Fields.ContainsKey("setBLE"))
            {
                throw new FormatException("RF Data did not contain correct status data");
            }
            
            SetRF433 = ProtocolParser.ToBoolean(data.Fields["setRF433"]);
            SetNodoNRF = ProtocolParser.ToBoolean(data.Fields["setNodoNRF"]);
            SetMiLight = ProtocolParser.ToBoolean(data.Fields["setMilight"]);
            SetLivingColors = ProtocolParser.ToBoolean(data.Fields["setLivingColors"]);
            SetAnsluta = ProtocolParser.ToBoolean(data.Fields["setAnsluta"]);
            SetGPIO = ProtocolParser.ToBoolean(data.Fields["setGPIO"]);
            SetMySensors = ProtocolParser.ToBoolean(data.Fields["setMysensors"]);
            SetBLE = ProtocolParser.ToBoolean(data.Fields["setBLE"]);
        }

        /// <summary>
        /// Get rflink software version
        /// </summary>
        /// <param name="data">received data</param>
        public void ProcessVerResponse(RFData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Fields["Protocol"].StartsWith("VER") || !data.Fields.ContainsKey("REV") || !data.Fields.ContainsKey("BUILD"))
            {
                throw new FormatException("RF Data did not contain correct version data");
            }

            Version = data.Fields["Protocol"];
            Rev = data.Fields["REV"];
            Build = data.Fields["BUILD"];
        }
    }
}

/*
 *	 RFXCom Core library
 *	 Author: Sébastien Warin
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
 *	
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *	
 *	 Unless required by applicable law or agreed to in writing,
 *	 software distributed under the License is distributed on an
 *	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *	 KIND, either express or implied. See the License for the
 *	 specific language governing permissions and limitations
 *	 under the License.
 */

namespace RfxCom.Packets
{
    using Core;
    using System.Collections.Generic;

    [RfxPacket(Type = RfxPacketType.TemperatureSensors, Length = 9)]
    public class TemperatureSensor : RfxPacket
    {
        private static readonly Dictionary<byte, string> rfx_subtype_50 = new Dictionary<byte, string>()
        {
            [0x01] = "THR128/138, THC138",
            [0x02] = "THC238/268,THN132,THWR288,THRN122,THN122,AW129/131",
            [0x03] = "THWR800",
            [0x04] = "RTHN318",
            [0x05] = "La Crosse TX2, TX3, TX4, TX17",
            [0x06] = "TS15C",
            [0x07] = "Viking 02811",
            [0x08] = "La Crosse WS2300",
            [0x09] = "RUBiCSON",
            [0x0A] = "TFA 30.3133"
        };

        public int SensorID { get; set; }
        public int Channel { get; set; }
        public int BatteryLevel { get; set; }
        public int SignalLevel { get; set; }
        public double Temperature { get; set; }

        public override void Parse(byte[] packet)
        {
            base.Parse(packet);
            this.SubTypeName = rfx_subtype_50[(byte)this.SubType];
            this.SensorID = int.Parse(packet[4].ToString("X2") + packet[5].ToString("X2"), System.Globalization.NumberStyles.HexNumber);
            this.Channel = packet[5];
            this.Temperature = ((packet[6] & 0x7f) * 256 + packet[7]) / 10d;
            var signbit = packet[6] & 0x80;
            if (signbit != 0)
            {
                this.Temperature = -this.Temperature;
            }
            this.BatteryLevel = packet[8] & 0xf;
            this.SignalLevel = packet[8] >> 4;
        }

        public override string ToString()
        {
            return $"[TemperatureSensor] ID={SensorID} (Ch:{Channel}) Temperature={Temperature}° (Signal:{SignalLevel} - Battery:{BatteryLevel})";
        }
    }
}

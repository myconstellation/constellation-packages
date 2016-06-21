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

    [RfxPacket(Type = RfxPacketType.TemperatureHumiditySensors, Length = 11)]
    public class TemperatureHumiditySensor : TemperatureSensor
    {
        private static readonly Dictionary<byte, string> rfx_subtype_52 = new Dictionary<byte, string>()
        {
            [0x01] = "THGN122/123, THGN132, THGR122/228/238/268",
            [0x02] = "THGR810, THGN800, THGR810",
            [0x03] = "RTGR328",
            [0x04] = "THGR328",
            [0x05] = "WTGR800",
            [0x06] = "THGR918/928, THGRN228, THGN500",
            [0x07] = "TFA TS34C, Cresta",
            [0x08] = "WT260,WT260H,WT440H,WT450,WT450H",
            [0x09] = "Viking 02035,02038 (02035 has no humidity)",
            [0x0A] = "Rubicson",
            [0x0B] = "EW109",
        };

        private static readonly Dictionary<byte, string> rfx_subtype_52_humstatus = new Dictionary<byte, string>()
        {
            [0x00] = "Dry",
            [0x01] = "Comfort",
            [0x02] = "Normal",
            [0x03] = "Wet"
        };

        public int Humidity { get; set; }
        public string Status { get; set; }

        public override void Parse(byte[] packet)
        {
            base.Parse(packet);
            this.SubTypeName = rfx_subtype_52[(byte)this.SubType];
            this.SensorID = int.Parse(packet[4].ToString("X2") + packet[5].ToString("X2"), System.Globalization.NumberStyles.HexNumber);
            this.Channel = packet[5];
            this.Temperature = (double)packet[7] / 10d;  // ((packet[6] & 0x7f) * 256 + packet[7]) / 10;
            var signbit = packet[6] & 0x80;
            if (signbit != 0)
            {
                this.Temperature = -this.Temperature;
            }
            this.Humidity = packet[8];
            this.Status = rfx_subtype_52_humstatus[packet[9]];
            this.BatteryLevel = packet[10] & 0xf;
            this.SignalLevel = packet[10] >> 4;
        }

        public override string ToString()
        {
            return $"[TemperatureHumiditySensor] ID={SensorID} (Ch:{Channel}) Temperature={Temperature}°|Humidity={Humidity}%|Status={Status} (Signal:{SignalLevel} - Battery:{BatteryLevel})";
        }
    }
}

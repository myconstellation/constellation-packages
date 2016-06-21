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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [RfxPacket(Type = RfxPacketType.InterfaceMessage, Length = 14)]
    public class InterfaceMessage : RfxPacket
    {
        private static readonly Dictionary<byte, string> rfx_receiver_type = new Dictionary<byte, string>()
        {
            [0x50] = "310MHz",
            [0x51] = "315MHz",
            [0x52] = "433.92MHz receiver only",
            [0x53] = "433.92MHz transceiver",
            [0x55] = "868.00MHz",
            [0x56] = "868.00MHz FSK",
            [0x57] = "868.30MHz",
            [0x58] = "868.30MHz FSK",
            [0x59] = "868.35MHz",
            [0x5A] = "868.35MHz FSK",
            [0x5B] = "868.95MHz",
        };

        private static readonly Dictionary<byte, string> rfx_packets_subtype = new Dictionary<byte, string>()
        {
            [0x00] = "Response on a mode command",
            [0xFF] = "Wrong command received from the application.",
        };

        public int CommandeType { get; set; }
        public int TransceiverType { get; set; }
        public string TransceiverName { get; set; }
        public int FirmwareVersion { get; set; }
        public List<Protocol> Protocols { get; set; }

        public override void Parse(byte[] packet)
        {
            base.Parse(packet);

            this.SubTypeName = rfx_packets_subtype[(byte)this.SubType];
            this.CommandeType = packet[4];
            this.TransceiverType = packet[5];
            this.TransceiverName = rfx_receiver_type[packet[5]];
            this.FirmwareVersion = packet[6];

            this.Protocols = new List<Protocol>();
            this.AddProtocolsFromFlag(packet[7], 0);
            this.AddProtocolsFromFlag(packet[8], 1);
            this.AddProtocolsFromFlag(packet[9], 2);
        }

        private void AddProtocolsFromFlag(byte flag, int flagId = 0)
        {
            char[] protocols = Convert.ToString(flag, 2).PadLeft(8, '0').ToArray();
            for (int i = flagId * 8; i < flagId * 8 + 8; i++)
            {
                this.Protocols.Add(new Protocol() { Name = Utils.GetDescriptionFromEnumValue((RfxProtocol)i), Enabled = protocols[i - flagId * 8] == '1' });
            }
        }

        public override string ToString()
        {
            return $"[InterfaceMessage] {TransceiverName} version {FirmwareVersion} with {Protocols.Count(p => p.Enabled)} protocol(s) enabled ({string.Join(", ", Protocols.Where(p => p.Enabled).Select(p => p.Name))})";
        }

        public class Protocol
        {
            public string Name { get; set; }
            public bool Enabled { get; set; }
        }
    }
}

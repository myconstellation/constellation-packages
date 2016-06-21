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

namespace RfxCom.Core
{
    using System;

    public class RfxPacket
    {
        public byte[] RawData { get; set; }
        public int Length { get; set; }
        public int Type { get; set; }
        public int SubType { get; set; }
        public int SequenceNumber { get; set; }
        public string TypeName { get; set; }
        public virtual string SubTypeName { get; set; }

        public virtual void Parse(byte[] packet)
        {
            if (packet.Length != packet[0] + 1)
            {
                throw new ArgumentOutOfRangeException("packet", $"Expected packet length to be {packet[0] + 1} bytes but it was {packet.Length} bytes");
            }

            this.RawData = packet;
            this.Length = packet[0];
            this.Type = packet[1];
            this.TypeName =  ((RfxPacketType)packet[1]).ToString();
            this.SubType = packet[2];
            this.SequenceNumber = packet[3];
        }

        public override string ToString()
        {
            return $"[RfxPacket] '{TypeName}'({Type}) - SubType:{SubType} - Sequence:{SequenceNumber} - Length:{Length}\nRaw data: {BitConverter.ToString(RawData)}";
        }
    }
}

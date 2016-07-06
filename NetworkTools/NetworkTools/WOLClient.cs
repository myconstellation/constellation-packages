/*
 *	 NetworkTools package for Constellation
 *	 Web site: http://www.myConstellation.io
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

namespace NetworkTools
{
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;

    public class WOLClient : UdpClient
    {
        public WOLClient() : base()
        {
        }

        public void SetClientToBrodcastMode()
        {
            if (this.Active)
            {
                this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
            }
        }

        public static void SendWakeUpPacket(IPAddress ip, string macAddress, short port = 9)
        {
            WOLClient client = new WOLClient();
            client.Connect(ip, (byte)port);
            client.SetClientToBrodcastMode();
            //set sending bites
            int counter = 0;
            //buffer to be send
            byte[] bytes = new byte[1024];   // more than enough :-)
            // Clear mac address
            macAddress = macAddress.Replace("-", "").Replace(":", "");
            //first 6 bytes should be 0xFF
            for (int y = 0; y < 6; y++)
            {
                bytes[counter++] = 0xFF;
            }
            //now repeate MAC 16 times
            for (int y = 0; y < 16; y++)
            {
                int i = 0;
                for (int z = 0; z < 6; z++)
                {
                    bytes[counter++] = byte.Parse(macAddress.Substring(i, 2), NumberStyles.HexNumber);
                    i += 2;
                }
            }
            //now send wake up packet
            int reterned_value = client.Send(bytes, 1024);
        }
    }
}

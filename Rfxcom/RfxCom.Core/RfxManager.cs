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
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class RfxManager : IDisposable
    {
        private Dictionary<RfxPacketAttribute, Type> packetTypes = null;

        public CancellationTokenSource CancellationToken { get; private set; }
        public RfxInterface RfxInterface { get; private set; }

        public event EventHandler<MessageEventArgs> OnMessage;
        public event EventHandler<PacketReceivedEventArgs> OnPacketReceived;

        public RfxManager(string portCom)
            : this(new RfxInterface(portCom))
        { }

        public RfxManager(SerialPort serialPort)
            : this(new RfxInterface(serialPort))
        { }

        public RfxManager(RfxInterface rfxInterface)
        {
            this.RfxInterface = rfxInterface;
            this.packetTypes = new Dictionary<RfxPacketAttribute, Type>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(RfxPacket))))
            {
                this.packetTypes.Add(type.GetCustomAttribute<RfxPacketAttribute>(), type);
            }
        }

        public void Connect(string[] protocolsEnabled = null)
        {
            this.CancellationToken = new CancellationTokenSource();
            Task.Factory.StartNew(async () =>
            {
                // Init
                await this.Reset();
                await this.Flush();
                // Get status or set mode
                if (protocolsEnabled != null)
                {
                    await this.SetProtocols(protocolsEnabled);
                }
                else
                {
                    await this.GetStatus();
                }
                // Listening incomming message
                while (!this.CancellationToken.IsCancellationRequested)
                {
                    // Get datas
                    byte[] datas = await this.RfxInterface.ReadAsync(this.CancellationToken.Token);
                    // Create the packet                   
                    var packetType = this.packetTypes.Where(p => datas.Length > 1 && p.Key.Type == (RfxPacketType)datas[1] && datas.Length >= p.Key.Length ).Select(p => p.Value).FirstOrDefault();
                    RfxPacket packet = packetType != null ? (RfxPacket)Activator.CreateInstance(packetType) : new RfxPacket();
                    // Parse packet
                    packet.Parse(datas);
                    // Raise events
                    this.OnPacketReceived?.Invoke(this, new PacketReceivedEventArgs() { Packet = packet });
                    this.OnMessage?.Invoke(this, new MessageEventArgs() { Message = packet.ToString() });
                }
            });
        }

        public void Subscribe<TPacket>(Action<TPacket> onPacketReceived) where TPacket : RfxPacket
        {
            this.OnPacketReceived += (s, e) =>
            {
                if (e.Packet.GetType() == typeof(TPacket))
                {
                    onPacketReceived(e.Packet as TPacket);
                }
            };
        }

        public async Task GetStatus()
        {
            await this.SendMessage(new byte[] { 0xD, 0x0, 0x0, 0x2, 0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
        }

        public async Task SendMessage(byte[] message)
        {
            await this.RfxInterface.WriteAsync(message);
        }

        public async Task SetProtocols(string[] protocolsEnabled)
        {
            await this.SendMessage(new byte[] { 0xD, 0x0, 0x0, 0x0, 0x3, 0x53, 0x0, this.GetFlagForProtocolsEnabled(protocolsEnabled, 0), this.GetFlagForProtocolsEnabled(protocolsEnabled, 8), this.GetFlagForProtocolsEnabled(protocolsEnabled, 16), 0x0, 0x0, 0x0, 0x0 });
        }
        
        public async Task Flush()
        {
            await this.RfxInterface.FlushAsync();
        }

        public async Task Reset()
        {
            await this.SendMessage(new byte[] { 0xD, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void Disconnect()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.CancellationToken?.Cancel();
            this.RfxInterface?.Dispose();
        }

        private byte GetFlagForProtocolsEnabled(string[] protocolsEnabled, int startIndex = 0)
        {
            int value = 0;
            for (int i = startIndex; i < startIndex + 8; i++)
            {
                if (protocolsEnabled.Contains(Utils.GetDescriptionFromEnumValue((RfxProtocol)i)))
                    value += Convert.ToInt16(Math.Pow(2, 7 - (i % 8)));
            }
            return (byte)value;
        }

        public class MessageEventArgs : EventArgs
        {
            public string Message { get; set; }
        }

        public class PacketReceivedEventArgs : EventArgs
        {
            public RfxPacket Packet { get; set; }
        }
    }
}

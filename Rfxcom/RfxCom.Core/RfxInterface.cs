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
    using System.IO.Ports;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class RfxInterface : IDisposable
    {
        public SerialPort SerialPort { get; set; }

        public RfxInterface(string portName) :
            this(CreateSerialPort(portName))
        {
        }

        public RfxInterface(SerialPort serialPort)
        {
            this.SerialPort = serialPort;
        }

        public void Dispose()
        {
            if (this.SerialPort.IsOpen)
            {
                this.SerialPort.Close();
            }
            this.SerialPort.Dispose();
        }
        
        public async Task<byte[]> ReadAsync(CancellationToken cancellationToken)
        {
            if (!this.SerialPort.IsOpen)
            {
                this.SerialPort.Open();
            }
            var packetLengthBuffer = await ReadBuffer(1, cancellationToken);
            var packetLength = packetLengthBuffer[0];
            var packetBuffer = await ReadBuffer(packetLength, cancellationToken);
            return packetLengthBuffer.Concat(packetBuffer).ToArray();
        }

        public async Task FlushAsync()
        {
            if (!this.SerialPort.IsOpen)
            {
                this.SerialPort.Open();
            }
            await this.SerialPort.BaseStream.FlushAsync();
        }

        public async Task WriteAsync(byte[] buffer)
        {
            if (!this.SerialPort.IsOpen)
            {
                this.SerialPort.Open();
            }
            await this.SerialPort.BaseStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task<byte[]> ReadBuffer(int length, CancellationToken cancellationToken)
        {
            var packetBuffer = new byte[length];
            var totalBytesRemaining = length;
            var totalBytesRead = 0;
            while (totalBytesRemaining != 0)
            {
                var bytesRead = await this.SerialPort.BaseStream.ReadAsync(packetBuffer, totalBytesRead, totalBytesRemaining, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                totalBytesRead += bytesRead;
                totalBytesRemaining -= bytesRead;
            }
            return packetBuffer;
        }
        
        private static SerialPort CreateSerialPort(string portName)
        {
            return new SerialPort
            {
                PortName = portName,
                BaudRate = 38400,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }
    }
}

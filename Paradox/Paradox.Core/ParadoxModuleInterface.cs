/*
 *	 Paradox .NET library
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Sebastien Warin under one or more contributor
 *	 license agreements. Sebastien Warin licenses this file to you under
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

namespace Paradox
{
    using System;
    using System.IO.Ports;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the Paradox PRT3 module interface
    /// </summary>
    public class ParadoxModuleInterface
    {
        private const int BUFFER_TIMEOUT = 2000; //ms
        private const int DEFAULT_BAUD_RATE = 57600;
        private const string FLAG_OK = "&ok";

        private object syncLock = new object();
        private string strBuffer = string.Empty;
        private SerialPort serialPort = null;
        private Thread readingThread = null;
        private bool isConnected = false;
        private DateTime lastReceivedDatas = DateTime.MinValue;

        /// <summary>
        /// Gets a value indicating whether the interface is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the interface is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return this.isConnected; }
            private set
            {
                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    if (this.ConnectionStateChanged != null)
                    {
                        this.ConnectionStateChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        #region Interface events

        /// <summary>
        /// Occurs when message is sent to the PRT3 module.
        /// </summary>
        public event EventHandler<ParadoxMessageEventArgs> MessageSent;

        /// <summary>
        /// Occurs when message is received from the PRT3 module.
        /// </summary>
        public event EventHandler<ParadoxMessageEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when the interface raise error.
        /// </summary>
        public event EventHandler<InterfaceErrorEventArgs> InterfaceError;

        /// <summary>
        /// Occurs when the connection state changed.
        /// </summary>
        public event EventHandler ConnectionStateChanged;

        #endregion

        #region Internal constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxModuleInterface"/> class.
        /// </summary>
        /// <param name="portCom">The serial port name.</param>
        /// <param name="baudRate">The serial port baud rate.</param>
        internal ParadoxModuleInterface(string portCom, int baudRate = DEFAULT_BAUD_RATE)
        {
            this.IsConnected = false;
            this.serialPort = new SerialPort(portCom, baudRate, Parity.None, 8, StopBits.One);
            this.serialPort.NewLine = "\r";
            this.readingThread = new Thread(new ThreadStart(ReadSerialPort));
        }

        #endregion

        #region Connect & Disconnect methods

        /// <summary>
        /// Connects to the Paradox PRT3 module.
        /// </summary>
        public void Connect()
        {
            if (!this.IsConnected)
            {
                this.IsConnected = true;
                // Open the serial port
                this.serialPort.Open();
                // Start the reading thread
                this.readingThread.Start();
            }
            else
            {
                throw new InvalidOperationException("The interface is already connected !");
            }
        }

        /// <summary>
        /// Disconnects from the Paradox PRT3 module.
        /// </summary>
        public void Disconnect()
        {
            if (this.IsConnected)
            {
                this.IsConnected = false;
                // Close the serial port
                this.serialPort.Close();
            }
            else
            {
                throw new InvalidOperationException("The interface is not connected!");
            }
        }

        #endregion

        #region Send command methods

        /// <summary>
        /// Sends the specified command to the Paradox PRT3 module.
        /// </summary>
        /// <param name="command">The command.</param>
        public void SendCommand(string command)
        {
            if (this.IsConnected)
            {
                try
                {
                    // Write the command to the serial port and wait 10ms
                    this.serialPort.WriteLine(command);
                    Thread.Sleep(10);
                    // Log obfuscation for PIN code
                    if (command.StartsWith("AA") || command.StartsWith("AD"))
                    {
                        command = command.Substring(0, command.Length - 4) + "****";
                    }
                    // Raise the MessageSent event
                    if (this.MessageSent != null)
                    {
                        var message = new ParadoxMessageEventArgs() { Date = DateTime.Now, Message = command };
                        Delegate[] receivers = this.MessageSent.GetInvocationList();
                        foreach (EventHandler<ParadoxMessageEventArgs> receiver in receivers)
                        {
                            receiver.BeginInvoke(this, message, null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.InterfaceError != null)
                    {
                        this.InterfaceError(this, new InterfaceErrorEventArgs() { Exception = ex });
                    }
                    this.IsConnected = false;
                }
            }
            else
            {
                throw new InvalidOperationException("The interface is not connected !");
            }
        }

        /// <summary>
        /// Sends the specified command to the Paradox PRT3 module with acknowledgement.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public async Task<bool> SendCommandWithAcknowledgement(string command, int timeout = 2000)
        {
            return await SendCommandWithAcknowledgement(command, new CancellationTokenSource(timeout).Token);
        }

        /// <summary>
        /// Sends the specified command to the Paradox PRT3 module with acknowledgement.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> SendCommandWithAcknowledgement(string command, CancellationToken cancellationToken)
        {
            var taskCompletationSource = new TaskCompletionSource<bool>();
            // Handle result
            var handler = new EventHandler<ParadoxMessageEventArgs>((s, e) =>
            {
                // the first five characters of the command back
                if (command.StartsWith(e.Message.Substring(0, 5)))
                {
                    // followed by “&OK” for valid commands (“&fail” for invalid commands)
                    taskCompletationSource.TrySetResult(e.Message.EndsWith(FLAG_OK));
                }
            });
            // Attach the handler
            this.MessageReceived += handler;
            try
            {
                // Register the cancellation token
                using (cancellationToken.Register(() => taskCompletationSource.TrySetCanceled(), useSynchronizationContext: false))
                {
                    // Send command
                    this.SendCommand(command);
                    // Await completation task
                    return await taskCompletationSource.Task.ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            finally
            {
                // Remove the handler
                this.MessageReceived -= handler;
            }
        }

        #endregion

        #region Internal reading loop

        /// <summary>
        /// Internal loop to read the serial port.
        /// </summary>
        private void ReadSerialPort()
        {
            while (this.IsConnected)
            {
                try
                {
                    // If there is data to read
                    if (this.serialPort.BytesToRead > 0)
                    {
                        // Read incoming datas to the string buffer
                        byte[] buffer = new byte[this.serialPort.BytesToRead];
                        this.serialPort.Read(buffer, 0, this.serialPort.BytesToRead);
                        this.strBuffer += ASCIIEncoding.ASCII.GetString(buffer);
                        this.lastReceivedDatas = DateTime.Now;
                        int nlIndex = -1;
                        // Read each line from the string buffer
                        while ((nlIndex = this.strBuffer.IndexOf('\r')) > 0)
                        {
                            // If there are subscribers
                            if (this.MessageReceived != null)
                            {
                                // Extract the line and build the ParadoxMessageEventArgs
                                var message = new ParadoxMessageEventArgs() { Date = DateTime.Now, Message = this.strBuffer.Substring(0, nlIndex) };
                                // Raise MessageReceived event for each subscribers
                                Delegate[] receivers = this.MessageReceived.GetInvocationList();
                                foreach (EventHandler<ParadoxMessageEventArgs> receiver in receivers)
                                {
                                    receiver.BeginInvoke(this, message, null, null);
                                }
                            }
                            // Remove the message processed from the buffer
                            this.strBuffer = this.strBuffer.Substring(nlIndex + 1);
                        }
                    }

                    // Check anormal buffer content (= data without '\r' in the string buffer since 2000ms)
                    if (this.strBuffer.Length > 0 && this.strBuffer.IndexOf('\r') < 0 && DateTime.Now.Subtract(lastReceivedDatas).TotalMilliseconds >= BUFFER_TIMEOUT)
                    {
                        // Raise error
                        if (this.InterfaceError != null)
                        {
                            this.InterfaceError(this, new InterfaceErrorEventArgs() { Exception = new TimeoutException($"String buffer timeout. The buffer content was '{this.strBuffer}'.") });
                        }
                        // Reset string buffer
                        this.strBuffer = string.Empty;
                    }

                    // Pause at each iteration
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    if (this.InterfaceError != null)
                    {
                        this.InterfaceError(this, new InterfaceErrorEventArgs() { Exception = ex });
                    }
                    this.IsConnected = false;
                }
            }
        }

        #endregion

        #region InterfaceErrorEventArgs nested class

        /// <summary>
        /// Represent an interface error that stop the communication with the PRT3 module
        /// </summary>
        public class InterfaceErrorEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the exception.
            /// </summary>
            /// <value>
            /// The exception.
            /// </value>
            public Exception Exception { get; set; }
        }

        #endregion
    }
}

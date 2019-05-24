using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace RFLinkNet
{
    /// <summary>
    /// Rf stdout data
    /// </summary>
    public class RFEventArgs : EventArgs
    {
        /// <summary>
        /// Data from rflink
        /// </summary>
        public string Data { get; set; }
    }

    /// <summary>
    /// Status of rflink
    /// </summary>
    public enum LibraryStatus
    {
        Ready,
        WaitingForStatus,
        WaitingForVersion,
        NotReady
    }

    /// <summary>
    /// Rflink client manager
    /// </summary>
    public class RFLinkClient : IDisposable
    {
        /// <summary>
        /// RfLink log
        /// </summary>
        public event EventHandler EventLogOut = null;

        /// <summary>
        /// RfLink rf log
        /// </summary>
        public event EventHandler EventRFOut = null;

        /// <summary>
        /// Last pong response to a ping
        /// in > 10s from now, rflink is dead
        /// </summary>
        public DateTime LastPong;

        /// <summary>
        /// RFLink Settings are returned from the device on load
        /// </summary>
        public RFLinkSettings Settings { get; } = new RFLinkSettings();

        private SerialPort serialPort = new SerialPort("COM1", 57600);
        private readonly object receiveLock = new object();
        private bool disposed = false;
        private LibraryStatus libraryStatus = LibraryStatus.NotReady;
        private ManualResetEvent statusReceived = new ManualResetEvent(false);
        private ManualResetEvent versionReceived = new ManualResetEvent(false);

        /// <summary>
        /// Callback for rflink log
        /// </summary>
        /// <param name="text"></param>
        protected void ReturnStdOut(string text)
        {
            EventLogOut?.Invoke(this, new RFEventArgs { Data = text });
        }

        /// <summary>
        /// Callback for rflink rf log
        /// </summary>
        protected void ReturnRFOutput(RFData data)
        {
            EventRFOut?.Invoke(this, data);
        }

        /// <summary>
        /// Construct a RFLink client with default port ("COM1")
        /// </summary>
        public RFLinkClient()
        {
            Setup("COM1");
        }

        /// <summary>
        /// Construct a RFLink client with option to specify port name
        /// </summary>
        /// <param name="port">Port name e.g. COM1 (default)</param>
        public RFLinkClient(string port)
        {
            if (String.IsNullOrEmpty(port))
            {
                throw new ArgumentNullException("port", "RFLinkClient contstructed with null port value");
            }

            Setup(port);
        }

        /// <summary>
        /// Construct RFLink client with existing serial port object
        /// </summary>
        /// <param name="serialPort"></param>
        public RFLinkClient(SerialPort serialPort)
        {
            this.serialPort = serialPort ?? throw new ArgumentNullException();
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>
        /// Dispose the rflink client
        /// </summary>
        ~RFLinkClient()
        {
            Close();
            Dispose(false);
        }

        /// <summary>
        /// Send data to the RFLink serial port
        /// No data will be manipulated 
        /// </summary>
        /// <param name="data">Raw data to send</param>
        /// <param name="repeat">How many times to send data</param>
        public void SendRawData(string data, int repeat = 1)
        {
            foreach (var i in Enumerable.Range(1, repeat))
            {
                serialPort.WriteLine(data);

                // Sleep if we're repeating
                if (repeat > 1)
                {
                    Thread.Sleep(50 + i);
                }
            }

            ReturnStdOut($"Sent ({repeat}):{data}");
        }

        /// <summary>
        /// Close connection with rflink
        /// </summary>
        public void Close()
        {
            serialPort?.Close();
            libraryStatus = LibraryStatus.NotReady;
            versionReceived.Reset();
            statusReceived.Reset();
        }

        /// <summary>
        /// Connect with rflink
        /// </summary>
        /// <returns>true if connected successfully, otherwise false</returns>
        public bool Connect()
        {
            bool connected = false;

            try
            {
                serialPort.Open();

                libraryStatus = LibraryStatus.WaitingForStatus;
                RequestDeviceState(Commands.GetStatus, ref statusReceived, 3);

                libraryStatus = LibraryStatus.WaitingForVersion;
                RequestDeviceState(Commands.GetVersion, ref versionReceived, 3);

                libraryStatus = LibraryStatus.Ready;
                connected = true;
            }
            catch (Exception e)
            {
                ReturnStdOut(e.Message + "  " + e.StackTrace);
                Close();
                throw;
            }

            return connected;
        }

        /// <summary>
        /// Setup the serialport parameters
        /// </summary>
        /// <param name="port"></param>
        private void Setup(string port)
        {
            serialPort.PortName = port;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;

            serialPort.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>
        /// Get status and version of rflink
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="waitevent">event to set value</param>
        /// <param name="attempts">number of attemps</param>
        private void RequestDeviceState(string request, ref ManualResetEvent waitevent, int attempts)
        {
            for (int i = 1; i < attempts; i++)
            {
                SendRawData(Commands.ConstructPacket(request));

                if (waitevent.WaitOne(TimeSpan.FromSeconds(3)))
                {
                    return;
                }
            }

            throw new TimeoutException($"RF did not return after {attempts} attempts: {libraryStatus.ToString()}");
        }

        /// <summary>
        /// Data to be processed when received by the RFlink serial port
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Data received over serial port</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (receiveLock)
            {
                SerialPort sp = sender as SerialPort;
                string indata = sp.ReadLine();

                try
                {
                    RFData rf = ProtocolParser.ProcessData(indata);

                    if (rf != null)
                    {
                        if(!string.IsNullOrWhiteSpace(rf.Protocol) && rf.Protocol == "PONG")
                        {
                            LastPong = DateTime.Now;
                        }
                        else if(!string.IsNullOrWhiteSpace(rf.Protocol) && rf.Protocol == "STATUS")
                        {
                            Settings.ProcessStatusResponse(rf);
                            statusReceived.Set();
                        }
                        else if (!string.IsNullOrWhiteSpace(rf.Protocol) && rf.Protocol.StartsWith("VER"))
                        {
                            Settings.ProcessVerResponse(rf);
                            versionReceived.Set();
                        }
                        else if (libraryStatus == LibraryStatus.Ready)
                        {
                            ReturnRFOutput(rf);
                        }
                    }
                }
                catch (FormatException)
                {
                    // Failed to process settings/version, ignore
                }
                catch (Exception ex)
                {
                    ReturnStdOut($"Invalid data «{indata}». \r\nException {ex.Message} \r\n {ex.StackTrace}");
                }
            }

        }

        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                serialPort?.Dispose();
                statusReceived?.Dispose();
                versionReceived?.Dispose();
            }

            disposed = true;
        }
    }
}

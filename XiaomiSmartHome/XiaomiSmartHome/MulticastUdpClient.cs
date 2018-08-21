using Constellation.Package;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace XiaomiSmartHome
{
    /// <summary>
    /// Multicast UdpClient wrapper with send and receive capabilities.
    /// Usage: pass local and remote multicast IPs and port to constructor.
    /// Use Send method to send data,
    /// subscribe to Received event to get notified about received data.
    /// </summary>
    public class MulticastUdpClient
    {
        UdpClient _udpclient;
        IPEndPoint _gatewayEndPoint;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="multicastIPaddress">Multicast Ip</param>
        /// <param name="port">Multicast Port</param>
        /// <param name="gatewayIpAdress">Ip adress of gateway</param>
        public MulticastUdpClient(IPAddress multicastIPaddress, int port, IPAddress gatewayIpAdress)
        {
            // Create endpoints
            _gatewayEndPoint = new IPEndPoint(gatewayIpAdress, port);

            // Create and configure UdpClient
            _udpclient = new UdpClient();
            // The following three lines allow multiple clients on the same PC
            _udpclient.ExclusiveAddressUse = false;
            _udpclient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpclient.ExclusiveAddressUse = false;
            // Bind, Join
            _udpclient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            _udpclient.JoinMulticastGroup(multicastIPaddress);

            PackageHost.WriteInfo("Starting to listen multicast on gateway multicast IP");

            // Start listening for incoming data
            _udpclient.BeginReceive(new AsyncCallback(ReceivedCallback), null);
        }

        /// <summary>
        /// Send the buffer by UDP to multicast address
        /// </summary>
        /// <param name="bufferToSend">Buffer to send over udp</param>
        public string SendToGateway(byte[] bufferToSend)
        {
            // TODO 
            //  - understand why i have to use new client 
            //  - understand why i have to receive now rather than asynchronous listening
            UdpClient client = new UdpClient();
            var ep = new IPEndPoint(IPAddress.Any, 9898);
            client.Send(bufferToSend, bufferToSend.Length, _gatewayEndPoint);
            var receivedData = client.Receive(ref ep);
            return Encoding.UTF8.GetString(receivedData).Trim();
        }

        /// <summary>
        /// Callback which is called when UDP packet is received
        /// </summary>
        /// <param name="ar">Received data</param>
        private void ReceivedCallback(IAsyncResult ar)
        {
            // Get received data
            IPEndPoint sender = new IPEndPoint(0, 0);
            Byte[] receivedBytes = _udpclient.EndReceive(ar, ref sender);

            // fire event if defined
            UdpMessageReceived?.Invoke(this, new UdpMessageReceivedEventArgs() { Buffer = receivedBytes });

            // Restart listening for udp data packages
            _udpclient.BeginReceive(new AsyncCallback(ReceivedCallback), null);
        }

        /// <summary>
        /// Event handler which will be invoked when UDP message is received
        /// </summary>
        public event EventHandler<UdpMessageReceivedEventArgs> UdpMessageReceived;

        /// <summary>
        /// Arguments for UdpMessageReceived event handler
        /// </summary>
        public class UdpMessageReceivedEventArgs : EventArgs
        {
            /// <summary>
            /// Buffer of received data
            /// </summary>
            public byte[] Buffer { get; set; }
        }
    }
}


namespace OpenSource.UPnP
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal class UPnPSearchSniffer
    {
        public static IPAddress UpnpMulticastV4Addr = IPAddress.Parse("239.255.255.250");
        public static IPAddress UpnpMulticastV6Addr1 = IPAddress.Parse("FF05::C"); // Site local
        public static IPAddress UpnpMulticastV6Addr2 = IPAddress.Parse("FF02::C"); // Link local

        public static IPEndPoint UpnpMulticastV4EndPoint = new IPEndPoint(UpnpMulticastV4Addr, 1900);
        public static IPEndPoint UpnpMulticastV6EndPoint1 = new IPEndPoint(UpnpMulticastV6Addr1, 1900);
        public static IPEndPoint UpnpMulticastV6EndPoint2 = new IPEndPoint(UpnpMulticastV6Addr2, 1900);

        public delegate void PacketHandler(object sender, string Packet, IPEndPoint Local, IPEndPoint From);
        public event PacketHandler OnPacket;
        protected Hashtable SSDPSessions = new Hashtable();

        public UPnPSearchSniffer()
        {
            IPAddress[] LocalAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            ArrayList temp = new ArrayList();
            foreach (IPAddress i in LocalAddresses) temp.Add(i);
            temp.Add(IPAddress.Loopback);
            LocalAddresses = (IPAddress[])temp.ToArray(typeof(IPAddress));

            for (int id = 0; id < LocalAddresses.Length; ++id)
            {
                try
                {
                    UdpClient ssdpSession = new UdpClient(new IPEndPoint(LocalAddresses[id], 0));
                    ssdpSession.EnableBroadcast = true;

                    if (!IsMono())
                    {
                        uint IOC_IN = 0x80000000;
                        uint IOC_VENDOR = 0x18000000;
                        uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                        ssdpSession.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                    }

                    ssdpSession.BeginReceive(new AsyncCallback(OnReceiveSink), ssdpSession);
                    SSDPSessions[ssdpSession] = ssdpSession;
                }
                catch (Exception) { }
            }
        }

        public void Search(string SearchString)
        {
            Search(SearchString, UpnpMulticastV4EndPoint);
            Search(SearchString, UpnpMulticastV6EndPoint1); // Site local
            Search(SearchString, UpnpMulticastV6EndPoint2); // Link local
        }

        public void SearchV4(string SearchString)
        {
            Search(SearchString, UpnpMulticastV4EndPoint);
        }

        public void SearchV6(string SearchString)
        {
            Search(SearchString, UpnpMulticastV6EndPoint1); // Site local
            Search(SearchString, UpnpMulticastV6EndPoint2); // Link local
        }

        public void Search(string SearchString, IPEndPoint ep)
        {
            HTTPMessage request = new HTTPMessage();
            request.Directive = "M-SEARCH";
            request.DirectiveObj = "*";
            if (ep.AddressFamily == AddressFamily.InterNetwork) request.AddTag("HOST", ep.ToString()); // "239.255.255.250:1900"
            if (ep.AddressFamily == AddressFamily.InterNetworkV6) request.AddTag("HOST", string.Format("[{0}]:{1}", ep.Address.ToString(), ep.Port)); // "[FF05::C]:1900" or "[FF02::C]:1900"
            request.AddTag("MAN", "\"ssdp:discover\"");
            request.AddTag("MX", "10");
            request.AddTag("ST", SearchString);
            SearchEx(UTF8Encoding.UTF8.GetBytes(request.StringPacket), ep);
        }

        public void SearchEx(string text, IPEndPoint ep)
        {
            SearchEx(UTF8Encoding.UTF8.GetBytes(text), ep);
        }

        public void SearchEx(byte[] buf, IPEndPoint ep)
        {
            foreach (UdpClient ssdpSession in SSDPSessions.Values)
            {
                try
                {
                    if (ssdpSession.Client.AddressFamily != ep.AddressFamily) continue;
                    if ((ssdpSession.Client.AddressFamily == AddressFamily.InterNetworkV6) && (((IPEndPoint)ssdpSession.Client.LocalEndPoint).Address.IsIPv6LinkLocal == true && ep.Address != Utils.UpnpMulticastV6Addr2)) continue;
                    if ((ssdpSession.Client.AddressFamily == AddressFamily.InterNetworkV6) && (((IPEndPoint)ssdpSession.Client.LocalEndPoint).Address.IsIPv6LinkLocal == false && ep.Address != Utils.UpnpMulticastV6Addr1)) continue;

                    IPEndPoint lep = (IPEndPoint)ssdpSession.Client.LocalEndPoint; // Seems can throw: System.Net.Sockets.SocketException: The requested address is not valid in its context
                    if (ssdpSession.Client.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ssdpSession.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, lep.Address.GetAddressBytes());
                    }
                    else if (ssdpSession.Client.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        ssdpSession.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastInterface, BitConverter.GetBytes((int)lep.Address.ScopeId));
                    }

                    ssdpSession.Send(buf, buf.Length, ep);
                    ssdpSession.Send(buf, buf.Length, ep);
                }
                catch (SocketException) { }
            }
        }

        public void OnReceiveSink(IAsyncResult ar)
        {
            IPEndPoint ep = null;
            UdpClient client = (UdpClient)ar.AsyncState;
            byte[] buf = null;
            try
            {
                buf = client.EndReceive(ar, ref ep);
            }
            catch (Exception) { }
            try
            {
                if (buf != null && OnPacket != null) OnPacket(this, UTF8Encoding.UTF8.GetString(buf, 0, buf.Length), (IPEndPoint)client.Client.LocalEndPoint, ep);
            }
            catch (Exception) { }
            try
            {
                client.BeginReceive(new AsyncCallback(OnReceiveSink), client);
            }
            catch (Exception) { }
        }

        private static bool MonoDetected = false;
        private static bool MonoActive = false;
        public static bool IsMono()
        {
            if (MonoDetected) return MonoActive;
            MonoActive = (Type.GetType("Mono.Runtime") != null);
            MonoDetected = true;
            return MonoActive;
        }
    }
}

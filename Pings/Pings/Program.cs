using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Pings
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);

        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
        }

        [MessageCallback(Description = "Send a ping to the target.")]
        bool Check_Ping(string Target)
        {

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(Target, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                PackageHost.WriteInfo(Target + " is pingable");
                return true;

            }
            else
            {
                PackageHost.WriteInfo(Target + " is not pingable");
                return false;

            }

        }

        [MessageCallback(Description = "Check if port is open on target.")]
        bool Check_Port(PortInfo address)
        {

            try
            {
                TcpClient client = new TcpClient(address.IP_address, address.Port);
                PackageHost.WriteInfo(address.IP_address + ":" + address.Port + " is open");
                return true;
            }
            catch (Exception ex)
            {
                PackageHost.WriteInfo(address.IP_address + ":" + address.Port + " is close");
                return false;
            }

        }

        public class PortInfo
        {
            /// <summary>
            /// IP address
            /// </summary>
            public string IP_address { get; set; }

            /// <summary>
            /// Port
            /// </summary>
            public int Port { get; set; }
        }
    }
}

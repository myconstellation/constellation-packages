using Constellation.Package;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace XiaomiSmartHome.Method
{
    public class Response
    {
        public string cmd { get; set; }
        public string model { get; set; }
        public string sid { get; set; }
        public int short_id { get; set; }
        public string token { get; set; }
        public string data { get; set; }

        [MessageCallback]
        public static Response ReadEquipement(string GatewayIP, string Mac)
        {
            //// We create an UDP client
            UdpClient Read = new UdpClient();

            //// Gateway IP adress as endpoint
            IPEndPoint gateway = new IPEndPoint(IPAddress.Parse(GatewayIP), 9898);

            //// Constellation IP adress as endpoint
            IPEndPoint constellation = (IPEndPoint)Read.Client.LocalEndPoint;

            //// Command to sent
            string command = string.Format("{{\"cmd\":\"read\",\"sid\":\"{0}\"}}", Mac);
            Byte[] buffer = Encoding.ASCII.GetBytes(command);

            //// Send command to gateway
            Read.Send(buffer, buffer.Length, gateway);

            //// Receive data
            var receivedData = Read.Receive(ref constellation);
            string returnData = Encoding.UTF8.GetString(receivedData).Trim();

            //// Deserialize it to Response class
            Response result = JsonConvert.DeserializeObject<Response>(returnData);
            return result;
        }
    }
}

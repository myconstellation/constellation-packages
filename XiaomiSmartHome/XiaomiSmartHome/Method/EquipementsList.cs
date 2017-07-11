using Newtonsoft.Json;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web.Script.Serialization;

namespace XiaomiSmartHome.Method
{
    public class EquipementsList
    {

        public string cmd { get; set; }
        public string sid { get; set; }
        public string token { get; set; }
        public string data { get; set; }

        //// Query the gateway for a list of equipements
        public static EquipementsList FindEquipementsList(string GatewayIP)
        {
            //// We create an UDP client
            UdpClient FindList = new UdpClient();

            //// Gateway IP adress as endpoint
            IPEndPoint gateway = new IPEndPoint(IPAddress.Parse(GatewayIP), 9898);

            //// Constellation IP adress as endpoint
            IPEndPoint constellation = (IPEndPoint)FindList.Client.LocalEndPoint;

            //// Command to sent
            string command = @"{""cmd"":""get_id_list""}";
            Byte[] buffer = Encoding.ASCII.GetBytes(command);

            //// Send command to gateway
            FindList.Send(buffer, buffer.Length, gateway);

            //// Receive data
            var receivedData = FindList.Receive(ref constellation);
            string returnData = Encoding.UTF8.GetString(receivedData).Trim();

            //// Deserialize it to EquipementsList class
            EquipementsList result = JsonConvert.DeserializeObject<EquipementsList>(returnData);
            return result;
        }

        public static string[] GetListData(string data)
        {
            JavaScriptSerializer result = new JavaScriptSerializer();
            return result.Deserialize<string[]>(data);
        }

    }

}

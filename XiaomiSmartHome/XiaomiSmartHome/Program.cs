using Constellation;
using Constellation.Package;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using XiaomiSmartHome.Model;
using XiaomiSmartHome.Equipement;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace XiaomiSmartHome
{
    public class Program : PackageBase
    {
        //// New Gateway class
        private Gateway gateway;
     
        //// Equipement name
        private string name = null;

        //// Equipements array
        private string[] equipements;

        //// Call at start
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        //// Main function
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            
            //// Creating a new gateway report
            gateway = new Gateway();
            
            //// Pushing gateway SO
            PackageHost.PushStateObject<Gateway>("Gateway", gateway);
            
            //// Starting connexion in a thread
            Thread connexion = new Thread(new ThreadStart(this.StartListener));
            connexion.Start();
            
            //// Getting equipements
            this.GetEquipements();
        }

        //// Listening multicast to get reports
        private void StartListener()
        {
            //// Launching multicast listener on gateway multicast IP
            PackageHost.WriteInfo("Starting to listen multicast on gateway multicast IP");
            UdpClient client = new UdpClient();
            IPAddress multicastaddress = IPAddress.Parse(PackageHost.GetSettingValue<string>("GatewayMulticastIP"));
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, PackageHost.GetSettingValue<int>("GatewayMulticastPort"));
            client.ExclusiveAddressUse = false;
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(localEp);
            try
            {
                //// Join multicast
                client.JoinMulticastGroup(multicastaddress);
                PackageHost.WriteInfo("Listening start...");
                while (PackageHost.IsRunning)
                {
                    try
                    {
                        //// We convert byte to string
                        byte[] bytes = client.Receive(ref localEp);
                        string response = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                        
                        //// We deserialize to Response class
                        Response data = JsonConvert.DeserializeObject<Response>(response);
                        
                        //// If command equal heartbeat
                        if(data.Cmd == "heartbeat")
                        {
                            //// We print response if heartbeat logs is true
                            if (PackageHost.GetSettingValue<bool>("HeartbeatLog"))
                            {
                                PackageHost.WriteInfo("{0}", response);
                            }

                            //// If heartbeat model is gateway, we save the data part to Gateway class
                            if (data.Model == "gateway")
                            {
                                //// We save the token
                                gateway.Token = data.Token;
                                
                                //// We deserialize data part
                                GatewayHeartbeat gateway_data = JsonConvert.DeserializeObject<GatewayHeartbeat>(data.Data);
                                
                                //// If we didn't set gateway's sid
                                if (gateway.Sid == null)
                                {
                                    //// If data ip equal to gateway ip
                                    if (gateway_data.IP == PackageHost.GetSettingValue<string>("GatewayIP"))
                                    {
                                        gateway.Sid = data.Sid;
                                    }
                                }
                                
                                //// We push the gateway SO
                                PackageHost.PushStateObject<Gateway>("Gateway", gateway);
                            }
                        }
                        //// If command equal report
                        else if (data.Cmd == "report")
                        {
                            //// We print response if report logs is true
                            if (PackageHost.GetSettingValue<bool>("ReportLog"))
                            {
                                PackageHost.WriteInfo("{0}", response);
                            }
                            
                            //// If model equal gateway
                            if ((data.Model == "gateway") && (data.Sid == gateway.Sid))
                            {
                                //// We deserialize report data as gateway class
                                GatewayReport datagate = JsonConvert.DeserializeObject<GatewayReport>(data.Data);
                                gateway.Report = datagate;
                               
                                //// We push the gateway SO
                                PackageHost.PushStateObject<Gateway>("Gateway", gateway);
                            }
                            else
                            {
                                //// If equipement sid is in the equipements list
                                if (equipements.Contains(data.Sid))
                                {
                                    this.SaveEquipement(data.Sid);
                                }
                            }
                        }
                        Thread.Yield();
                    }
                    catch(Exception ex)
                    {
                        PackageHost.WriteError("Error with receive message : {0}", ex);
                    }
                }
            }
            catch(Exception ex)
            {
                PackageHost.WriteError("Error joigning multicast : {0}", ex);
                return;
            }
        }

        /// <summary>
        /// Get equipements from the gateway
        /// </summary>
        private void GetEquipements()
        {
            //// Get equipements list
            EquipementsList list = this.FindEquipementsList();
            
            //// Get data part
            equipements = GetListData(list.Data);
            
            //// Foreach equipement in data
            foreach (string equipement in equipements)
            {
                this.SaveEquipement(equipement);
            }
        }

        /// <summary>
        /// Get equipement values then save it to Constellation
        /// </summary>
        private void SaveEquipement(string mac)
        {
            Response read = null;
            Type modelType = null;
            Type modelReportType = null;
            dynamic model = null;
            dynamic modelReport = null;
            
            //// Try to get setting based on equipement SID
            if (!PackageHost.TryGetSettingValue<string>(mac, out name))
            {
                name = mac;
            }

            try
            {
                //// Get response from read function
                read = this.ReadEquipement(mac);
                
                //// Create instance of model class
                modelType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(t => t.GetCustomAttribute<Response.XiaomiEquipementAttribute>()?.Model == read.Model);

                if (modelType == null)
                {
                    PackageHost.WriteError("{0} type not found !", read.Model);
                    return;
                }

                model = Activator.CreateInstance(modelType);
                
                //// Create report instance of model class 
                modelReportType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(t => t.GetCustomAttribute<Response.XiaomiEquipementAttribute>()?.Model == read.Model + "_report");

                if (modelReportType == null)
                {
                    PackageHost.WriteError("{0}_report type not found !", read.Model);
                    return;
                }

                modelReport = Activator.CreateInstance(modelReportType);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error ReadEquipement : {0}", ex);
                return;
            }

            //// Deserialize data part of the report
            dynamic data = JsonConvert.DeserializeObject(Convert.ToString(read.Data), modelReportType);
           
            //// Add informations to Report class
            model.BatteryLevel = System.Convert.ToInt32(System.Math.Floor((data.Voltage - 2800) * 0.33));
            model.Sid = mac;
            model.Report = data;
            
            //// Pushing the SO
            PackageHost.PushStateObject<dynamic>(name, model);
        }

        /// <summary>
        /// Get equipement values from gateway
        /// </summary>
        /// <param name="sid">SID of the equipement.</param>
        [MessageCallback]
        public Response ReadEquipement(string sid)
        {
            //// Init result
            Response result = null;

            //// Create UDP client
            UdpClient read = new UdpClient();

            //// Gateway IP adress as endpoint
            IPEndPoint gateway = new IPEndPoint(IPAddress.Parse(PackageHost.GetSettingValue<string>("GatewayIP")), PackageHost.GetSettingValue<int>("GatewayMulticastPort"));         
           
            //// Constellation IP adress as endpoint
            IPEndPoint constellation = (IPEndPoint)read.Client.LocalEndPoint;
           
            //// Command to sent
            string command = string.Format("{{\"cmd\":\"read\",\"sid\":\"{0}\"}}", sid);
            Byte[] buffer = Encoding.ASCII.GetBytes(command);

            try
            {
                //// Send command to gateway
                read.Send(buffer, buffer.Length, gateway);
               
                //// Receive data
                var receivedData = read.Receive(ref constellation);
                string returnData = Encoding.UTF8.GetString(receivedData).Trim();
                
                //// Deserialize it to Response class
                result = JsonConvert.DeserializeObject<Response>(returnData);
                return result;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error ReadEquipement : {0}", ex);
                return result;
            }
        }

        /// <summary>
        /// Get equipements list from gateway
        /// </summary>
        [MessageCallback]
        public EquipementsList FindEquipementsList()
        {
            //// Init result
            EquipementsList result = null;
             
            //// Create an UDP client
            UdpClient findList = new UdpClient();
           
            //// Gateway IP adress as endpoint
            IPEndPoint gateway = new IPEndPoint(IPAddress.Parse(PackageHost.GetSettingValue<string>("GatewayIP")), PackageHost.GetSettingValue<int>("GatewayMulticastPort"));
           
            //// Constellation IP adress as endpoint
            IPEndPoint constellation = (IPEndPoint)findList.Client.LocalEndPoint;
            
            //// Command to sent
            string command = @"{""cmd"":""get_id_list""}";
            Byte[] buffer = Encoding.ASCII.GetBytes(command);

            try
            {
                //// Send command to gateway
                findList.Send(buffer, buffer.Length, gateway);
                
                //// Receive data
                var receivedData = findList.Receive(ref constellation);
                string returnData = Encoding.UTF8.GetString(receivedData).Trim();
               
                //// Deserialize it to EquipementsList class
                result = JsonConvert.DeserializeObject<EquipementsList>(returnData);
                return result;
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error FindEquipementList : {0}", ex);
                return result;
            }
        }
        
        /// <summary>
        /// Get data part as array
        /// </summary>
        /// <param name="data"></param>
        private static string[] GetListData(string data)
        {
            JavaScriptSerializer result = new JavaScriptSerializer();
            return result.Deserialize<string[]>(data);
        }

        /// <summary>
        /// Get equipements list from gateway
        /// </summary>
        [MessageCallback]
        public void GatewayRgb()
        {

            string original = "1234567890abcdef";
            string token = "0987654321qwerty";
            
            string encrypted = Cryptography.Encrypt(token, original);
            PackageHost.WriteWarn("{0}", encrypted);
        }

        public static string Encrypt(string value, string password) {
            return Encrypt<AesManaged>(value, password);
        }
        public static string Encrypt<T>(string value, string password) 
                where T : SymmetricAlgorithm, new() {
            byte[] vectorBytes = GetBytes<ASCIIEncoding>(_vector);
            byte[] saltBytes = GetBytes<ASCIIEncoding>(_salt);
            byte[] valueBytes = GetBytes<UTF8Encoding>(value);

            byte[] encrypted;
            using (T cipher = new T()) {
                PasswordDeriveBytes _passwordBytes = 
                    new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes)) {
                    using (MemoryStream to = new MemoryStream()) {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write)) {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string value, string password) {
            return Decrypt<AesManaged>(value, password);
        }
        public static string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new() {
            byte[] vectorBytes = GetBytes<ASCIIEncoding>(_vector);
            byte[] saltBytes = GetBytes<ASCIIEncoding>(_salt);
            byte[] valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T()) {
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes)) {
                        using (MemoryStream from = new MemoryStream(valueBytes)) {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read)) {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                } catch (Exception ex) {
                    return String.Empty;
                }

                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

    }
}


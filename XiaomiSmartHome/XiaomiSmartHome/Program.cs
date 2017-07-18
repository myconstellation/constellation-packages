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

            // Create a new instance of the Aes 
            // class.  This generates a new key and initialization  
            // vector (IV). 
            using (var random = new RNGCryptoServiceProvider())
            {
                var key = new byte[16];
                random.GetBytes(key);

                // Encrypt the string to an array of bytes. 
                byte[] encrypted = EncryptStringToBytes_Aes(original, key);

                //Display the original data and the decrypted data.
                PackageHost.WriteWarn("Original:   {0}", original);
                PackageHost.WriteWarn("Convert:   {0}", Encoding.Default.GetString(encrypted));
                PackageHost.WriteWarn("Encrypted (b64-encode): {0}", Convert.ToBase64String(encrypted));
            }

        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key)
        {
            byte[] encrypted;
            byte[] IV;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                aesAlg.Mode = CipherMode.CBC;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            var combinedIvCt = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);

            // Return the encrypted bytes from the memory stream. 
            return combinedIvCt;

        }
    }
}

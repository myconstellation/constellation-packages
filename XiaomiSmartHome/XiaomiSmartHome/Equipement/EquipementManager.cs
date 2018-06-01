using Constellation.Package;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using XiaomiSmartHome.Model;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Manager for all equipements
    /// </summary>
    public class EquipementManager
    {
        #region PROPERTIES
        /// <summary>
        /// Wrapper to communicate with gateway
        /// </summary>
        MulticastUdpClient udpClientWrapper;

        /// <summary>
        /// Equipment list connected to gateway
        /// </summary>
        public List<dynamic> lEquipements = new List<dynamic>();

        /// <summary>
        /// Get the gateway in equipement list
        /// TODO multiple gateway support
        /// </summary>
        public Gateway gateway
        {
            get
            {
                return lEquipements.First(cur => cur.Model.Equals(Constants.GATEWAY));
            }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="udpClient">The Wrapper to communicate with gateway</param>
        public EquipementManager(MulticastUdpClient udpClient)
        {
            udpClientWrapper = udpClient;
        }
        #endregion

        /// <summary>
        /// Init all equipement and gateway
        /// </summary>
        public void InitEquipements()
        {
            Gateway gateway = new Gateway();
            lEquipements.Add(gateway);

            // Load all equipements
            this.FindEquipementsList();
        }

        /// <summary>
        /// Send request to get id of items connected to gateway
        /// </summary>
        public void FindEquipementsList()
        {
            try
            {
                this.SendCommand(Constants.GET_ID_LISTE, null, null);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error FindEquipementList : {0}", ex);
            }
        }

        /// <summary>
        /// Send to gateway command to read one equipement
        /// </summary>
        /// <param name="sid">SID of the equipement.</param>
        public void ReadEquipement(string sid)
        {
            try
            {
                this.SendCommand(Constants.READ, sid, null);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error ReadEquipement : {0}", ex);
            }
        }

        /// <summary>
        /// Process message according to the command
        /// </summary>
        /// <param name="resp">Data received from gateway</param>
        public void ProcessUdpDiagram(string resp)
        {
            dynamic model;

            try
            {
                Response reponse = JsonConvert.DeserializeObject<Response>(resp);

                if (reponse.Cmd.Equals(Constants.HEARTBEAT) && PackageHost.GetSettingValue<bool>(Constants.HEARTBEAT_LOG))
                {
                    PackageHost.WriteInfo("{0}", resp);
                }

                if (reponse.Cmd.Equals(Constants.REPORT) && PackageHost.GetSettingValue<bool>(Constants.REPORT_LOG))
                {
                    PackageHost.WriteInfo("{0}", resp);
                }

                switch (reponse.Cmd)
                {
                    // Id list acknowledgement : we read items
                    case Constants.GET_ID_LISTE_ACK:
                        // Set gateway data
                        this.gateway.Sid = reponse.Sid;
                        this.gateway.Token = reponse.Token;
                        PackageHost.PushStateObject<Gateway>(Constants.GATEWAY, gateway);

                        // Init other equipements
                        foreach (string equipementSid in new JavaScriptSerializer().Deserialize<string[]>(reponse.Data))
                        {
                            this.ReadEquipement(equipementSid);
                        }
                        break;

                    // Read acknowledgement : we create equipements
                    case Constants.READ_ACK:
                    default:
                        model = this.GetModel(reponse.Model);
                        dynamic data = JsonConvert.DeserializeObject(reponse.Data, this.GetModelReportType(reponse.Model));
                        model.BatteryLevel = ParseVoltage(data.Voltage);
                        model.Sid = reponse.Sid;
                        model.Report = data;
                        lEquipements.Add(model);
                        this.PushStateObject(reponse.Sid, model);
                        break;

                    // Report : we set report
                    case Constants.REPORT:
                        model = lEquipements.SingleOrDefault(cur => cur.Sid != null && cur.Sid.Equals(reponse.Sid));
                        dynamic parse = JsonConvert.DeserializeObject(reponse.Data, this.GetModelReportType(reponse.Model));
                        model.Report = parse;
                        this.PushStateObject(reponse.Sid, model);
                        break;

                    // For hearbeat we update equipement.
                    case Constants.HEARTBEAT:
                        model = lEquipements.SingleOrDefault(cur => cur.Sid != null && cur.Sid.Equals(reponse.Sid));
                        JsonConvert.PopulateObject(resp, model);
                        this.PushStateObject(reponse.Sid, model);
                        break;

                    case Constants.WRITE_ACK:
                        // {"cmd":"write_ack","model":"gateway","sid":"xxxxx","short_id":0,"data":"{\"rgb\":0,\"illumination\":1292,\"proto_version\":\"1.0.9\"}"}
                        break;
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error SaveEquipement : {0}", ex);
                return;
            }
        }

        /// <summary>
        /// Send command to gateway
        /// </summary>
        /// <returns></returns>
        public void SendCommand(string cmd, string sid, Dictionary<string, object> lParam)
        {
            dynamic model = lEquipements.SingleOrDefault(cur => cur.Sid != null && cur.Sid.Equals(sid));

            // Add key to command data
            lParam?.Add("key", this.GetActualKey());
            Command command = new Command
            {
                Cmd = cmd,
                Model = model?.Model,
                Sid = sid,
                Short_id = model?.ShortId,
                Key = model?.Model == Constants.GATEWAY ? "8" : null, // Why 8 ? what if not present ?
                Data = lParam
            };

            string scommand = JsonConvert.SerializeObject(command, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            ProcessUdpDiagram(this.udpClientWrapper.SendToGateway(Encoding.ASCII.GetBytes(scommand)));
        }

        #region PRIVATE METHODS

        /// <summary>
        /// Push state object to constellation
        /// </summary>
        /// <param name="sid">Model sid</param>
        /// <param name="model">the model</param>
        private void PushStateObject(string sid, dynamic model)
        {
            string name;
            if (model.Model.Equals(Constants.GATEWAY))
            {
                name = Constants.GATEWAY;
            }
            //// Get personal name based on user setting
            else if (!PackageHost.TryGetSettingValue<string>(sid, out name))
            {
                name = sid;
            }

            PackageHost.PushStateObject<dynamic>(name, model);
        }

        /// <summary>
        /// Get instance of model type
        /// </summary>
        /// <param name="model">The model type</param>
        /// <returns></returns>
        private dynamic GetModel(string model)
        {
            Type modelType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(t => t.GetCustomAttribute<Response.XiaomiEquipementAttribute>()?.Model == model);
            if (modelType == null)
            {
                PackageHost.WriteError("{0} type not found !", model);
                return null;
            }

            return Activator.CreateInstance(modelType);
        }

        /// <summary>
        /// Get type of model report
        /// </summary>
        /// <param name="model">The model type</param>
        /// <returns></returns>
        private Type GetModelReportType(string model)
        {
            Type modelReportType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(t => t.GetCustomAttribute<Response.XiaomiEquipementAttribute>()?.Model == model + "_report");
            if (modelReportType == null)
            {
                PackageHost.WriteError("{0}_report type not found !", model);
                return null;
            }

            return modelReportType;
        }

        /// <summary>
        /// Generate current key to send command at gateway
        /// </summary>
        /// <returns>Actual key</returns>
        private string GetActualKey()
        {
            byte[] IV = { 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58, 0x56, 0x2e };
            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            string gwPassword = PackageHost.GetSettingValue<string>(Constants.GATEWAY_PASSWORD);
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(Encoding.ASCII.GetBytes(gwPassword), IV);

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream
            (
                memoryStream,
                encryptor,
                CryptoStreamMode.Write
            );

            byte[] gwToken = Encoding.ASCII.GetBytes(gateway.Token);

            // Start encrypting.
            cryptoStream.Write(gwToken, 0, gwToken.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = BitConverter.ToString(cipherTextBytes, 0, 16).Replace("-", string.Empty);

            // Return encrypted string.
            return cipherText;
        }

        /// <summary>
        /// Get percent battery left
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        public int ParseVoltage(int voltage)
        {
            int maxVolt = 3300;
            int minVolt = 2800;
            if (voltage > maxVolt) voltage = maxVolt;
            if (voltage < minVolt) voltage = minVolt;
            return (int)Math.Round((((decimal)(voltage - minVolt) / (decimal)(maxVolt - minVolt)) * 100));
        }

        #endregion
    }
}

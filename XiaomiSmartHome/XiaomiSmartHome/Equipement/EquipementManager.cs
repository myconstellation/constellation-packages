using Constellation.Package;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using XiaomiSmartHome.Model;
using static XiaomiSmartHome.Enums;

namespace XiaomiSmartHome.Equipement
{
    /// <summary>
    /// Manager for all equipements
    /// </summary>
    public class EquipementManager
    {
        #region PROPERTIES
        /// <summary>
        /// Chemin des logs
        /// </summary>
        private string logPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\log.txt";

        /// <summary>
        /// Wrapper to communicate with gateway
        /// </summary>
        MulticastUdpClient udpClientWrapper;

        /// <summary>
        /// Equipment list connected to gateway
        /// </summary>
        public static List<Equipment> lEquipements = new List<Equipment>();

        /// <summary>
        /// Get the gateway in equipement list
        /// TODO multiple gateway support
        /// </summary>
        public Gateway Gateway
        {
            get
            {
                return lEquipements.First(cur => cur.Model.Equals(EquipmentType.Gateway)) as Gateway;
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
            Gateway gateway = new Gateway
            {
                Model = EquipmentType.Gateway
            };
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
                this.SendCommand(CommandType.GetIdList, null, null);
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
                this.SendCommand(CommandType.Read, sid, null);
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
            //File.AppendAllText("data.txt", resp + "\r\n");

            try
            {
                Response reponse = JsonConvert.DeserializeObject<Response>(resp);

                if (reponse.Cmd.Equals(CommandType.Heartbeat) && PackageHost.GetSettingValue<bool>(Constants.HEARTBEAT_LOG))
                {
                    PackageHost.WriteInfo("{0}", resp);
                }

                if (reponse.Cmd.Equals(CommandType.Report) && PackageHost.GetSettingValue<bool>(Constants.REPORT_LOG))
                {
                    PackageHost.WriteInfo("{0}", resp);
                }

                switch (reponse.Cmd)
                {
                    // Id list acknowledgement : we read items
                    case CommandType.GetIdListAck:
                        // Set gateway data
                        this.Gateway.Sid = reponse.Sid;
                        this.Gateway.Token = reponse.Token;
                        PackageHost.PushStateObject<Gateway>(EquipmentType.Gateway.GetRealName(), Gateway, lifetime: 20);

                        // Init other equipements
                        foreach (string equipementSid in JsonConvert.DeserializeObject<string[]>(reponse.Data))
                        {
                            this.ReadEquipement(equipementSid);
                        }
                        break;

                    // Read acknowledgement : we create equipements
                    case CommandType.ReadAck:
                        Type type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(cur => cur.Name.ToLower().Equals(reponse.Model.ToString().ToLower()));
                        if (type != null)
                        {
                            // En cas de refresh via un read, on essaie de récup l’item dans la liste.
                            if(lEquipements.Any(cur => cur.Sid != null && cur.Sid.Equals(reponse.Sid)))
                            {
                                goto case CommandType.Report;
                            }

                            // Sinon on l’ajoute
                            dynamic model = JsonConvert.DeserializeObject(resp, type);
                            model.Update(JsonConvert.DeserializeObject(reponse.Data, type), reponse.Cmd.ToString());
                            if (!lEquipements.Any(cur => cur.Sid.Equals(model.Sid))) lEquipements.Add(model);
                            this.PushStateObject(reponse.Sid, model);
                        }
                        break;

                    // We update equipment
                    case CommandType.Report:
                    case CommandType.Heartbeat:
                    case CommandType.WriteAck:
                        // write ack plug : if set on => read to get LoadPower ?
                        if (reponse.Cmd.Equals(CommandType.WriteAck) && reponse.Data.Contains("error"))
                        {
                            PackageHost.WriteError("write ack error : {0}", resp);
                        }
                        else
                        {
                            Equipment equipment = lEquipements.SingleOrDefault(cur => cur.Sid != null && cur.Sid.Equals(reponse.Sid));

                            // Update gateway token
                            if (equipment.Model.Equals(EquipmentType.Gateway) && reponse.Cmd.Equals(CommandType.Heartbeat))
                            {
                                (equipment as Gateway).Token = reponse.Token;
                            }

                            equipment.Update(JsonConvert.DeserializeObject(reponse.Data, equipment.GetType()), reponse.Cmd.ToString());
                            this.PushStateObject(reponse.Sid, equipment);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error ProcessUdpDiagram : {0}", ex);
                return;
            }
        }

        /// <summary>
        /// Send command to gateway
        /// </summary>
        /// <returns></returns>
        public void SendCommand(CommandType cmd, string sid, Dictionary<string, object> lParam)
        {
            Equipment model = lEquipements.SingleOrDefault(cur => cur.Sid != null && cur.Sid.Equals(sid));

            // Add key to command data
            lParam?.Add("key", this.GetActualKey());
            Command command = new Command
            {
                Cmd = cmd.GetRealName(),
                Model = model?.Model.GetRealName(),
                Sid = sid,
                Short_id = model?.ShortId,
                Key = model?.Model == EquipmentType.Gateway ? "8" : null, // Why 8 ? what if not present ?
                Data = lParam
            };

            string sCommand = JsonConvert.SerializeObject(command, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            ProcessUdpDiagram(this.udpClientWrapper.SendToGateway(Encoding.ASCII.GetBytes(sCommand)));
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
            if (model.Model.Equals(EquipmentType.Gateway))
            {
                name = EquipmentType.Gateway.GetRealName();
            }
            else
            {
                name = Program.GetCustomSoName(sid);
            }
            
            PackageHost.PushStateObject<dynamic>(name, model, lifetime: 3600);
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

            byte[] gwToken = Encoding.ASCII.GetBytes(Gateway.Token);

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

        #endregion
    }
}

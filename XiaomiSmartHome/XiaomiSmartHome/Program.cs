using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft;
using Newtonsoft.Json;
using XiaomiSmartHome.Method;
using XiaomiSmartHome.Equipement;

namespace XiaomiSmartHome
{
    public class Program : PackageBase
    {

        string GatewayIP = null;
        gateway Gateway;

        public Boolean HeartBeatLog = false;
        public Boolean ReportLog = false;

        string name = null;
        string[] equipements;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            //// Creating a new gateway report
            Gateway = new gateway();

            //// Pushing gateway SO
            PackageHost.PushStateObject<gateway>("Gateway", Gateway);

            //// Starting connexion in a thread
            Thread connexion = new Thread(new ThreadStart(Connexion));
            connexion.Start();

            //// Getting equipements
            GetEquipements();
        }

        public void Connexion()
        {
            //// Launching multicast listener on 224.0.0.50:9898
            PackageHost.WriteInfo("Starting to listen multicast on 224.0.0.50:9898");
            UdpClient client = new UdpClient();
            IPAddress multicastaddress = IPAddress.Parse("224.0.0.50");
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 9898);
            client.ExclusiveAddressUse = false;
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(localEp);
            client.JoinMulticastGroup(multicastaddress);
            PackageHost.WriteInfo("Listening start...");

            //// While receive response
            while (true)
            {

                //// We convert byte to string
                byte[] b = client.Receive(ref localEp);
                string response = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);

                //// We deserialize to Response class
                Response data = JsonConvert.DeserializeObject<Response>(response);

                //// If command equal heartbeat
                if (data.cmd == "heartbeat")
                {

                    //// We print response if heartbeat logs is true
                    if (PackageHost.TryGetSettingValue<bool>("HeartbeatLog", out HeartBeatLog))
                    {
                        if (HeartBeatLog)
                        {
                            PackageHost.WriteInfo("{0}", response);
                        }
                    }
                    else
                    {
                        PackageHost.WriteError("Impossible de récupérer le setting 'HeartbeatLog' en boolean");
                    }

                    //// If heartbeat model is gateway
                    if(data.model == "gateway")
                    {

                        //// We save the token
                        Gateway.Token = data.token;

                        //// We deserialize data part
                        gateway_heartbeat gateway_data = JsonConvert.DeserializeObject<gateway_heartbeat>(data.data);

                        //// If we didn't set gateway's sid
                        if (Gateway.Sid == null)
                        {
                            if (PackageHost.TryGetSettingValue<string>("GatewayIP", out GatewayIP))
                            {
                                //// If data ip equal to gateway ip
                                if (gateway_data.IP == GatewayIP)
                                {
                                    Gateway.Sid = data.sid;
                                }                               
                            }
                            else
                            {
                                PackageHost.WriteError("Impossible de récupérer le setting 'GatewayIP' en string");
                            }
                        }

                        PackageHost.PushStateObject<gateway>("Gateway", Gateway);
                       
                    }  
                }

                //// If command equal report
                else if (data.cmd == "report")
                {

                    //// We print response if report logs is true
                    if (PackageHost.TryGetSettingValue<bool>("ReportLog", out ReportLog))
                    {
                        if (ReportLog)
                        {
                            PackageHost.WriteInfo("{0}", response);
                        }
                    }
                    else
                    {
                        PackageHost.WriteError("Impossible de récupérer le setting 'ReportLog' en boolean");
                    }

                    //// If model equal gateway
                    if ((data.model == "gateway") && (data.sid == Gateway.Sid))
                    {

                        //// We deserialize report data as gateway class
                        gateway_report datagate = JsonConvert.DeserializeObject<gateway_report>(data.data);
                        Gateway.Report = datagate;
                        
                        //// We push the gateway SO
                        PackageHost.PushStateObject<gateway>("Gateway", Gateway);
                    }
                    else
                    {

                        //// If equipement sid is in the equipements list
                        if (equipements.Contains(data.sid))
                        {
                            SaveEquipement(data.sid);
                        }
                        
                    }

                }
                Thread.Yield();
            }
        }
        
        public void GetEquipements()
        {

            if (PackageHost.TryGetSettingValue<string>("GatewayIP", out GatewayIP))
            {
                //// Get equipements list
                EquipementsList list = EquipementsList.FindEquipementsList(GatewayIP);

                //// Get data part
                equipements = EquipementsList.GetListData(list.data);

                //// Foreach equipement in data
                foreach (string equipement in equipements)
                {
                    SaveEquipement(equipement);
                }
            }
            else
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'GatewayIP' en string");
            }
        }

        public void SaveEquipement(string mac)
        {
            //// Try to get setting based on equipement SID
            if (!PackageHost.TryGetSettingValue<string>(mac, out name))
            {
                name = mac;
            }

            //// Try to get gateway IP adress
            if (PackageHost.TryGetSettingValue<string>("GatewayIP", out GatewayIP))
            {

                //// Get response from read function
                Response read = Response.ReadEquipement(GatewayIP, mac);

                //// Create instance of model class
                dynamic model  = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("XiaomiSmartHome.Equipement." + read.model);
                var modelType = model.GetType();

                //// Create report instance of model class 
                dynamic model_report = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("XiaomiSmartHome.Equipement." + read.model + "_report");
                var model_reportType = model_report.GetType();

                //// Deserialize data part of the report
                var data = JsonConvert.DeserializeObject(Convert.ToString(read.data), model_reportType);

                //// Add informations to Report class
                model.BatteryLevel = System.Convert.ToInt32(System.Math.Floor((data.Voltage - 2800) * 0.33));
                model.Sid = mac;
                model.Report = data;

                //// Pushing the SO
                PackageHost.PushStateObject<dynamic>(name, model);
            }
            else
            {
                PackageHost.WriteError("Impossible de récupérer le setting 'GatewayIP' en string");
            }

        }

    }
}

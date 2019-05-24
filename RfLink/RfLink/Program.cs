using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constellation.Package;
using RFLinkNet;

namespace RfLink
{
    public class Program : PackageBase
    {
        private new Dictionary<string, Tuple<string, int>> soCustomNames = new Dictionary<string, Tuple<string, int>>();
        RFLinkClient rfLinkClient = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Package start
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            // Get the custom names
            this.soCustomNames = PackageHost.GetSettingAsJsonObject<Dictionary<string, Tuple<string, int>>>("soCustomNames");
            PackageHost.SettingsUpdated += (s, e) =>
            {
                this.soCustomNames = PackageHost.GetSettingAsJsonObject<Dictionary<string, Tuple<string, int>>>("soCustomNames");
            };

            // Connect to rflink
            PackageHost.WriteInfo($"Connecting to RfLink device ... {PackageHost.GetSettingValue("PortName")}");
            this.RFLinkConnect(PackageHost.GetSettingValue("PortName"), new List<string>());

            // Assert RfLink is alive
            KeepAlive();
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            PackageHost.WriteInfo("Disconnecting to RfLink device ...");
            this.rfLinkClient.Close();
        }

        /// <summary>
        /// Connect to RFLink device using values 
        /// defined in from data
        /// </summary>
        private string RFLinkConnect(string port, List<string> stateFields)
        {
            rfLinkClient = new RFLinkClient(port);
            rfLinkClient.EventLogOut += ReceivedStdOut;
            rfLinkClient.EventRFOut += ReceivedRFOut;

            try
            {
                if (rfLinkClient.Connect())
                {
                    return String.Format("Connected with RFLink Version {0}, Rev {1}, build {2}", rfLinkClient.Settings.Version, rfLinkClient.Settings.Rev, rfLinkClient.Settings.Build);
                }
                else
                {
                    return "Connection Failed";
                }
            }
            catch (Exception ex)
            {
                return String.Format("Connection Failed with exception {0}", ex.Message);
            }
        }

        /// <summary>
        /// Send ping commande to rflink
        /// </summary>
        private void KeepAlive()
        {
            Task.Factory.StartNew(async () =>
            {
                while (PackageHost.IsRunning)
                {
                    if ((DateTime.Now - rfLinkClient.LastPong).Minutes > 10)
                    {
                        PackageHost.WriteWarn("Pas de PONG du RfLink");
                        rfLinkClient.Close();
                        this.RFLinkConnect(PackageHost.GetSettingValue("PortName"), new List<string>());
                    }

                    SendData("10;PING;", 1);
                    await Task.Delay(TimeSpan.FromMinutes(5));
                }
            });
        }

        /// <summary>
        /// Log of rflink
        /// </summary>
        void ReceivedStdOut(object sender, EventArgs e)
        {
            if (PackageHost.GetSettingValue<bool>("Log"))
            {
                PackageHost.WriteInfo("RfLink : {0}", (e as RFEventArgs).Data);
            }
        }

        /// <summary>
        /// Data received from rflink
        /// </summary>
        void ReceivedRFOut(object sender, EventArgs e)
        {
            RFData rf = e as RFData;

            if (PackageHost.GetSettingValue<bool>("Log"))
            {
                PackageHost.WriteInfo("RfLink data receive : {0}", rf.ToString());
            }

            // Push state object
            if (rf.Fields.ContainsKey("ID"))
            {
                var param = this.GetCustomSoParams(rf.Fields["ID"]);
                PackageHost.PushStateObject(param.Item1, rf.Fields, lifetime: param.Item2);
            }
        }

        /// <summary>
        /// Send data to RFFlink
        /// </summary>
        [MessageCallback]
        private void SendData(string message, int retry = 1)
        {
            try
            {
                rfLinkClient.SendRawData(message, retry);

                RFData rf = ProtocolParser.ProcessData(message);
                if (rf.Fields.ContainsKey("ID"))
                {
                    var param = this.GetCustomSoParams(rf.Fields["ID"]);
                    PackageHost.PushStateObject(param.Item1, rf.Fields, lifetime: param.Item2);
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError(ex.Message);
            }
        }

        /// <summary>
        /// Gets custom name of the state object by its id
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <returns>The so name</returns>
        private Tuple<string, int> GetCustomSoParams(string deviceId)
        {
            return (this.soCustomNames != null && this.soCustomNames.ContainsKey(deviceId)) ? this.soCustomNames[deviceId] : new Tuple<string, int>(deviceId, 3600);
        }
    }
}

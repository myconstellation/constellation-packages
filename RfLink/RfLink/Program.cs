using System;
using System.Collections.Generic;
using Constellation.Package;
using RFLinkNet;

namespace RfLink
{
    public class Program : PackageBase
    {
        private Dictionary<string, string> soCustomNames = new Dictionary<string, string>();
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
            this.soCustomNames = PackageHost.GetSettingAsJsonObject<Dictionary<string, string>>("soCustomNames");
            PackageHost.SettingsUpdated += (s, e) =>
            {
                this.soCustomNames = PackageHost.GetSettingAsJsonObject<Dictionary<string, string>>("soCustomNames");
            };

            PackageHost.WriteInfo($"Connecting to RfLink device ... {PackageHost.GetSettingValue("PortName")}");
            this.RFLinkConnect(PackageHost.GetSettingValue("PortName"), new List<string>());
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
                string output = rf.ToString();
                PackageHost.WriteInfo("RfLink data receive : {0}", output);
            }

            // Push state object
            PackageHost.PushStateObject(this.GetCustomSoName(rf.Fields["ID"]), rf.Fields);
        }

        /// <summary>
        /// Send data to RFFlink
        /// </summary>
        [MessageCallback]
        private void SendData(string message, int retry = 1)
        {
            try
            {
                rfLinkClient?.SendRawData(message, retry);
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
        private string GetCustomSoName(string deviceId)
        {
            return (this.soCustomNames != null && this.soCustomNames.ContainsKey(deviceId)) ? this.soCustomNames[deviceId] : deviceId;
        }
    }
}

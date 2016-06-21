using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using FreeboxTV.Remote.Enumerations;
using FreeboxTV.Remote.Interfaces;
using FreeboxTV.Remote;

namespace FreeboxTV
{
    public class Program : PackageBase
    {
        /// <summary>
        ///  Main remote controller
        /// </summary>
        public IRemoteController<FreeCommandKey,FreeCommandType> RemoteController { get; set; }

        /// <summary>
        /// Remote key config value key
        /// </summary>
        public const string RemoteKeySettingKey = "RemoteKey";

        /// <summary>
        /// Free API Remote key
        /// </summary>
        public string RemoteKey { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            //// Getting configuration
            this.RemoteKey = PackageHost.GetSettingValue<string>(RemoteKeySettingKey);

            //// Remote controller
            this.RemoteController = new HttpFreeRemoteController(this.RemoteKey);
        }

        [MessageCallback(Description="Send a command to the Freebox Remote. See Enum description for more informations.")]
        public void SendCommand(FreeCommandKey command, FreeCommandType type)
        {
            //// TODO :  Manage Exceptions with SAGAs
            this.RemoteController.SendKey(command, type);
        }

    }
}

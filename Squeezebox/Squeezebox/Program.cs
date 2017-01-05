using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Squeezebox.Remote.Enumerations;
using Squeezebox.Remote.Interfaces;
using Squeezebox.Remote;

namespace FreeboxTV
{
    public class Program : PackageBase
    {
        /// <summary>
        ///  Main remote controller
        /// </summary>
        public IRemoteController<SqueezeboxCommand, string, string> RemoteController { get; set; }

        /// <summary>
        ///  Main server controller
        /// </summary>
        public IServerController<ServerCommand, string> ServerController { get; set; }

        /// <summary>
        /// Remote key config value key
        /// </summary>
        public const string RemoteKeySettingKey = "ServerUrl";

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
            this.RemoteController = new HttpSqueezeboxController(this.RemoteKey);

            //// Remote controller
            this.ServerController = new HttpServerController(this.RemoteKey);

        }

        /// [MessageCallback(Description = "Send a command to the Logitech Media Server. See Enum description for more informations.")]
        /// public void SendCommand(SqueezeboxCommand command)
        /// {
        //// TODO :  Manage Exceptions with SAGAs
        ///this.RemoteController.SendKey(command);
        ///}

        /// <summary>
        /// Send a command to a Squeezebox.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="value">The command value if necessary.</param>
        /// <param name="squeezebox">The Squeezebox name.</param>
        [MessageCallback]
        public void SendToSqueezebox(SqueezeboxCommand command, string value, string squeezebox)
        {
            this.RemoteController.SendKey(command, value, squeezebox);
        }

        /// <summary>
        /// Send a command to the Logitech Media Server.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="value">The command value if necessary.</param>
        [MessageCallback]
        public void SendToServer(ServerCommand command, string value)
        {
            this.ServerController.SendKey(command, value);
        }

    }
}

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

namespace Squeezebox
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
        public IServerController<ServerCommand> ServerController { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            
            //// Remote controller
            this.RemoteController = new HttpSqueezeboxController();

            //// Remote controller
            this.ServerController = new HttpServerController();

        }

        /// <summary>
        /// Send a command to a Squeezebox.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="value">The command value if necessary.</param>
        /// <param name="squeezebox">Squeezebox(s) name(s) comma separed or empty to target all.</param>
        [MessageCallback]
        public void SendToSqueezebox(SqueezeboxCommand command, string squeezebox = "", string value = "")
        {
            this.RemoteController.SendKey(command, squeezebox, value);
        }

        /// <summary>
        /// Send a command to the Logitech Media Server.
        /// </summary>
        /// <param name="command">The command.</param>
        [MessageCallback]
        public void SendToServer(ServerCommand command)
        {
            this.ServerController.SendKey(command);
        }

    }
}

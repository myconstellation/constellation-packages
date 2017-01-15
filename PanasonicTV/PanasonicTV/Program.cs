using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using PanasonicTV.Remote.Enumerations;
using PanasonicTV.Remote.Interfaces;
using PanasonicTV.Remote;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PanasonicTV
{
    public class Program : PackageBase
    {

        private HttpWebRequest Request = null;

        /// <summary>
        ///  Main remote controller
        /// </summary>
        public IRemoteController<PanasonicCommandKey, string> RemoteController { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            //// Remote controller
            this.RemoteController = new HttpPanasonicRemoteController();

        }

        /// <summary>
        /// Send a command to the Panasonic TV.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="target">The command target.</param>
        [MessageCallback]
        public void SendCommand(PanasonicCommandKey command, string target)
        {
            this.RemoteController.SendKey(command, target);
        }
    }
}

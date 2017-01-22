using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Jeedom.Remote.Enumerations;
using Jeedom.Remote.Interfaces;
using Jeedom.Remote;

namespace Jeedom
{
    public class Program : PackageBase
    {
        /// <summary>
        ///  Main remote controller
        /// </summary>
        public IController<int, Command, string> Controller { get; set; }

        /// <summary>
        ///  Main server controller
        /// </summary>
        public ISceneController<int, SceneCommand> SceneController { get; set; }

        /// <summary>
        /// Remote key config value key
        /// </summary>
        public const string JeedomUrl = "ServerUrl";

        /// <summary>
        /// Remote key config value key
        /// </summary>
        public const string JeedomApiKey = "ApiKey";

        /// <summary>
        /// Free API Remote key
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Free API Remote key
        /// </summary>
        public string ApiKey { get; set; }

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            //// Getting configuration
            this.Url = PackageHost.GetSettingValue<string>(JeedomUrl);

            //// Getting configuration
            this.ApiKey = PackageHost.GetSettingValue<string>(JeedomApiKey);

            //// Remote controller
            this.Controller = new HttpController(this.Url, this.ApiKey);

            //// Remote controller
            this.SceneController = new HttpSceneController(this.Url, this.ApiKey);

        }

        /// <summary>
        /// Send a command to an ID.
        /// </summary>
        /// <param name="id">The command ID.</param>
        /// <param name="command">The command type.</param>
        /// <param name="value">The command value if necessary.</param>+
        /// <param name="value2">The command value if necessary.</param>
        [MessageCallback]
        public void SendCommand(int id, Command command, string value = null, string value2 = null)
        {
            this.Controller.SendKey(id, command, value, value2);
        }

        /// <summary>
        /// Control a scene by ID.
        /// </summary>
        /// <param name="scene_id">The scene ID.</param>
        /// <param name="scene_command">The scene command.</param>
        [MessageCallback]
        public void SceneControl(int scene_id, SceneCommand scene_command)
        {
            this.SceneController.SendKey(scene_id, scene_command);
        }

    }
}

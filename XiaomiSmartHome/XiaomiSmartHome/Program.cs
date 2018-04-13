using Constellation.Package;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using XiaomiSmartHome.Equipement;

namespace XiaomiSmartHome
{
    /// <summary>
    /// Base class
    /// </summary>
    public class Program : PackageBase
    {
        /// <summary>
        /// Gateway listener
        /// </summary>
        public MulticastUdpClient udpClientWrapper;

        /// <summary>
        /// Manager for xiaomi equipments
        /// </summary>
        EquipementManager equipementManager;

        /// <summary>
        /// Equipement controller
        /// </summary>
        EquipementController equipementController;

        /// <summary>
        /// Call at start
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Main function
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            Thread connexion = new Thread(new ThreadStart(this.StartListener));
            connexion.Start();
        }

        /// <summary>
        /// Listening multicast to get reports and heartbeat
        /// </summary>
        private void StartListener()
        {
            // Create MulticastUdpClient
            udpClientWrapper = new MulticastUdpClient(
                IPAddress.Parse(PackageHost.GetSettingValue<string>(Constants.GATEWAY_MULTICAST_IP)),
                PackageHost.GetSettingValue<int>(Constants.GATEWAY_MULTICAST_PORT),
                IPAddress.Parse(PackageHost.GetSettingValue<string>(Constants.GATEWAY_IP)));

            // Instanciate equipement manager and controller
            equipementManager = new EquipementManager(udpClientWrapper);
            equipementController = new EquipementController(equipementManager);

            // And get all equipements
            equipementManager.InitEquipements();

            // Listen for udp messages
            udpClientWrapper.UdpMessageReceived += OnUdpMessageReceived;
        }

        /// <summary>
        /// Event handler which will be invoked when UDP message is received
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">data</param>
        private void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            equipementManager.ProcessUdpDiagram(ASCIIEncoding.ASCII.GetString(e.Buffer));
        }

        #region MESSAGE CALLBACK

        /// <summary>
        /// Turn the gateway light on
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="brightness">Brightness optional</param>
        [MessageCallback]
        void TurnGatewayLightOn(int r, int g, int b, int? brightness = null)
        {
            equipementController.TurnGatewayLightOn(r, g, b, brightness);
        }

        /// <summary>
        /// Turn the gateway light off
        /// </summary>
        [MessageCallback]
        void TurnGatewayLightOff()
        {
            equipementController.TurnGatewayLightOff();

        }

        /// <summary>
        /// Start playing sound on gateway
        /// </summary>
        /// <param name="mid">Sound identifier</param>
        /// <param name="vol">Volume</param>
        [MessageCallback]
        void PlayGatewaySound(int mid, int vol)
        {
            equipementController.PlayGatewaySound(mid, vol);
        }

        /// <summary>
        /// Stop playing sound on gateway
        /// </summary>
        [MessageCallback]
        void StopGatewaySound()
        {
            equipementController.StopGatewaySound();
        }

        /// <summary>
        /// Turn plug on
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        [MessageCallback]
        void TurnPlugOn(string sid)
        {
            equipementController.TurnPlugOn(sid);
        }

        /// <summary>
        /// Turn plug off
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        [MessageCallback]
        void TurnPlugOff(string sid)
        {
            equipementController.TurnPlugOff(sid);
        }

        /// <summary>
        /// Simulate click on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        [MessageCallback]
        void Click(string sid)
        {
            equipementController.Click(sid);
        }

        /// <summary>
        /// Simulate DoubleClick on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        [MessageCallback]
        void DoubleClick(string sid)
        {
            equipementController.DoubleClick(sid);
        }

        /// <summary>
        /// Simulate LongClickPress on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        [MessageCallback]
        void LongClickPress(string sid)
        {
            equipementController.LongClickPress(sid);
        }

        /// <summary>
        /// Simulate LongClickRelease on switch
        /// </summary>
        /// <param name="sid">Equipement SID</param>
        [MessageCallback]
        void LongClickRelease(string sid)
        {
            equipementController.LongClickRelease(sid);
        }

        #endregion
    }
}

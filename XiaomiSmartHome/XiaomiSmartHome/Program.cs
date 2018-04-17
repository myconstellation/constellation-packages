using Constellation.Package;
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

            //Gateway gateway = new Gateway
            //{
            //    Model = Enums.EquipmentType.Gateway
            //};
            //equipementManager.lEquipements.Add(gateway);
            //// get list
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""get_id_list_ack"",""sid"":""7811dcdf0ae6"",""token"":""4cUUY0X95fhfmGU7"",""data"":""[\""158d0001bbfe49\"",\""158d00023218ed\"",\""158d00020e8344\"",\""158d0002371a0c\""]""}");
            //// read
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""read_ack"",""model"":""plug"",""sid"":""158d0001bbfe49"",""short_id"":40845,""data"":""{\""voltage\"":3600,\""status\"":\""on\"",\""inuse\"":\""1\"",\""power_consumed\"":\""17530\"",\""load_power\"":\""191.40\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""read_ack"",""model"":""switch"",""sid"":""158d0002371a0c"",""short_id"":18313,""data"":""{\""voltage\"":3072}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""read_ack"",""model"":""magnet"",""sid"":""158d00020e8344"",""short_id"":37400,""data"":""{\""voltage\"":3005,\""status\"":\""close\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""read_ack"",""model"":""motion"",""sid"":""158d00023218ed"",""short_id"":40870,""data"":""{\""voltage\"":3015}""}");
            //// HeartBeat
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""heartbeat"", ""model"":""plug"", ""sid"":""158d0001bbfe49"", ""short_id"":40845,""data"":""{\""voltage\"":3600,\""status\"":\""on\"",\""inuse\"":\""1\"",\""power_consumed\"":\""20482\"",\""load_power\"":\""0.97\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""heartbeat"",""model"":""gateway"",""sid"":""7811dcdf0ae6"",""short_id"":""0"",""token"":""InXucBigQWh5KFet"",""data"":""{\""ip\"":\""192.168.1.26\""}""}");
            //// Report
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""gateway"",""sid"":""7811dcdf0ae6"",""short_id"":0,""data"":""{\""rgb\"":1677727487,\""illumination\"":1292}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""gateway"",""sid"":""7811dcdf0ae6"",""short_id"":0,""data"":""{\""rgb\"":0,\""illumination\"":1292}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""plug"",""sid"":""158d0001bbfe49"",""short_id"":40845,""data"":""{\""status\"":\""off\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""plug"",""sid"":""158d0001bbfe49"",""short_id"":40845,""data"":""{\""status\"":\""on\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""heartbeat"", ""model"":""plug"", ""sid"":""158d0001bbfe49"", ""short_id"":40845,""data"":""{\""voltage\"":3600,\""status\"":\""on\"",\""inuse\"":\""1\"",\""power_consumed\"":\""20482\"",\""load_power\"":\""0.97\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""switch"",""sid"":""158d0002371a0c"",""short_id"":18313,""data"":""{\""status\"":\""double_click\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""magnet"",""sid"":""158d00020e8344"",""short_id"":37400,""data"":""{\""status\"":\""open\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""magnet"",""sid"":""158d00020e8344"",""short_id"":37400,""data"":""{\""status\"":\""close\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""magnet"",""sid"":""158d00020e8344"",""short_id"":37400,""data"":""{\""no_close\"":\""60\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""magnet"",""sid"":""158d00020e8344"",""short_id"":37400,""data"":""{\""status\"":\""close\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""motion"",""sid"":""158d00023218ed"",""short_id"":40870,""data"":""{\""status\"":\""motion\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""motion"",""sid"":""158d00023218ed"",""short_id"":40870,""data"":""{\""no_motion\"":\""180\""}""}");
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""report"",""model"":""motion"",""sid"":""158d00023218ed"",""short_id"":40870,""data"":""{\""status\"":\""motion\""}""}");
            //// Write
            ////equipementController.TurnPlugOn("158d0001bbfe49");
            //// Write ack
            //equipementManager.ProcessUdpDiagram(@"{""cmd"":""write_ack"",""model"":""gateway"",""sid"":""7811dcdf0ae6"",""short_id"":0,""data"":""{\""rgb\"":1677727487,\""illumination\"":1292,\""proto_version\"":\""1.0.9\""}""}");
            
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
            //equipementController.TurnPlugOff("158d0001bbfe49");
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

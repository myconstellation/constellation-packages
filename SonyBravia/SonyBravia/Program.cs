namespace SonyBravia
{
    using BraviaIRCCControl.Extensions;
    using Constellation.Package;

    /// <summary>
    /// Package to control Sony Bravia Devices
    /// </summary>
    public class Program : PackageBase
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// OnStart 
        /// </summary>
        public override void OnStart()
        {
            string hostname = PackageHost.GetSettingValue<string>("Hostname");
            int port = PackageHost.GetSettingValue<int>("Port");
            string pinCode = PackageHost.GetSettingValue<string>("PinCode");

            IRCCCodesExtension.InitializeController(hostname, port, pinCode);
        }

        /// <summary>
        /// Sends an IRCC code to the Bravia device
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [MessageCallback]
        public bool SendIRCCCode(BraviaIRCCControl.IRCCCodes code)
        {
            return code.Send().Result;
        }
    }
}
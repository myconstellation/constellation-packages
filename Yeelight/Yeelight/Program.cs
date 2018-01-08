using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YeelightAPI;
using YeelightAPI.Models;

namespace Yeelight
{
    /// <summary>
    /// Yeelight Package
    /// </summary>
    public class Program : PackageBase
    {
        private static Dictionary<string, DeviceManager> _devices = null;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            _devices = new Dictionary<string, DeviceManager>();

            foreach (Bulb bulb in PackageHost.GetSettingAsJsonObject<IEnumerable<Bulb>>("Bulbs"))
            {
                Dictionary<string, object> bulbProperties = new Dictionary<string, object>();
                DeviceManager dm = new DeviceManager();
                dm.Connect(bulb.Hostname, bulb.Port);

                dm.NotificationReceived += async (object sender, NotificationReceivedEventArgs e) =>
                {
                    try
                    {
                        PackageHost.WriteInfo($"Receiving informations from notification : {Newtonsoft.Json.JsonConvert.SerializeObject(e.Result.Params)}");
                        foreach (string key in e.Result.Params.Keys)
                        {
                            if (bulbProperties.ContainsKey(key))
                            {
                                bulbProperties[key] = e.Result.Params[key];
                            }
                            else
                            {
                                bulbProperties.Add(key, e.Result.Params[key]);
                            }
                        }

                        PackageHost.PushStateObject(bulb.Name, bulbProperties);
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError($"error while retrieving informations from notfication : {ex.Message}");
                    }
                };

                //get all properties of bulb
                bulbProperties = dm.GetAllProps();
                PackageHost.PushStateObject(bulb.Name, bulbProperties);

                _devices.Add(bulb.Name, dm);
            }
        }


        public override void OnPreShutdown()
        {
            base.OnPreShutdown();

            foreach (var device in _devices)
            {
                device.Value.Disconnect();
            }
        }

        /// <summary>
        /// Toggle a bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <returns></returns>
        [MessageCallback]
        public CommandResult Toggle(string name)
        {
            DeviceManager manager = _devices[name];

            CommandResult result = manager.Toggle();

            return result;
        }

        /// <summary>
        /// Change the power state of a bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <param name="state">state : true is on, false is off</param>
        /// <returns></returns>
        [MessageCallback]
        public CommandResult SetPower(string name, bool state = true)
        {
            DeviceManager manager = _devices[name];

            CommandResult result = manager.SetPower(state);

            return result;
        }

        /// <summary>
        /// Change the brightness of the bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <param name="brightness">brightness intensity. From 1 to 100</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public CommandResult SetBrightness(string name, int brightness, int? smooth = null)
        {
            DeviceManager manager = _devices[name];

            CommandResult result = manager.SetBrightness(brightness, smooth);

            return result;
        }

        /// <summary>
        /// Change the color temperature of the bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <param name="temperature">temperature. From 1700 to 6500</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public CommandResult SetColorTemperature(string name, int temperature, int? smooth = null)
        {
            DeviceManager manager = _devices[name];

            CommandResult result = manager.SetColorTemperature(temperature, smooth);

            return result;
        }

        /// <summary>
        /// Change the bulb's RGB color
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <param name="red">red level : from 0 to 255</param>
        /// <param name="green">green level : from 0 to 255</param>
        /// <param name="blue">blue level : from 0 to 255</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public CommandResult SetRGBColor(string name, int red, int green, int blue, int? smooth = null)
        {
            DeviceManager manager = _devices[name];

            CommandResult result = manager.SetRGBColor(red, green, blue, smooth);

            return result;
        }

        /// <summary>
        /// Get all properties from a bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <returns></returns>
        [MessageCallback]
        public Dictionary<string, object> GetAllProps(string name)
        {
            DeviceManager manager = _devices[name];

            Dictionary<string, object> result = manager.GetAllProps();

            return result;
        }

        /// <summary>
        /// Get a single property from a bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <param name="propertyKey">Name of the property</param>
        /// <returns></returns>
        [MessageCallback]
        public object GetProp(string name, string propertyKey)
        {
            DeviceManager manager = _devices[name];

            object result = manager.GetProp(propertyKey);

            return result;
        }

        /// <summary>
        /// Get multiple properties from a bulb
        /// </summary>
        /// <param name="name">Bulb's name</param>
        /// <param name="props">list of properties names</param>
        /// <returns></returns>
        [MessageCallback]
        public Dictionary<string, object> GetProps(string name, List<object> props)
        {
            DeviceManager manager = _devices[name];

            Dictionary<string, object> result = manager.GetProps(props);

            return result;
        }

    }
}

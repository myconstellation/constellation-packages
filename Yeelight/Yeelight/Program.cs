using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Yeelight.Configs;
using YeelightAPI;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Scene;

namespace Yeelight
{
    /// <summary>
    /// Yeelight Package
    /// </summary>
    public class Program : PackageBase
    {
        #region Private Attributes

        private static Dictionary<string, object> _all = new Dictionary<string, object>();

        #endregion Private Attributes

        #region Constellation Init

        /// <summary>
        /// Package Pre-Shutdown
        /// </summary>
        public override void OnPreShutdown()
        {
            foreach (KeyValuePair<string, object> device in _all)
            {
                if (device.Value is IDisposable disposableDevice)
                {
                    try
                    {
                        disposableDevice.Dispose();
                    }
                    catch(Exception ex)
                    {
                        PackageHost.WriteError("Unable to dispose device '{dc.Name}' ({device.Hostname}:{device.Port})");
                    }
                }
            }

            base.OnPreShutdown();
        }

        /// <summary>
        /// Package start
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            //parallel connection to the devices
            PackageHost.GetSettingAsJsonObject<IEnumerable<DeviceConfig>>("Devices").AsParallel().ForAll(dc =>
            {
                //create new device and connect 
                Device device = new Device(dc.Hostname, dc.Port) { Name = dc.Name };
                try
                {
                    device.Connect().Wait();
                    if (device.IsConnected)
                    {
                        device.OnError -= ErrorEvent;
                        device.OnError += ErrorEvent;

                        device.SetName(dc.Name).Wait();
                        PackageHost.WriteInfo($"device '{dc.Name}' ({device.Hostname}:{device.Port}) connected");
                    }
                }
                catch(Exception ex)
                {
                    PackageHost.WriteWarn($"Cannot connect device '{device.Name}' ({device.Hostname}:{device.Port}), message : {ex.Message}");
                }

                _all.Add(dc.Name, device);
            });

            //creation of groups
            foreach (DeviceGroupConfig gc in PackageHost.GetSettingAsJsonObject<IEnumerable<DeviceGroupConfig>>("Groups"))
            {
                DeviceGroup group = new DeviceGroup();
                foreach (Device device in gc.Devices.Select(x => _all.SingleOrDefault(d => d.Key == x).Value))
                {
                    group.Add(device);
                }

                _all.Add(gc.Name, group);
            }

            UpdateDevicesStateObjects();
        }

        private static void UpdateDevicesStateObjects()
        {
            Task.Factory.StartNew(async () =>
            {
                Dictionary<string, bool> states = new Dictionary<string, bool>();
                while (PackageHost.IsRunning)
                {
                    _all.AsParallel().ForAll(kvp =>
                    {
                        if (kvp.Value is Device device)
                        {
                            try
                            {
                                if (!device.IsConnected)
                                {
                                    device.Connect().Wait();
                                }

                                if (device.IsConnected)
                                {
                                    var props = device.GetAllProps().Result;
                                    PackageHost.PushStateObject(kvp.Key, props, lifetime: 60);
                                    states[kvp.Key] = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (!states.ContainsKey(kvp.Key) || states[kvp.Key] == true)
                                { // log only if previous attempt was a success
                                    PackageHost.WriteError($"An error occurred while fetching device '{device.Name}' ({device.Hostname}:{device.Port}) state, message : {ex.Message} | {ex.InnerException?.Message}");
                                }

                                states[kvp.Key] = false;
                            }
                        }
                    });
                    await Task.Delay(5000);
                }
            });
        }

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        #endregion Constellation Init

        #region Yeelight Utils

        /// <summary>
        /// Error event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ErrorEvent(object sender, UnhandledExceptionEventArgs e)
        {
            PackageHost.WriteError($"An unknown error occured : '{e.ExceptionObject}'");
        }

        /// <summary>
        /// Get a controller device or group by its name
        /// </summary>
        /// <param name="deviceOrGroupName"></param>
        /// <returns></returns>
        private static IDeviceController GetControllerDevice(string deviceOrGroupName)
        {
            return _all[deviceOrGroupName] as IDeviceController;
        }

        /// <summary>
        /// Get a reader device or group by its name
        /// </summary>
        /// <param name="deviceOrGroupName"></param>
        /// <returns></returns>
        private static IDeviceReader GetReaderDevice(string deviceOrGroupName)
        {
            return _all[deviceOrGroupName] as IDeviceReader;
        }

        #endregion

        #region MessageCallbacks

        /// <summary>
        /// Adjusts the Brightness
        /// </summary>
        /// <param name="deviceOrGroupName"></param>
        /// <param name="percent">range between -100 and 100</param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [MessageCallback]
        public bool AdjustBrightness(string deviceOrGroupName, int percent, int? duration)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.AdjustBright(percent, duration).Result;
        }

        /// <summary>
        /// Adjusts the Color
        /// </summary>
        /// <param name="deviceOrGroupName"></param>
        /// <param name="percent">range between -100 and 100</param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [MessageCallback]
        public bool AdjustColor(string deviceOrGroupName, int percent, int? duration)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.AdjustColor(percent, duration).Result;
        }

        /// <summary>
        /// Adjusts the Color temperature
        /// </summary>
        /// <param name="deviceOrGroupName"></param>
        /// <param name="percent">range between -100 and 100</param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [MessageCallback]
        public bool AdjustColorTemperature(string deviceOrGroupName, int percent, int? duration)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.AdjustColorTemperature(percent, duration).Result;
        }

        /// <summary>
        /// Discover devices through LAN
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        public List<Device> Discover()
        {
            return DeviceLocator.Discover().Result;
        }

        /// <summary>
        /// Get all properties from a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public Dictionary<PROPERTIES, object> GetAllProps(string deviceOrGroupName)
        {
            IDeviceReader device = GetReaderDevice(deviceOrGroupName);

            return device.GetAllProps().Result;
        }

        /// <summary>
        /// Get a single property from a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="property">Name of the property</param>
        /// <returns></returns>
        [MessageCallback]
        public object GetProp(string deviceOrGroupName, PROPERTIES property)
        {
            IDeviceReader device = GetReaderDevice(deviceOrGroupName);

            return device.GetProp(property).Result;
        }

        /// <summary>
        /// Get multiple properties from a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="properties">list of properties names</param>
        /// <returns></returns>
        [MessageCallback]
        public Dictionary<PROPERTIES, object> GetProps(string deviceOrGroupName, PROPERTIES properties)
        {
            IDeviceReader device = GetReaderDevice(deviceOrGroupName);

            return device.GetProps(properties).Result;
        }

        /// <summary>
        /// Adjusts the device settings
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="adjustAction">Adjust action to perform</param>
        /// <param name="adjustProperty">the property to adjust</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetAdjust(string deviceOrGroupName, AdjustAction adjustAction, AdjustProperty adjustProperty)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetAdjust(adjustAction, adjustProperty).Result;
        }

        /// <summary>
        /// Change the brightness of the Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="brightness">brightness intensity. From 1 to 100</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetBrightness(string deviceOrGroupName, int brightness, int? smooth = null)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetBrightness(brightness, smooth).Result;
        }

        /// <summary>
        /// Change the color temperature of the Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="temperature">temperature. From 1700 to 6500</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetColorTemperature(string deviceOrGroupName, int temperature, int? smooth = null)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetColorTemperature(temperature, smooth).Result;
        }

        /// <summary>
        /// Save the current state as the default
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetDefault(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetDefault().Result;
        }

        /// <summary>
        /// Change the Device's HSV color
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="hue">red level : from 0 to 255</param>
        /// <param name="sat">green level : from 0 to 255</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetHSVColor(string deviceOrGroupName, int hue, int sat, int? smooth = null)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetHSVColor(hue, sat, smooth).Result;
        }

        /// <summary>
        /// Change the power state of a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="state">state : true is on, false is off</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetPower(string deviceOrGroupName, bool state = true)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetPower(state).Result;
        }

        /// <summary>
        /// Change the Device's RGB color
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="red">red level : from 0 to 255</param>
        /// <param name="green">green level : from 0 to 255</param>
        /// <param name="blue">blue level : from 0 to 255</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public bool SetRGBColor(string deviceOrGroupName, int red, int green, int blue, int? smooth = null)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.SetRGBColor(red, green, blue, smooth).Result;
        }

        /// <summary>
        /// Toggle a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool Toggle(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.Toggle().Result;
        }

        /// <summary>
        /// Change the power state of a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        [MessageCallback]
        public bool TurnOff(string deviceOrGroupName, int? smooth = null)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.TurnOff(smooth).Result;
        }

        /// <summary>
        /// Change the power state of a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [MessageCallback]
        public bool TurnOn(string deviceOrGroupName, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            return device.TurnOn(smooth, mode).Result;
        }

        /// <summary>
        /// Turn to sunrise from 0 to 100% on 15 minutes and keep bulb on
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool StartSunRise(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            ColorFlow flow = new ColorFlow(1, ColorFlowEndAction.Keep)
            {
                new ColorFlowTemperatureExpression(2500, 30, 30000),
                new ColorFlowSleepExpression(1000),
                new ColorFlowTemperatureExpression(3000, 40, 30000),
                new ColorFlowSleepExpression(1000),
                new ColorFlowTemperatureExpression(3500, 50, 30000),
                new ColorFlowSleepExpression(1000),
                new ColorFlowTemperatureExpression(3700, 60, 30000),
                new ColorFlowSleepExpression(1000),
                new ColorFlowTemperatureExpression(4000, 10, 30000),
            };

            return device.SetScene(Scene.FromColorFlow(flow)).Result;
        }

        /// <summary>
        /// Turn to sunset from 100% to 0 on 15 minutes and turn bulb off
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool StartSunSet(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            ColorFlow flow = new ColorFlow(3, ColorFlowEndAction.TurnOff)
            {
                new ColorFlowTemperatureExpression(2700, 10, 50),
                new ColorFlowTemperatureExpression(1700, 5, 180000),
                new ColorFlowRGBExpression(255, 0, 4, 1, 420000)
            };

            return device.SetScene(Scene.FromColorFlow(flow)).Result;
        }

        /// <summary>
        /// Start a 4-7-8 Breath indicator on selected device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool StartFourSevenEight(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            ColorFlow flow = new ColorFlow(30, ColorFlowEndAction.TurnOff)
            {
                new ColorFlowRGBExpression(255, 65, 17, 50, 300),
                new ColorFlowSleepExpression(4000),
                new ColorFlowRGBExpression(255, 230, 0, 50, 300),
                new ColorFlowSleepExpression(7000),
                new ColorFlowRGBExpression(255, 120, 0, 50, 300),
                new ColorFlowSleepExpression(8000)
            };

            return device.SetScene(Scene.FromColorFlow(flow)).Result;
        }

        /// <summary>
        /// Turn to romantic mode
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool StartRomantic(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);

            ColorFlow flow = new ColorFlow(0, ColorFlowEndAction.TurnOff)
            {
                new ColorFlowRGBExpression(255, 0, 204, 40, 2000),
                new ColorFlowSleepExpression(3000),
                new ColorFlowRGBExpression(255, 0, 255, 30, 2000),
                new ColorFlowSleepExpression(3000)
            };

            return device.SetScene(Scene.FromColorFlow(flow)).Result;
        }

        /// <summary>
        /// Turn device on with high luminosity
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool TurnOnDay(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);
            return device.SetScene(Scene.FromColorTemperature(2700, 100)).Result;
        }

        /// <summary>
        /// Turn device on with low luminosity
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public bool TurnOnNight(string deviceOrGroupName)
        {
            IDeviceController device = GetControllerDevice(deviceOrGroupName);
            return device.SetScene(Scene.FromColorTemperature(2700, 25)).Result;
        }

        #endregion MessageCallbacks
    }
}
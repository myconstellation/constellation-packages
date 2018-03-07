using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yeelight.Configs;
using YeelightAPI;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;

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
                    disposableDevice.Dispose();
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

            foreach (DeviceConfig dc in PackageHost.GetSettingAsJsonObject<IEnumerable<DeviceConfig>>("Devices"))
            {
                Device device = new Device(dc.Hostname, dc.Port) { Name = dc.Name };
                if (device.Connect().Result)
                {
                    device.OnNotificationReceived += (object sender, NotificationReceivedEventArgs e) =>
                    {
                        PackageHost.PushStateObject(device.Name, device.Properties);
                        PackageHost.WriteDebug(e.Result);
                    };

                    device.OnError += (object sender, UnhandledExceptionEventArgs e) =>
                    {
                        PackageHost.WriteError(e.ExceptionObject);
                    };
                }

                _all.Add(dc.Name, device);
            }

            foreach (DeviceGroupConfig gc in PackageHost.GetSettingAsJsonObject<IEnumerable<DeviceGroupConfig>>("DeviceGroups"))
            {
                DeviceGroup group = new DeviceGroup();
                foreach (Device device in gc.Devices.Select(x => _all.SingleOrDefault(d => d.Key == x).Value))
                {
                    group.Add(device);
                }

                _all.Add(gc.Name, group);
            }
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

        #region MessageCallbacks

        /// <summary>
        /// Get all properties from a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<Dictionary<PROPERTIES, object>> GetAllProps(string deviceOrGroupName)
        {
            IDeviceReader device = _all[deviceOrGroupName] as IDeviceReader;

            return await device.GetAllProps();
        }

        /// <summary>
        /// Get a single property from a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="property">Name of the property</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<object> GetProp(string deviceOrGroupName, PROPERTIES property)
        {
            IDeviceReader device = _all[deviceOrGroupName] as IDeviceReader;

            return await device.GetProp(property);
        }

        /// <summary>
        /// Get multiple properties from a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="properties">list of properties names</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<Dictionary<PROPERTIES, object>> GetProps(string deviceOrGroupName, PROPERTIES properties)
        {
            IDeviceReader device = _all[deviceOrGroupName] as IDeviceReader;

            return await device.GetProps(properties);
        }

        /// <summary>
        /// Adjusts the device settings
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="adjustAction">Adjust action to perform</param>
        /// <param name="adjustProperty">the property to adjust</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> SetAdjust(string deviceOrGroupName, AdjustAction adjustAction, AdjustProperty adjustProperty)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetAdjust(adjustAction, adjustProperty);
        }

        /// <summary>
        /// Change the brightness of the Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="brightness">brightness intensity. From 1 to 100</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> SetBrightness(string deviceOrGroupName, int brightness, int? smooth = null)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetBrightness(brightness, smooth);
        }

        /// <summary>
        /// Change the color temperature of the Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="temperature">temperature. From 1700 to 6500</param>
        /// <param name="smooth">Duration of the effect in milliseconds. Min : 50</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> SetColorTemperature(string deviceOrGroupName, int temperature, int? smooth = null)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetColorTemperature(temperature, smooth);
        }

        /// <summary>
        /// Save the current state as the default
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> SetDefault(string deviceOrGroupName)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetDefault();
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
        public async Task<bool> SetHSVColor(string deviceOrGroupName, int hue, int sat, int? smooth = null)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetHSVColor(hue, sat, smooth);
        }

        /// <summary>
        /// Change the power state of a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="state">state : true is on, false is off</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> SetPower(string deviceOrGroupName, bool state = true)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetPower(state);
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
        public async Task<bool> SetRGBColor(string deviceOrGroupName, int red, int green, int blue, int? smooth = null)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.SetRGBColor(red, green, blue, smooth);
        }

        /// <summary>
        /// Toggle a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> Toggle(string deviceOrGroupName)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.Toggle();
        }

        /// <summary>
        /// Change the power state of a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> TurnOff(string deviceOrGroupName, int? smooth = null)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.TurnOff(smooth);
        }

        /// <summary>
        /// Change the power state of a Device
        /// </summary>
        /// <param name="deviceOrGroupName">Device's name</param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [MessageCallback]
        public async Task<bool> TurnOn(string deviceOrGroupName, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal)
        {
            IDeviceController device = _all[deviceOrGroupName] as IDeviceController;

            return await device.TurnOn(smooth, mode);
        }

        #endregion MessageCallbacks
    }
}
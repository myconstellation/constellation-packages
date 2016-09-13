/*
 *	 Vera Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2012-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
 *	
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *	
 *	 Unless required by applicable law or agreed to in writing,
 *	 software distributed under the License is distributed on an
 *	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *	 KIND, either express or implied. See the License for the
 *	 specific language governing permissions and limitations
 *	 under the License.
 */

namespace Vera
{
    using Constellation.Package;
    using System.Collections.Generic;
    using System.Linq;
    using VeraNet;

    class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        private VeraController vera = null;

        public override void OnPreShutdown()
        {
            if (vera != null)
            {
                PackageHost.WriteInfo("Disconnecting from the Vera");
                vera.Dispose();
            }
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Initializing Vera");
            vera = new VeraController(new VeraConnectionInfo(PackageHost.GetSettingValue<string>("VeraHost")));
            vera.ErrorOccurred += (s, e) => PackageHost.WriteError("ErrorOccurred: {0}", e.Exception.ToString());
            vera.DeviceUpdated += (s, e) => this.PushDevice(e.Device);
            vera.SceneUpdated += (s, e) =>
            {
                if (e.Scene != null && !string.IsNullOrEmpty(e.Scene.Name))
                {
                    PackageHost.WriteInfo("Updating scene '{0}' ({1})", e.Scene.Name, e.Scene.Id);
                    PackageHost.PushStateObject(e.Scene.Name, new Scene
                    {
                        Id = e.Scene.Id,
                        IsActive = e.Scene.IsActive,
                        LastUpdate = e.Scene.LastUpdate,
                        Name = e.Scene.Name,
                        Room = e.Scene.Room != null ? e.Scene.Room.Name : string.Empty,
                        State = e.Scene.State
                    });
                }
            };
            vera.DataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(vera.SerialNumber))
                {
                    PackageHost.PushStateObject("Vera_" + vera.SerialNumber, new VeraDevice
                    {
                        DataVersion = e.DataVersion,
                        LoadTime = e.LoadTime,
                        State = vera.CurrentState.ToString(),
                        Comment = vera.CurrentComment,
                        Model = vera.Model,
                        SerialNumber = vera.SerialNumber,
                        Version = vera.Version
                    });
                }
            };

            PackageHost.WriteInfo("Perform a full request ...");
            vera.WaitForFullRequest();

            PackageHost.WriteInfo("Starting listener ...");
            vera.StartListener();

            PackageHost.WriteInfo("Vera lite ready !");
        }

        /// <summary>
        /// Runs the scene.
        /// </summary>
        /// <param name="idScene">The identifier scene.</param>
        /// <returns></returns>
        [MessageCallback]
        private bool RunScene(int idScene)
        {
            VeraNet.Objects.Scene scene = vera.Scenes.FirstOrDefault(s => s.Id == idScene);
            if (scene != null)
            {
                PackageHost.WriteInfo("Running scene {0} ({1})", scene.Name, scene.Id);
                return scene.RunScene();
            }
            else
            {
                PackageHost.WriteError("The scene #'{0}' not found !",  idScene);
                return false;
            }
        }

        /// <summary>
        /// Sets the state of the switch.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [MessageCallback]
        private bool SetSwitchState(DeviceRequest request)
        {
            VeraNet.Objects.Devices.Switch switchDevice = vera.Devices.FirstOrDefault(s => s.Id == request.DeviceID) as VeraNet.Objects.Devices.Switch;
            if (switchDevice != null)
            {
                PackageHost.WriteInfo("Switch '{0}' {1} ({2})", request.State ? "On" : "Off", switchDevice.Name, switchDevice.Id);
                if (request.State)
                {
                    return switchDevice.SwitchOn();
                }
                else
                {
                    return switchDevice.SwitchOff();
                }
            }
            else
            {
                PackageHost.WriteError("The device #'{0}' not found !", request.DeviceID);
                return false;
            }
        }

        /// <summary>
        /// Sets the state and dimmable level.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [MessageCallback]
        private bool SetDimmableLevel(DeviceRequest request)
        {
            VeraNet.Objects.Devices.DimmableLight dimmableDevice = vera.Devices.FirstOrDefault(s => s.Id == request.DeviceID) as VeraNet.Objects.Devices.DimmableLight;
            if (dimmableDevice != null)
            {
                PackageHost.WriteInfo("Set level {0}% to {1} ({2})", request.Level, dimmableDevice.Name, dimmableDevice.Id);
                return dimmableDevice.SetLevel(request.Level);
            }
            else
            {
                PackageHost.WriteError("The device #'{0}' not found !", request.DeviceID);
                return false;
            }
        }

        private void PushDevice(VeraNet.Objects.Device device)
        {
            if (device != null && !string.IsNullOrEmpty(device.Name))
            {
                PackageHost.WriteInfo("Updating device '{0}' ({1})", device.Name, device.Id);

                var metadata = new Dictionary<string, object>()
                {
                    { "DeviceId", device.Id },
                    { "Category", device.Category != null ? device.Category.Name : string.Empty },
                    { "Room", device.Room != null ? device.Room.Name : string.Empty },
                    { "Type", device.GetType().Name }
                };

                if (device is VeraNet.Objects.Devices.TemperatureSensor)
                {
                    PackageHost.PushStateObject(device.Name, new TemperatureSensor()
                    {
                        Temperature = ((VeraNet.Objects.Devices.TemperatureSensor)device).Temperature,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }
                else if (device is VeraNet.Objects.Devices.HumiditySensor)
                {
                    PackageHost.PushStateObject(device.Name, new HumiditySensor()
                    {
                        Humidity = ((VeraNet.Objects.Devices.HumiditySensor)device).Humidity,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }
                else if (device is VeraNet.Objects.Devices.WindowCovering)
                {
                    PackageHost.PushStateObject(device.Name, new WindowCovering()
                    {
                        Status = ((VeraNet.Objects.Devices.WindowCovering)device).Status,
                        Level = ((VeraNet.Objects.Devices.WindowCovering)device).Level,
                        Watts = ((VeraNet.Objects.Devices.WindowCovering)device).Watts,
                        KWh = ((VeraNet.Objects.Devices.WindowCovering)device).KWh,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }

                else if (device is VeraNet.Objects.Devices.DimmableLight)
                {
                    PackageHost.PushStateObject(device.Name, new DimmableLight()
                    {
                        Status = ((VeraNet.Objects.Devices.DimmableLight)device).Status,
                        Level = ((VeraNet.Objects.Devices.DimmableLight)device).Level,
                        Watts = ((VeraNet.Objects.Devices.DimmableLight)device).Watts,
                        KWh = ((VeraNet.Objects.Devices.DimmableLight)device).KWh,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }
                else if (device is VeraNet.Objects.Devices.Switch)
                {
                    PackageHost.PushStateObject(device.Name, new Switch()
                    {
                        Status = ((VeraNet.Objects.Devices.Switch)device).Status,
                        Watts = ((VeraNet.Objects.Devices.Switch)device).Watts,
                        KWh = ((VeraNet.Objects.Devices.Switch)device).KWh,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }
                else if (device is VeraNet.Objects.Devices.PowerMeter)
                {
                    PackageHost.PushStateObject(device.Name, new PowerMeter()
                    {
                        Watts = ((VeraNet.Objects.Devices.PowerMeter)device).Watts,
                        KWh = ((VeraNet.Objects.Devices.PowerMeter)device).KWh,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }
                else if (device is VeraNet.Objects.Devices.SecuritySensor)
                {
                    PackageHost.PushStateObject(device.Name, new SecuritySensor()
                    {
                        Tripped = ((VeraNet.Objects.Devices.SecuritySensor)device).Tripped,
                        Armed = ((VeraNet.Objects.Devices.SecuritySensor)device).Armed,
                        ArmedTripped = ((VeraNet.Objects.Devices.SecuritySensor)device).ArmedTripped,
                        LastTrip = ((VeraNet.Objects.Devices.SecuritySensor)device).LastTrip,
                        Id = device.Id,
                        BatteryLevel = device.BatteryLevel,
                        Room = device.Room != null ? device.Room.Name : string.Empty,
                        Category = device.Category.Name,
                        LastUpdate = device.LastUpdate,
                        State = device.State
                    }, metadatas: metadata);
                }
            }
        }

        /// <summary>
        /// Z-Wave Device request
        /// </summary>
        public class DeviceRequest
        {
            /// <summary>
            /// The device identifier.
            /// </summary>
            public int DeviceID { get; set; }

            /// <summary>
            /// The state of the device (True = On, False = Off)
            /// </summary>
            public bool State { get; set; }

            /// <summary>
            /// The level (0 to 100%) for dimmable device 
            /// </summary>
            public int Level { get; set; }
        }
    }
}

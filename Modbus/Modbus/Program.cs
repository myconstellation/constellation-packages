/*
 *	 Modbus Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Modbus
{
    using Constellation;
    using Constellation.Package;
    using EasyModbus;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Modbus Constellation connector
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private const int AVG_REGISTER_READ_PER_SECOND = 10;
        private static readonly object syncLock = new object();

        private ModbusClient modbusClient = null;
        private List<DeviceState> deviceStates = new List<DeviceState>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        /// <exception cref="System.Exception">Invalid configuration : you have to set the TcpAddress/TcpPort settings for Modbus TCP or RtuSerialPort's setting for Modbus RTU !</exception>
        public override void OnStart()
        {
            // Create the Modbus client
            if (PackageHost.ContainsSetting("TcpAddress") && PackageHost.ContainsSetting("TcpPort"))
            {
                // Modbus TCP
                this.modbusClient = new ModbusClient(PackageHost.GetSettingValue("TcpAddress"), PackageHost.GetSettingValue<int>("TcpPort"));
            }
            else if (PackageHost.ContainsSetting("RtuSerialPort"))
            {
                // Modbus RTU
                this.modbusClient = new ModbusClient(PackageHost.GetSettingValue("RtuSerialPort"))
                {
                    Baudrate = PackageHost.GetSettingValue<int>("RtuBaudRate")
                };
            }
            else
            {
                // Invalid configuration !
                throw new Exception("Invalid configuration : you have to set the TcpAddress/TcpPort settings for Modbus TCP or RtuSerialPort's setting for Modbus RTU !");
            }

            // Load and register device definitions
            List<ModbusDeviceDefinition> modbusDevicesDefintions = LoadModbusDevicesDefinitions();

            // Load Modbus devices to request
            this.LoadDevices(modbusDevicesDefintions);

            // Client connection
            this.modbusClient.ConnectedChanged += ModbusClient_ConnectedChanged;
            this.modbusClient.Connect();

            // Start the backgroup task
            if (this.deviceStates.Count > 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    int estimatedTotalReadingTime = (int)Math.Round((decimal)deviceStates.Sum(d => d.Definition.Properties.Count) / (decimal)AVG_REGISTER_READ_PER_SECOND);
                    while (PackageHost.IsRunning)
                    {
                        // For each device to poll
                        foreach (DeviceState device in this.deviceStates)
                        {
                            // Need to request ?
                            if (DateTime.Now.Subtract(device.LastUpdate).TotalSeconds >= device.Device.RequestInterval)
                            {
                                PackageHost.WriteLog(PackageHost.GetSettingValue<bool>("Verbose") ? LogLevel.Info : LogLevel.Debug, $"Requesting {device.Device}...");
                                try
                                {
                                    // Set the Slave ID
                                    modbusClient.UnitIdentifier = device.Device.SlaveID;
                                    // Create the result object
                                    dynamic result = new ExpandoObject();
                                    var resultDict = result as IDictionary<string, object>;
                                    // For each property for the device
                                    foreach (var property in device.Definition.Properties)
                                    {
                                        int attempt = 0;
                                        // Attempt the read the register
                                        while (++attempt <= this.modbusClient.NumberOfRetries && !resultDict.ContainsKey(property.Name))
                                        {
                                            PackageHost.WriteLog(PackageHost.GetSettingValue<bool>("Verbose") ? LogLevel.Info : LogLevel.Debug, $"Reading {property.Name} ({property.Address})");
                                            try
                                            {
                                                // Read register from the modbus client
                                                int[] datas = null;
                                                lock (syncLock)
                                                {
                                                    datas = property.RegisterType == ModbusDeviceDefinition.RegisterType.Holding ?
                                                        modbusClient.ReadHoldingRegisters(Convert.ToInt32(property.Address, 16), property.Length) :
                                                        modbusClient.ReadInputRegisters(Convert.ToInt32(property.Address, 16), property.Length);
                                                }
                                                // Boolean type
                                                if (property.Type == ModbusDeviceDefinition.PropertyType.Boolean)
                                                {
                                                    resultDict[property.Name] = datas[0] == 1;
                                                }
                                                else
                                                {
                                                    // Raw value
                                                    int rawValue = datas.Length == 1 ? datas[0] : ((datas[0] << 8) + datas[1]);
                                                    // Apply the ratio and set the result
                                                    resultDict[property.Name] = (float)(rawValue * property.Ratio);
                                                    // Trucate Integer property
                                                    if (property.Type == ModbusDeviceDefinition.PropertyType.Int)
                                                    {
                                                        resultDict[property.Name] = (int)(float)resultDict[property.Name];
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                if (++attempt > this.modbusClient.NumberOfRetries)
                                                {
                                                    // Unable to read the register, log and set the default value
                                                    resultDict[property.Name] = Activator.CreateInstance(property.GetCLRType());
                                                    PackageHost.WriteLog(PackageHost.GetSettingValue<bool>("Verbose") ? LogLevel.Error : LogLevel.Debug, $"Unable to read the property {property.Name} ({property.Address}h) of {device.Device} : {ex.Message}");
                                                }
                                            }
                                        }
                                    }
                                    // Push the StateObject 
                                    PackageHost.PushStateObject(device.Device.Name, result,
                                            type: $"Modbus.{device.Definition.Name}",
                                            lifetime: (estimatedTotalReadingTime + device.Device.RequestInterval) * 2,
                                            metadatas: new Dictionary<string, object>
                                            {
                                                ["SlaveID"] = device.Device.SlaveID
                                            });
                                    // Done !
                                    device.LastUpdate = DateTime.Now;
                                }
                                catch (Exception ex)
                                {
                                    PackageHost.WriteError($"Error to request {device.Device} : {ex.Message}");
                                }
                            }
                            await Task.Delay(500);
                        }
                        await Task.Delay(10);
                    }
                });
            }
        }

        /// <summary>
        /// Called when the package is shutdown (disconnected from Constellation)
        /// </summary>
        public override void OnShutdown()
        {
            if (this.modbusClient.Connected)
            {
                this.modbusClient.Disconnect();
            }
        }
        private void ModbusClient_ConnectedChanged(object sender)
        {
            if (this.modbusClient.IPAddress != null)
            {
                PackageHost.WriteInfo($"{(this.modbusClient.Connected ? "Connected to" : "Disconnected from")} {this.modbusClient.IPAddress}:{this.modbusClient.Port}");
            }
            else
            {
                PackageHost.WriteInfo($"{(this.modbusClient.Connected ? "Connected to" : "Disconnected from")} {this.modbusClient.SerialPort}");
            }
        }

        #region MessageCallbacks

        /// <summary>
        /// This command is requesting the ON/OFF status of discrete coils (FC=01).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the first coil to read.</param>
        /// <param name="quantity">The total number of coils requested.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool[] ReadCoils(int startingAddress, int quantity)
        {
            lock (syncLock)
            {
                return this.modbusClient.ReadCoils(startingAddress, quantity);
            }
        }

        /// <summary>
        /// This command is requesting the ON/OFF status of discrete inputs (FC=02).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the first input to read.</param>
        /// <param name="quantity">The total number of coils requested.</param>
        /// <returns></returns>
        [MessageCallback]
        public bool[] ReadDiscreteInputs(int startingAddress, int quantity)
        {
            lock (syncLock)
            {
                return this.modbusClient.ReadDiscreteInputs(startingAddress, quantity);
            }
        }

        /// <summary>
        /// This command is requesting the content of analog output holding registers (FC=03).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the first register requested.</param>
        /// <param name="quantity">The total number of registers requested.</param>
        /// <returns></returns>
        [MessageCallback]
        public int[] ReadHoldingRegisters(int startingAddress, int quantity)
        {
            lock (syncLock)
            {
                return this.modbusClient.ReadHoldingRegisters(startingAddress, quantity);
            }
        }

        /// <summary>
        /// This command is requesting the content of analog input register (FC=04).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the first register requested.</param>
        /// <param name="quantity">The total number of registers requested.</param>
        /// <returns></returns>
        [MessageCallback]
        public int[] ReadInputRegisters(int startingAddress, int quantity)
        {
            lock (syncLock)
            {
                return this.modbusClient.ReadInputRegisters(startingAddress, quantity);
            }
        }

        /// <summary>
        /// This command is writing the contents of discrete coil (FC=05).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the coil.</param>
        /// <param name="value">The status to write.</param>
        [MessageCallback]
        public void WriteSingleCoil(int startingAddress, bool value)
        {
            lock (syncLock)
            {
                this.modbusClient.WriteSingleCoil(startingAddress, value);
            }
        }

        /// <summary>
        /// This command is writing the contents of analog output holding register (FC=06).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the register.</param>
        /// <param name="value">The value to write.</param>
        [MessageCallback]
        public void WriteSingleRegister(int startingAddress, int value)
        {
            lock (syncLock)
            {
                this.modbusClient.WriteSingleRegister(startingAddress, value);
            }
        }

        /// <summary>
        /// This command is reading and writing the contents of analog output holding registers (FC=03 + FC=16).
        /// </summary>
        /// <param name="startingAddressRead">The Data Address of the first register requested.</param>
        /// <param name="quantityRead">TThe total number of registers requested.</param>
        /// <param name="startingAddressWrite">The Data Address of the first register to write.</param>
        /// <param name="values">The values to write.</param>
        /// <returns></returns>
        [MessageCallback]
        public int[] ReadWriteMultipleRegisters(int startingAddressRead, int quantityRead, int startingAddressWrite, int[] values)
        {
            lock (syncLock)
            {
                return this.modbusClient.ReadWriteMultipleRegisters(startingAddressRead, quantityRead, startingAddressWrite, values);
            }
        }

        /// <summary>
        /// This command is writing the contents of a series of discrete coils (FC=15). 
        /// </summary>
        /// <param name="startingAddress">The Data Address of the first coil.</param>
        /// <param name="values">The values to write.</param>
        [MessageCallback]
        public void WriteMultipleCoils(int startingAddress, bool[] values)
        {
            lock (syncLock)
            {
                this.modbusClient.WriteMultipleCoils(startingAddress, values);
            }
        }

        /// <summary>
        /// This command is writing the contents of analog output holding registers (FC=16).
        /// </summary>
        /// <param name="startingAddress">The Data Address of the first register.</param>
        /// <param name="values">The values to write.</param>
        [MessageCallback]
        public void WriteMultipleRegisters(int startingAddress, int[] values)
        {
            lock (syncLock)
            {
                this.modbusClient.WriteMultipleRegisters(startingAddress, values);
            }
        }

        #endregion

        private void LoadDevices(List<ModbusDeviceDefinition> modbusDevicesDefintions)
        {
            if (PackageHost.ContainsSetting("Devices"))
            {
                // Read the JSON setting
                var devices = PackageHost.GetSettingAsJsonObject<List<ModbusDevice>>("Devices");
                // For each device
                foreach (ModbusDevice device in devices)
                {
                    // Get the definition from the ModbusDeviceDefinitions's setting
                    var definition = modbusDevicesDefintions.FirstOrDefault(d => d.Name == device.DeviceType);
                    // If the definition exists
                    if (definition != null)
                    {
                        // Registering the device
                        PackageHost.WriteInfo($"Registering {device} ({definition.Name} with {definition.Properties.Count} properties). Interval: {device.RequestInterval} second(s)");
                        this.deviceStates.Add(new DeviceState { Device = device, Definition = definition });
                    }
                    else
                    {
                        // Otherwise ignore this device !
                        PackageHost.WriteWarn($"The device {device} will be ignore : unknown device type ({device.DeviceType})");
                    }
                }
            }
        }

        private static List<ModbusDeviceDefinition> LoadModbusDevicesDefinitions()
        {
            if (PackageHost.ContainsSetting("ModbusDeviceDefinitions"))
            {
                // Read the JSON setting
                var modbusDevicesDefintions = PackageHost.GetSettingAsJsonObject<List<ModbusDeviceDefinition>>("ModbusDeviceDefinitions");
                // For each definition
                foreach (ModbusDeviceDefinition definition in modbusDevicesDefintions)
                {
                    // Create the StateObject type descriptor
                    var soType = new PackageDescriptor.TypeDescriptor
                    {
                        TypeName = definition.Name,
                        TypeFullname = $"Modbus.{definition.Name}",
                        Description = definition.Description,
                        Properties = definition.Properties.Select(p => new PackageDescriptor.MemberDescriptor
                        {
                            Name = p.Name,
                            Description = p.Description,
                            Type = PackageDescriptor.MemberDescriptor.MemberType.Property,
                            TypeName = p.GetCLRType().FullName
                        }).ToList()
                    };
                    // And add it to the Package's descriptor
                    PackageHost.WriteInfo($"Registering type {soType.TypeFullname} ...");
                    PackageHost.PackageDescriptor.StateObjectTypes.Add(soType);
                }
                // Declare the Package's descriptor on Constellation
                PackageHost.DeclarePackageDescriptor();
                // Return the loaded definitions
                return modbusDevicesDefintions;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Represent a Modbus device to request
        /// </summary>
        private class DeviceState
        {
            /// <summary>
            /// Gets or sets the Modbus device.
            /// </summary>
            /// <value>
            /// The device.
            /// </value>
            public ModbusDevice Device { get; set; }

            /// <summary>
            /// Gets or sets the device's definition.
            /// </summary>
            /// <value>
            /// The device's definition.
            /// </value>
            public ModbusDeviceDefinition Definition { get; set; }

            /// <summary>
            /// Gets or sets the last update.
            /// </summary>
            /// <value>
            /// The last update.
            /// </value>
            public DateTime LastUpdate { get; set; }
        }
    }
}

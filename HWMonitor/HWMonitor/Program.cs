/*
 *	 HWMonitor Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace HWMonitor
{
    using Constellation.Package;
    using OpenHardwareMonitor.Hardware;
    using OpenHardwareMonitor.WMI;
    using System.Linq;
    using System.Management;
    using System.Threading;

    /// <summary>
    /// HWMonitor Package
    /// </summary>
    public class Program : PackageBase
    {
        private const int DEFAULT_LIFETIME = 60;

        private Computer computer = null;
        private bool pushHardwareListOnChange = false;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            this.computer = new Computer();
            var updateVisitor = new UpdateVisitor();
            var wmiProvider = new WmiProvider(computer);

            this.computer.HardwareAdded += new HardwareEventHandler(computer_HardwareAdded);
            this.computer.HardwareRemoved += new HardwareEventHandler(computer_HardwareRemoved);

            this.computer.Open();

            this.computer.MainboardEnabled = true;
            this.computer.CPUEnabled = true;
            this.computer.FanControllerEnabled = true;
            this.computer.GPUEnabled = true;
            this.computer.HDDEnabled = true;
            this.computer.NICEnabled = true;
            this.computer.RAMEnabled = true;

            this.pushHardwareListOnChange = true;
            this.PushHardwaresList();

            while (PackageHost.IsRunning)
            {
                Thread.Sleep(PackageHost.GetSettingValue<int>("Interval"));

                this.computer.Accept(updateVisitor);
                if (wmiProvider != null)
                {
                    wmiProvider.Update();
                }

                foreach (var hardware in computer.Hardware)
                {
                    this.PushHardware(hardware);
                }
            }
        }

        /// <summary>
        /// Called when the package is shutdown (disconnected from Constellation)
        /// </summary>
        public override void OnShutdown()
        {
            this.computer.Close();
        }

        private void PushHardwaresList()
        {
            PackageHost.WriteInfo("Hardware changed");
            PackageHost.PushStateObject("Hardware", this.computer.Hardware.Select(hw => new HardwareDevice () { Name = hw.Name, Identifier = hw.Identifier.ToString(), Type = hw.HardwareType }).ToList(), lifetime: DEFAULT_LIFETIME);
        }

        private void PushHardware(IHardware hardware, int level = 0)
        {
            if (hardware.SubHardware.Length > 0)
            {
                foreach (var shw in hardware.SubHardware)
                {
                    this.PushHardware(shw, level + 1);
                }
            }

            foreach (var sensorType in hardware.Sensors.Select(g => g.SensorType).Distinct())
            {
                foreach (var item in hardware.Sensors)
                {
                    if (item.SensorType == sensorType)
                    {
                        PackageHost.PushStateObject(item.Identifier.ToString(),
                            new SensorValue()
                            {
                                Name = item.Name,
                                Value = item.Value,
                                Type = item.SensorType
                            },
                            lifetime: DEFAULT_LIFETIME,
                            metadatas: new System.Collections.Generic.Dictionary<string, object>() { ["Hardware"] = item.Hardware.Name });
                    }
                }
            }
        }

        private void computer_HardwareRemoved(IHardware hardware)
        {
            if (this.pushHardwareListOnChange)
            {
                this.PushHardwaresList();
            }
        }

        private void computer_HardwareAdded(IHardware hardware)
        {
            PackageHost.WriteInfo("Adding " + hardware.Name);
            if (hardware.HardwareType == HardwareType.HDD)
            {
                var hddName = "PHYSICALDRIVE" + hardware.Identifier.ToString().Replace("/hdd/", "");
                var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_DiskDrive WHERE Name LIKE \"%{ hddName }\"");
                ManagementObject mo = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                if (mo != null)
                {
                    PackageHost.PushStateObject(hardware.Identifier.ToString(), new DiskDrive()
                    {
                        Name = this.GetWmiValue(mo, "Name"),
                        Hardware = hardware.ToString(),
                        SerialNumber = this.GetWmiValue(mo, "SerialNumber"),
                        Model = this.GetWmiValue(mo, "Model"),
                        Status = this.GetWmiValue(mo, "Status"),
                        MediaType = this.GetWmiValue(mo, "MediaType"),
                        Manufacturer = this.GetWmiValue(mo, "Manufacturer"),
                        Caption = this.GetWmiValue(mo, "Caption")
                    }, lifetime: DEFAULT_LIFETIME);
                }
            }
            if (this.pushHardwareListOnChange)
            {
                this.PushHardwaresList();
            }
        }

        private string GetWmiValue(ManagementObject mo, string name)
        {
            foreach (PropertyData prop in mo.Properties)
            {
                if (prop.Name == name)
                {
                    return prop.Value != null ? prop.Value.ToString() : string.Empty;
                }
            }
            return string.Empty;
        }
    }
}

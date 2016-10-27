/*
 *	 RelayBoard Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2013 - Anthony Marshall 	
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

namespace RelayBoard
{
    using Constellation.Package;
    using FTD2XX_NET;
    using System;

    /// <summary>
    /// SainSmart USB Relay Board control
    /// </summary>
    [StateObject]
    public class RelayBoard
    {
        private const int BAUD_RATE = 9600;
        private readonly byte[] startup = { 0x00 };
        
        private uint bytesToSend = 1;
        private UInt32 ftdiDeviceCount = 0;
        private FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        private FTDI ftdiDevice = new FTDI();

        private string serialNumber, description, portCOM, deviceType;
        private uint deviceId, driverVersion;

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        public string SerialNumber { get { return serialNumber; } }
        /// <summary>
        /// Gets the port COM.
        /// </summary>
        public string PortCOM { get { return portCOM; } }
        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get { return description; } }
        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public uint DeviceID { get { return deviceId; } }
        /// <summary>
        /// Gets the type of the device.
        /// </summary>
        public string DeviceType { get { return deviceType; } }
        /// <summary>
        /// Gets the driver version.
        /// </summary>
        public uint DriverVersion { get { return driverVersion; } }
    
        /// <summary>
        /// Find the FDTI chip, connect, set baud to 9600, set sync bit-bang mode
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            // Determine the number of FTDI devices connected to the machine
            this.ftStatus = ftdiDevice.GetNumberOfDevices(ref this.ftdiDeviceCount);
            // Check status
            if (this.ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                PackageHost.WriteInfo("Number of FTDI devices: {0}", this.ftdiDeviceCount);
            }
            else
            {
                PackageHost.WriteError("Failed to get number of devices (error {0})", this.ftStatus);
                return false;
            }
            // Check device count
            if (ftdiDeviceCount == 0)
            {
                PackageHost.WriteError("No relay board found, please try again");
                return false;
            }
            else
            {
                if (PackageHost.ContainsSetting("SerialNumber") && !string.IsNullOrEmpty(PackageHost.GetSettingValue("SerialNumber")))
                {
                    // Open device by serial number
                    this.ftStatus = this.ftdiDevice.OpenBySerialNumber(PackageHost.GetSettingValue("SerialNumber"));
                }
                else
                {
                    // Open the first device
                    do
                    {
                        this.ftStatus = this.ftdiDevice.OpenByIndex(deviceId++);
                    }
                    while (this.ftStatus != FTDI.FT_STATUS.FT_OK && deviceId < ftdiDeviceCount);
                }
                // Get serial number
                if (this.ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    this.ftStatus = this.ftdiDevice.GetSerialNumber(out serialNumber);
                    this.ftStatus = this.ftdiDevice.GetCOMPort(out portCOM);
                    this.ftStatus = this.ftdiDevice.GetDescription(out description);
                    this.ftStatus = this.ftdiDevice.GetDeviceID(ref deviceId);
                    this.ftStatus = this.ftdiDevice.GetDriverVersion(ref driverVersion);
                    FTDI.FT_DEVICE deviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    this.ftStatus = this.ftdiDevice.GetDeviceType(ref deviceType);
                    this.deviceType = deviceType.ToString();
                }
                // Check status
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    PackageHost.WriteInfo("Connected on device '{0}'", this.SerialNumber);
                }
                else
                {
                    PackageHost.WriteError("Unable to connect to the relay board");
                    return false;
                }

                // Set Baud rate to 9600
                this.ftStatus = this.ftdiDevice.SetBaudRate(BAUD_RATE);
                // Set FT245RL to synchronous bit-bang mode, used on sainsmart relay board
                this.ftdiDevice.SetBitMode((byte)Relay.All, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);

                // Switch off all the relays
                this.ftdiDevice.Write(startup, 1, ref bytesToSend);

                // Ok !
                return true;
            }
        }

        /// <summary>
        /// Activate/De-activate a specific relay
        /// </summary>
        /// <param name="relay">The relay.</param>
        /// <param name="state">If set to <c>true</c> switch on, otherwise, switch off.</param>
        public void RelaySwitch(Relay relay, bool state)
        {
            uint numBytes = 1;
            byte[] Out = { 0x00 };
            byte pins = 0x00;

            // Find which relays are ON/OFF
            ftdiDevice.GetPinStates(ref pins);

            // Permut
            Out[0] = state ?
                (byte)(pins | (int)relay) :
                (byte)(pins & ~((int)relay));

            // Set state
            ftdiDevice.Write(Out, 1, ref numBytes);
        }
    }
}

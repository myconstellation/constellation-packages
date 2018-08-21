/*
 *	 S-Sound - Multi-room audio system for Constellation
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2018 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace SSound.Core.Dlna
{
    using Constellation.Package;
    using OpenSource.UPnP;
    using OpenSource.UPnP.AV.CdsMetadata;
    using OpenSource.UPnP.AV.RENDERER.Device;
    using System;

    /// <summary>
    /// Represents a DLNA device
    /// </summary>
    public class Device
    {
        private UPnPDevice device = null;
        private Renderer currentRenderer = null;

        /// <summary>
        /// Starts the DLNA renderer.
        /// </summary>
        /// <param name="endpointName">The endpoint name.</param>
        public void StartRenderer(string endpointName)
        {
            // Create the device
            this.device = UPnPDevice.CreateRootDevice(900, 1, "");
            device.FriendlyName = "S-Sound " + endpointName;
            device.UniqueDeviceName = "SSound_" + endpointName.Replace(" ", "");
            device.StandardDeviceType = "MediaRenderer";
            device.HasPresentation = false;
            device.Manufacturer = "Sebastien.warin.fr";
            device.ManufacturerURL = "http://sebastien.warin.fr/";
            device.PresentationURL = "/";
            device.HasPresentation = true;
            device.ModelName = "AV Renderer";
            device.ModelDescription = "Media Renderer Device";
            device.ModelURL = new Uri("http://sebastien.warin.fr/");

            // Create the AV renderer
            var avRenderer = new AVRenderer(0, new ProtocolInfoString[]
            {
                new ProtocolInfoString("http-get:*:audio/mp3:*"),
                new ProtocolInfoString("http-get:*:audio/x-ms-wma:*"),
                new ProtocolInfoString("http-get:*:audio/wma:*")
            }, new AVRenderer.ConnectionHandler(NewConnectionSink));
            avRenderer.OnClosedConnection += new AVRenderer.ConnectionHandler(ClosedConnectionSink);

            avRenderer.Manager.RemoveAction_PrepareForConnection();
            avRenderer.Manager.RemoveAction_ConnectionComplete();

            // Set the UPnP Servoce state variables
            UPnPService upnpATService = avRenderer.AVT.GetUPnPService();
            upnpATService.GetStateVariableObject("CurrentPlayMode").AllowedStringValues = new String[3] { "NORMAL", "REPEAT_ALL", "INTRO" };

            UPnPService upnpControlService = avRenderer.Control.GetUPnPService();
            upnpControlService.GetStateVariableObject("A_ARG_TYPE_Channel").AllowedStringValues = new String[3] { "Master", "LF", "RF" };
            upnpControlService.GetStateVariableObject("RedVideoBlackLevel").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("GreenVideoBlackLevel").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("BlueVideoBlackLevel").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("RedVideoGain").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("GreenVideoGain").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("BlueVideoGain").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("Brightness").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("Contrast").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("Sharpness").SetRange((ushort)0, (ushort)100, (ushort)1);
            upnpControlService.GetStateVariableObject("Volume").SetRange((UInt16)0, (UInt16)100, (ushort)1);

            // Add the services
            device.AddService(avRenderer.Control);
            device.AddService(avRenderer.AVT);
            device.AddService(avRenderer.Manager);

            // Start the device
            device.StartDevice();
            PackageHost.PushStateObject<bool>("DlnaMediaRenderer", true);
            PackageHost.WriteInfo("DLNA MediaRenderer '{0}' is started", endpointName);
        }

        /// <summary>
        /// Stops the renderer.
        /// </summary>
        public void StopRenderer()
        {
            if (this.device != null)
            {
                this.device.StopDevice();
                PackageHost.PushStateObject<bool>("DlnaMediaRenderer", false);
                PackageHost.WriteInfo("DLNA MediaRenderer is stopped");
            }
        }

        private void ClosedConnectionSink(AVRenderer sender, AVConnection connection)
        {
            if(this.currentRenderer != null && this.currentRenderer.AVConnection == connection)
            {
                this.currentRenderer = null;
            }
        }

        private void NewConnectionSink(AVRenderer sender, AVConnection connection)
        {
            if(this.currentRenderer != null)
            {
                this.currentRenderer = null;
            }
            this.currentRenderer = new Renderer(connection);
        }
    }
}

/*
 *	 Wemo Package for Constellation
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

namespace Wemo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Provides a set of static methods for Wemo Device
    /// </summary>
    public static class WemoExtensions
    {
        /// <summary>
        /// Queries the state of the switch.
        /// </summary>
        /// <param name="device">The device.</param>
        public static void QuerySwitchState(this WemoSwitch device)
        {
            if (device.DeviceType == "urn:Belkin:device:insight:1")
            {
                device.QueryInsightState();
            }
            else
            {
                device.State = device.InvokeServiceAction("basicevent", "GetBinaryState").InnerText == "1";
            }
            device.Signal = int.Parse(device.InvokeServiceAction("basicevent", "GetSignalStrength").InnerText);
        }
        
        /// <summary>
        /// Queries the state of the insight device.
        /// </summary>
        /// <param name="device">The device.</param>
        public static void QueryInsightState(this WemoSwitch device)
        {
            string[] data = device.InvokeServiceAction("insight", "GetInsightParams").InnerText.Split('|');
            device.State = data[0] == "1";
            device.LastChange = double.Parse(data[1]).UnixTimeStampToDateTime();
            device.OnFor = TimeSpan.FromSeconds(double.Parse(data[2]));
            device.OnToday = TimeSpan.FromSeconds(double.Parse(data[3]));
            device.OnTotal = TimeSpan.FromSeconds(double.Parse(data[4]));
            device.TimePeriod = TimeSpan.FromSeconds(double.Parse(data[5]));
            device.Thresold = double.Parse(data[6]);
            device.CurrentMW = double.Parse(data[7], CultureInfo.InvariantCulture);
            device.TodayMW = double.Parse(data[8], CultureInfo.InvariantCulture);
            device.TotalMW = double.Parse(data[9], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the state of the switch.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public static void SetSwitchState(this WemoSwitch device, bool state)
        {
            device.InvokeServiceAction("basicevent", "SetBinaryState", new Dictionary<string, string>() { ["BinaryState"] = state ? "1" : "0" });
            device.State = state;
        }

        /// <summary>
        /// Invokes the service action.
        /// </summary>
        /// <param name="device">The Wemo device.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="arguments">The optional arguments.</param>
        /// <returns>XML document</returns>
        public static XmlDocument InvokeServiceAction(this WemoDevice device, string serviceName, string actionName, Dictionary<string, string> arguments = null)
        {
            HttpWebRequest req = WebRequest.Create($"{device.Location.Scheme}://{device.Location.Host}:{device.Location.Port}/upnp/control/{serviceName}1") as HttpWebRequest;
            // Set headers
            req.Headers.Add($"SOAPACTION:\"urn:Belkin:service:{serviceName}:1#{actionName}\"");
            req.ContentType = "text/xml; charset=\"utf-8\"";
            req.Method = "POST";
            req.Timeout = 2000;
            // Build request content
            string reqContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            reqContent += "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">";
            reqContent += "<s:Body>";
            reqContent += $"<u:{actionName} xmlns:u=\"urn:Belkin:service:{serviceName}:1\">";
            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    reqContent += $"<{arg.Key}>{arg.Value}</{arg.Key}>";
                }
            }
            reqContent += $"</u:{actionName}>";
            reqContent += "</s:Body>";
            reqContent += "</s:Envelope>";
            // Write the request coontent
            using (Stream requestStream = req.GetRequestStream())
            {
                requestStream.Write(Encoding.UTF8.GetBytes(reqContent), 0, Encoding.UTF8.GetByteCount(reqContent));
            }
            // Execute request 
            var xmlResponse = new XmlDocument();
            try
            {
                HttpWebResponse response = req.GetResponse() as HttpWebResponse;
                using (Stream rspStm = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(rspStm))
                    {
                        xmlResponse.LoadXml(reader.ReadToEnd());
                    }
                }
                Debug.WriteLine("Command '{0}' result = {1}", actionName, response.StatusCode.ToString());
            }
            catch (WebException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // Return the XML response
            return xmlResponse;
        }

        /// <summary>
        /// Changes the type of the device.
        /// </summary>
        /// <typeparam name="TWemoDevice">The type of the wemo device.</typeparam>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        public static TWemoDevice ChangeDeviceType<TWemoDevice>(this WemoDevice device) where TWemoDevice : WemoDevice, new()
        {
            return new TWemoDevice()
            {
                Location = device.Location,
                FriendlyName = device.FriendlyName,
                DeviceType = device.DeviceType,
                Manufacturer = device.Manufacturer,
                ModelDescription = device.ModelDescription,
                ModelName = device.ModelName,
                ModelNumber = device.ModelNumber,
                SerialNumber = device.SerialNumber,
                UDN = device.UDN,
                UPC = device.UPC,
                MacAddress = device.MacAddress,
                FirmwareVersion = device.FirmwareVersion
            };
        }

        /// <summary>
        /// Get date time from Unix timestamp.
        /// </summary>
        /// <param name="unixTimeStamp">The unix time stamp.</param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

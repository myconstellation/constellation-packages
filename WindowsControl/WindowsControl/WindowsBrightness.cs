/*
 *	 WindowsControl Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace WindowsControl
{
    using System;
    using System.Collections.Generic;
    using System.Management;

    internal static class WindowsBrightness
    {
        public static int GetBrightness()
        {
            byte curBrightness = 0;
            foreach (ManagementObject mObj in GetWmiManagementObjects("WmiMonitorBrightness"))
            {
                curBrightness = (byte)mObj.GetPropertyValue("CurrentBrightness");
                break;
            }
            return curBrightness;
        }

        public static byte[] GetBrightnessLevels()
        {
            byte[] brightnessLevels = new byte[0];
            try
            {
                foreach (ManagementObject mObj in GetWmiManagementObjects("WmiMonitorBrightness"))
                {
                    brightnessLevels = (byte[])mObj.GetPropertyValue("Level");
                    break;
                }
            }
            catch { }
            return brightnessLevels;
        }

        public static void SetBrightness(byte targetBrightness)
        {
            foreach (ManagementObject mObj in GetWmiManagementObjects("WmiMonitorBrightnessMethods"))
            {
                mObj.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness });
                break;
            }
        }

        private static IEnumerable<ManagementObject> GetWmiManagementObjects(string query)
        {
            ManagementScope scope = new ManagementScope("root\\WMI");
            SelectQuery selectQuery = new SelectQuery(query);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, selectQuery))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (ManagementObject mObj in objectCollection)
                    {
                        yield return mObj;
                    }
                }
            }
        }
    }
}
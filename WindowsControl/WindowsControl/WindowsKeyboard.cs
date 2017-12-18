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
    using System.Runtime.InteropServices;

    internal static class WindowsKeyboard
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public enum Keys
        {
            VolumeDown = 174,
            VolumeUp = 175,
            VolumeMute = 173,
            MediaNextTrack = 176,
            MediaPreviousTrack = 177,
            MediaPlayPause = 179,
            MediaStop = 178
        }

        public static void SendKey(Keys key)
        {
            keybd_event((byte)key, 0, 0, 0);
        }
    }
}
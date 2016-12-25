/*
 *	 WindowsControl Package for Constellation
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

namespace WindowsControl
{
    using Constellation.Package;
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// WindowsControl Package 
    /// </summary>
    public class Program : PackageBase
    {
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        private WindowsSession session = new WindowsSession();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.ConnectionStateChanged += (s, e) =>
            {
                if (PackageHost.IsConnected)
                {
                    PackageHost.PushStateObject("SessionLocked", this.session.IsLocked());
                }
            };

            this.session.StateChanged += (s, e) =>
            {
                if (e.Reason == SessionSwitchReason.SessionLock)
                {
                    PackageHost.PushStateObject("SessionLocked", true);
                }
                else if (e.Reason == SessionSwitchReason.SessionUnlock)
                {
                    PackageHost.PushStateObject("SessionLocked", false);
                }
            };
        }

        /// <summary>
        /// Called before shutdown the package (the package is still connected to Constellation).
        /// </summary>
        public override void OnPreShutdown()
        {
            PackageHost.PushStateObject("SessionLocked", true);
        }

        /// <summary>
        /// Called when the package is shutdown (disconnected from Constellation)
        /// </summary>
        public override void OnShutdown()
        {
            this.session.Dispose();
        }

        /// <summary>
        /// Logoff the current session.
        /// </summary>
        [MessageCallback]
        public void LogoffSession()
        {
            WindowsSession.ExitWindowsEx(0, 0);
        }

        /// <summary>
        /// Locks the work station.
        /// </summary>
        [MessageCallback]
        public void LockWorkStation()
        {
            WindowsSession.LockWorkStation();
        }

        /// <summary>
        /// Shutdowns the work station.
        /// </summary>
        [MessageCallback]
        public void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");
        }

        /// <summary>
        /// Reboots the work station.
        /// </summary>
        [MessageCallback]
        public void Reboot()
        {
            Process.Start("shutdown", "/r /t 0");
        }

        /// <summary>
        /// Sleeps the work station.
        /// </summary>
        [MessageCallback]
        public void Sleep()
        {
            SetSuspendState(false, true, true);
        }

        /// <summary>
        /// Hibernates the work station.
        /// </summary>
        [MessageCallback]
        public void Hibernate()
        {
            SetSuspendState(true, true, true);
        }
    }
}

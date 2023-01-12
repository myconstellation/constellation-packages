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
                PackageHost.WriteInfo("Session State changed to {0}", e.Reason.ToString());
                if (e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.RemoteDisconnect)
                {
                    PackageHost.PushStateObject("SessionLocked", true);
                }
                else if (e.Reason == SessionSwitchReason.SessionUnlock || e.Reason == SessionSwitchReason.RemoteConnect)
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
            PackageHost.PushStateObject("SessionLocked", true, lifetime: 1); // Set the StateObject expire the next second
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

        /// <summary>
        /// Mute/Unmute volume.
        /// </summary>
        [MessageCallback]
        public void Mute()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.VolumeMute);
        }

        /// <summary>
        /// Increase volume.
        /// </summary>
        [MessageCallback]
        public void VolumeUp()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.VolumeUp);
        }

        /// <summary>
        /// Decrease volume.
        /// </summary>
        [MessageCallback]
        public void VolumeDown()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.VolumeDown);
        }

        /// <summary>  
        /// Next track  
        /// </summary>  
        [MessageCallback]
        public void MediaNextTrack()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.MediaNextTrack);
        }

        /// <summary>  
        /// Previous track  
        /// </summary>  
        [MessageCallback]
        public void MediaPreviousTrack()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.MediaPreviousTrack);
        }

        /// <summary>  
        /// Toogle play pause of the current media  
        /// </summary>  
        [MessageCallback]
        public void MediaPlayPause()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.MediaPlayPause);
        }

        /// <summary>  
        /// Stop the curent media  
        /// </summary>  
        [MessageCallback]
        public void MediaStop()
        {
            WindowsKeyboard.SendKey(WindowsKeyboard.Keys.MediaStop);
        }

        /// <summary>
        /// Sets the brightness.
        /// </summary>
        /// <param name="targetBrightness">The target brightness.</param>
        [MessageCallback]
        public void SetBrightness(int targetBrightness)
        {
            if (targetBrightness < 0 || targetBrightness > 100)
            {
                PackageHost.WriteError("Invalid value !");
            }
            else
            {
                WindowsBrightness.SetBrightness((byte)targetBrightness);
            }
        }

        /// <summary>
        /// Increase Brightness.
        /// </summary>
        [MessageCallback]
        public void BrightnessUp()
        {
            int currentValue = WindowsBrightness.GetBrightness();
            byte[] levels = WindowsBrightness.GetBrightnessLevels();
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] > currentValue)
                {
                    WindowsBrightness.SetBrightness(levels[i]);
                    break;
                }
            }
        }

        /// <summary>
        /// Decrease Brightness.
        /// </summary>
        [MessageCallback]
        public void BrightnessDown()
        {
            int currentValue = WindowsBrightness.GetBrightness();
            byte[] levels = WindowsBrightness.GetBrightnessLevels();
            for (int i = levels.Length - 1; i > 0; i--)
            {
                if (currentValue > levels[i])
                {
                    WindowsBrightness.SetBrightness(levels[i]);
                    break;
                }
            }
        }

        /// <summary>  
        /// Executes a command.
        /// </summary>  
        [MessageCallback]
        public void Execute(string cmd)
        {
            ExecuteCommand(cmd);
        }

        static void ExecuteCommand(string command)
        {
            PackageHost.WriteInfo("Executing command : " + command);
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                PackageHost.WriteInfo("output>> " + e.Data);
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                PackageHost.WriteError("error>> " + e.Data);
            process.BeginErrorReadLine();

            process.WaitForExit();

            PackageHost.WriteError("ExitCode: {0}", process.ExitCode);
            process.Close();
        }
    }
}

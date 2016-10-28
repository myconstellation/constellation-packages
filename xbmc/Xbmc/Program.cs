/*
 *	 XBMC/Kodi Package for Constellation
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

namespace Xbmc
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbmc.Core;

    /// <summary>
    /// XBMC/Kodi Package
    /// </summary>
    public class Program : PackageBase
    {
        private Dictionary<string, Connection> xbmcConnections = new Dictionary<string, Connection>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            foreach (XbmcHost host in PackageHost.GetSettingAsConfigurationSection<XbmcConfigurationSection>("xbmcConfigurationSection").Hosts)
            {
                PackageHost.WriteInfo("Loading {0} ({1})", host.Name, host.Host);
                var currentState = new XbmcState() { Host = host.Host, Name = host.Name, Port = host.Port };
                var xbmcConnection = new Connection(host.Host, host.Port.ToString(), host.Login, host.Password);
                xbmcConnections.Add(host.Name, xbmcConnection);

                Task.Factory.StartNew(() =>
                {
                    while (PackageHost.IsRunning)
                    {
                        try
                        {
                            if (xbmcConnection.JsonRpc.PingAsync().GetAwaiter().GetResult())
                            {
                                if (!currentState.IsConnected)
                                {
                                    PackageHost.WriteInfo("{0} is connected", host.Name);
                                }

                                var currentPlayer = xbmcConnection.Player.GetActivePlayersAsync().GetAwaiter().GetResult().FirstOrDefault();
                                if (currentPlayer != null)
                                {
                                    var playerProperty = xbmcConnection.Player.GetPropertiesAsync(currentPlayer.PlayerId).GetAwaiter().GetResult();
                                    var playerItem = xbmcConnection.Player.GetItemAsync(currentPlayer.PlayerId).GetAwaiter().GetResult();

                                    if (currentState.PlayerItem == null || currentState.PlayerItem.Id != playerItem.Id)
                                    {
                                        PackageHost.WriteInfo("{0} is playing {1}", host.Name, playerItem.Title);
                                    }
                                    if (currentState.PlayerState != null && currentState.PlayerState.Speed != playerProperty.Speed)
                                    {
                                        PackageHost.WriteInfo("{0} : {1}", host.Name, playerProperty.Speed == 0 ? "PAUSE" : "PLAY");
                                    }

                                    currentState.PlayerState = playerProperty;
                                    currentState.PlayerItem = playerItem;
                                }
                                else if (currentState.PlayerState != null && currentState.PlayerItem != null)
                                {
                                    currentState.PlayerState = null;
                                    currentState.PlayerItem = null;
                                    PackageHost.WriteInfo("{0} Player stop", host.Name);
                                }

                                currentState.IsConnected = true;
                            }
                        }
                        catch (System.Net.WebException)
                        {
                            if (currentState.IsConnected)
                            {
                                PackageHost.WriteInfo("{0} is disconnected", host.Name);
                            }

                            currentState.IsConnected = false;
                            currentState.PlayerState = null;
                            currentState.PlayerItem = null;
                        }
                        catch (Exception ex)
                        {
                            PackageHost.WriteError("Error while pulling '{0}' : {1}", host.Name, ex.Message);
                        }

                        PackageHost.PushStateObject<XbmcState>(host.Name, currentState);
                        Thread.Sleep(host.Interval);
                    }
                });
            }
        }

        /// <summary>
        /// Start playback of a movie with the given ID
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="itemId">The item identifier.</param>
        [MessageCallback]
        public void OpenMovie(string xbmcName, int itemId)
        {
            this.Execute(xbmcName, connection => connection.Player.OpenAsync(movieId: itemId));
        }

        /// <summary>
        /// Start playback of an episode with the given ID
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="itemId">The item identifier.</param>
        [MessageCallback]
        public void OpenEpisode(string xbmcName, int itemId)
        {
            this.Execute(xbmcName, connection => connection.Player.OpenAsync(episodeId: itemId));
        }

        /// <summary>
        /// Pauses or unpause playback
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public void PlayPause(string xbmcName)
        {
            this.Execute(xbmcName, connection => connection.Player.PlayPauseAsync(1));
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public void Stop(string xbmcName)
        {
            this.Execute(xbmcName, connection => connection.Player.StopAsync(1));
        }

        /// <summary>
        /// Sends the input key to the XBMC host.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="inputKey">The input key.</param>
        [MessageCallback]
        public void SendInputKey(string xbmcName, InputKey inputKey)
        {
            switch (inputKey)
            {
                case InputKey.Back:
                    this.Execute(xbmcName, connection => connection.Input.BackAsync());
                    break;
                case InputKey.ContextMenu:
                    this.Execute(xbmcName, connection => connection.Input.ContextMenuAsync());
                    break;
                case InputKey.Down:
                    this.Execute(xbmcName, connection => connection.Input.DownAsync());
                    break;
                case InputKey.Home:
                    this.Execute(xbmcName, connection => connection.Input.HomeAsync());
                    break;
                case InputKey.Info:
                    this.Execute(xbmcName, connection => connection.Input.InfoAsync());
                    break;
                case InputKey.Left:
                    this.Execute(xbmcName, connection => connection.Input.LeftAsync());
                    break;
                case InputKey.Right:
                    this.Execute(xbmcName, connection => connection.Input.RightAsync());
                    break;
                case InputKey.Select:
                    this.Execute(xbmcName, connection => connection.Input.SelectAsync());
                    break;
                case InputKey.Up:
                    this.Execute(xbmcName, connection => connection.Input.UpAsync());
                    break;
            }
        }

        /// <summary>
        /// Scans the video sources for new library items.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public void ScanVideoLibrary(string xbmcName)
        {
            this.Execute(xbmcName, connection => connection.VideoLibrary.ScanAsync());
        }

        /// <summary>
        /// Scans the audio sources for new library items.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public void ScanAudioLibrary(string xbmcName)
        {
            this.Execute(xbmcName, connection => connection.AudioLibrary.ScanAsync());
        }

        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="volume">The volume.</param>
        [MessageCallback]
        public void SetVolume(string xbmcName, int volume)
        {
            this.Execute(xbmcName, connection => connection.Application.SetVolumeAsync(volume));
        }

        /// <summary>
        /// Sets the mute mode.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="mute">if set to <c>true</c> to mute.</param>
        [MessageCallback]
        public void SetMute(string xbmcName, bool mute)
        {
            this.Execute(xbmcName, connection => connection.Application.SetMuteAsync(mute));
        }

        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="request">The notification request.</param>
        [MessageCallback]
        public void ShowNotification(string xbmcName, NotificationRequest request)
        {
            this.Execute(xbmcName, connection => connection.Gui.ShowNotification(request.Title, request.Message, request.Image, request.DisplayTime == 0 ? 5000 : request.DisplayTime));
        }

        private void Execute(string xbmcName, Func<Connection, Task> action)
        {
            if (xbmcConnections.ContainsKey(xbmcName))
            {
                action(xbmcConnections[xbmcName]).Start();
            }
            else
            {
                PackageHost.WriteWarn("{0} not exist !", xbmcName);
            }
        }
    }
}

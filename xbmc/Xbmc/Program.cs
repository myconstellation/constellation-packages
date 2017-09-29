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
    using System.Threading.Tasks;
    using Xbmc.Core;

    /// <summary>
    /// XBMC/Kodi Package
    /// </summary>
    public class Program : PackageBase
    {
        private const int COMMAND_TIMEOUT = 5000; //ms
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

                Task.Factory.StartNew(async () =>
                {
                    while (PackageHost.IsRunning)
                    {
                        try
                        {
                            bool isAlive = await xbmcConnection.JsonRpc.PingAsync();
                            if (isAlive)
                            {
                                if (!currentState.IsConnected)
                                {
                                    PackageHost.WriteInfo("{0} is connected", host.Name);
                                }
                                currentState.IsConnected = true;

                                var currentPlayer = (await xbmcConnection.Player.GetActivePlayersAsync()).FirstOrDefault();
                                if (currentPlayer != null)
                                {
                                    var playerProperty = await xbmcConnection.Player.GetPropertiesAsync(currentPlayer.PlayerId);
                                    var playerItem = await xbmcConnection.Player.GetItemAsync(currentPlayer.PlayerId);

                                    if (currentState.PlayerItem == null || currentState.PlayerItem.Id != playerItem.Id)
                                    {
                                        PackageHost.WriteInfo("{0} is currently playing {1}", host.Name, playerItem.Title);
                                    }
                                    if (currentState.PlayerState != null && currentState.PlayerState.Speed != playerProperty.Speed)
                                    {
                                        PackageHost.WriteInfo("{0} is {1} {2}", host.Name, playerProperty.Speed == 0 ? "pausing" : "playing", playerItem.Title);
                                    }

                                    currentState.PlayerState = playerProperty;
                                    currentState.PlayerItem = playerItem;
                                }
                                else if (currentState.PlayerState != null && currentState.PlayerItem != null)
                                {
                                    currentState.PlayerState = null;
                                    currentState.PlayerItem = null;
                                    PackageHost.WriteInfo("{0} player stopped !", host.Name);
                                }
                            }
                        }
                        catch (System.Net.WebException ex) when (ex.InnerException is ObjectDisposedException)
                        {
                            // Do nothing (mono bug)
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

                        int interval = host.Interval / 1000;
                        PackageHost.PushStateObject<XbmcState>(host.Name, currentState,
                            lifetime: interval > 10 ? interval : 10);
                        await Task.Delay(host.Interval);
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Start playback of a movie with the given ID
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="itemId">The item identifier.</param>
        [MessageCallback]
        public bool OpenMovie(string xbmcName, int itemId)
        {
            return this.Execute(xbmcName, connection => connection.Player.OpenAsync(movieId: itemId));
        }

        /// <summary>
        /// Start playback of an episode with the given ID
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="itemId">The item identifier.</param>
        [MessageCallback]
        public bool OpenEpisode(string xbmcName, int itemId)
        {
            return this.Execute(xbmcName, connection => connection.Player.OpenAsync(episodeId: itemId));
        }

        /// <summary>
        /// Start playback of a song with the given ID
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="itemId">The item identifier.</param>
        [MessageCallback]
        public bool OpenSong(string xbmcName, int itemId)
        {
            return this.Execute(xbmcName, connection => connection.Player.OpenAsync(songId: itemId));
        }

        /// <summary>
        /// Start playback of an item
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="item>The item.</param>
        [MessageCallback]
        public bool OpenItem(string xbmcName, Core.Model.PlaylistItem item)
        {
            return this.Execute(xbmcName, connection => connection.Player.OpenAsync(item));
        }

        /// <summary>
        /// Pauses or unpause playback
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public bool PlayPause(string xbmcName)
        {
            return this.Execute(xbmcName, async connection =>
            {
                var currentPlayer = (await connection.Player.GetActivePlayersAsync()).FirstOrDefault();
                if (currentPlayer != null)
                {
                    await connection.Player.PlayPauseAsync(currentPlayer.PlayerId);
                }
            });
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public bool Stop(string xbmcName)
        {
            return this.Execute(xbmcName, async connection =>
            {
                var currentPlayer = (await connection.Player.GetActivePlayersAsync()).FirstOrDefault();
                if (currentPlayer != null)
                {
                    await connection.Player.StopAsync(currentPlayer.PlayerId);
                }
            });
        }

        /// <summary>
        /// Sends the input key to the XBMC host.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="inputKey">The input key.</param>
        [MessageCallback]
        public bool SendInputKey(string xbmcName, InputKey inputKey)
        {
            switch (inputKey)
            {
                case InputKey.Back:
                    return this.Execute(xbmcName, connection => connection.Input.BackAsync());
                case InputKey.ContextMenu:
                    return this.Execute(xbmcName, connection => connection.Input.ContextMenuAsync());
                case InputKey.Down:
                    return this.Execute(xbmcName, connection => connection.Input.DownAsync());
                case InputKey.Home:
                    return this.Execute(xbmcName, connection => connection.Input.HomeAsync());
                case InputKey.Info:
                    return this.Execute(xbmcName, connection => connection.Input.InfoAsync());
                case InputKey.Left:
                    return this.Execute(xbmcName, connection => connection.Input.LeftAsync());
                case InputKey.Right:
                    return this.Execute(xbmcName, connection => connection.Input.RightAsync());
                case InputKey.Select:
                    return this.Execute(xbmcName, connection => connection.Input.SelectAsync());
                case InputKey.Up:
                    return this.Execute(xbmcName, connection => connection.Input.UpAsync());
                default:
                    return false;
            }
        }

        /// <summary>
        /// Scans the video sources for new library items.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public bool ScanVideoLibrary(string xbmcName)
        {
            return this.Execute(xbmcName, connection => connection.VideoLibrary.ScanAsync());
        }

        /// <summary>
        /// Scans the audio sources for new library items.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        [MessageCallback]
        public bool ScanAudioLibrary(string xbmcName)
        {
            return this.Execute(xbmcName, connection => connection.AudioLibrary.ScanAsync());
        }

        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="volume">The volume.</param>
        [MessageCallback]
        public bool SetVolume(string xbmcName, int volume)
        {
            return this.Execute(xbmcName, connection => connection.Application.SetVolumeAsync(volume));
        }

        /// <summary>
        /// Sets the mute mode.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="mute">if set to <c>true</c> to mute.</param>
        [MessageCallback]
        public bool SetMute(string xbmcName, bool mute)
        {
            return this.Execute(xbmcName, connection => connection.Application.SetMuteAsync(mute));
        }

        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="xbmcName">Name of the XBMC.</param>
        /// <param name="request">The notification request.</param>
        [MessageCallback]
        public bool ShowNotification(string xbmcName, NotificationRequest request)
        {
            return this.Execute(xbmcName, connection => connection.Gui.ShowNotification(request.Title, request.Message, request.Image, request.DisplayTime == 0 ? 5000 : request.DisplayTime));
        }

        private bool Execute(string xbmcName, Func<Connection, Task> action)
        {
            if (xbmcConnections.ContainsKey(xbmcName))
            {
                Task task = action(xbmcConnections[xbmcName]);
                try
                {
                    return task.Wait(COMMAND_TIMEOUT) && !task.IsFaulted;
                }
                catch (Exception ex)
                {
                    PackageHost.WriteError("Unable to execute the command on {0} : {1}", xbmcName, ex.ToString());
                    return false;
                }
            }
            else
            {
                PackageHost.WriteWarn("{0} not exist !", xbmcName);
                return false;
            }
        }
    }
}

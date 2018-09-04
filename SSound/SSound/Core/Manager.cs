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

namespace SSound.Core
{
    using Constellation.Package;
    using NAudio.CoreAudioApi;
    using NAudio.Wave;
    using SSound.Core.Players;
    using SSound.Core.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// S-Sound Manager
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Manager : PackageBase
    {
        private static object syncLock = new object();
        private static SSoundConfigurationSection configuration = null;

        private Dictionary<string, PlayerBase> waveInPlayers = new Dictionary<string, PlayerBase>();
        private MMDevice outDevice = null;
        private IWavePlayer outPlayer = null;
        private Dlna.Device dlnaDevice = new Dlna.Device();
        private PlayerBase loadingPlayer = null, currentPlayer = null;
        private bool isInPlaylist = false;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The singleton instance.
        /// </value>
        public static Manager Instance => PackageHost.Package as Manager;

        /// <summary>
        /// Gets the name of the endpoint.
        /// </summary>
        /// <value>
        /// The name of the endpoint.
        /// </value>
        public string EndpointName => configuration?.EndpointName ?? Environment.MachineName;

        /// <summary>
        /// Gets the current player.
        /// </summary>
        /// <value>
        /// The current player.
        /// </value>
        public PlayerBase CurrentPlayer => this.currentPlayer;

        /// <summary>
        /// Called when start the package.
        /// </summary>
        /// <exception cref="Exception">Unable to read the SSound configuration section !</exception>
        public override void OnStart()
        {
            if (!PackageHost.TryGetSettingAsConfigurationSection<SSoundConfigurationSection>("ssoundConfiguration", out configuration))
            {
                PackageHost.WriteError("Unable to read the configuration, the defaut configuration will be use !");
            }
            PackageHost.SubscribeMessages("SSound");
            PackageHost.WriteInfo("Starting S-Sound endpoint '{0}'", this.EndpointName);
            // Discovering and load the Output device
            this.PushAllDevices();
            this.outDevice = this.GetDevice(configuration?.OutputDeviceName);
            this.outDevice.AudioEndpointVolume.OnVolumeNotification += (AudioVolumeNotificationData data) =>
            {
                PackageHost.PushStateObject<bool>("Mute", this.outDevice.AudioEndpointVolume.Mute);
                PackageHost.PushStateObject<float>("Volume", this.outPlayer.Volume);
                PackageHost.WriteInfo("AudioEndpointVolume notification. Volume: {0} - IsMute: {1}",
                    outPlayer.Volume, this.outDevice.AudioEndpointVolume.Mute);
            };
            PackageHost.WriteInfo("Output device : '{0}' ({1})", this.outDevice.FriendlyName, this.outDevice.ID);
            PackageHost.PushStateObject("OutDevice", new SSoundDevice(this.outDevice));
            // Create output player
            this.outPlayer = this.CreateWasapiOut(this.outDevice);
            this.outPlayer.PlaybackStopped += (object sender, StoppedEventArgs e) =>
            {
                this.Stop();
            };
            if (configuration?.InitialVolume >= 0)
            {
                this.outPlayer.Volume = (float)configuration.InitialVolume;
            }
            // Push current state
            PackageHost.PushStateObject("CurrentPlayer", string.Empty);
            PackageHost.PushStateObject<bool>("Mute", this.outDevice.AudioEndpointVolume.Mute);
            PackageHost.PushStateObject<float>("Volume", this.outPlayer.Volume);
            PackageHost.PushStateObject<PlaybackState>("State", PlaybackState.Stopped);
            // Load WaveInPlayers
            this.LoadWaveInPlayers();
            // Started DLNA MediaRenderer is needeed
            if (configuration?.EnableDlnaRenderer ?? true)
            {
                dlnaDevice.StartRenderer(this.EndpointName);
            }
            // SSound is ready !
            PackageHost.WriteInfo("S-Sound '{0}' is ready", this.EndpointName);
        }

        /// <summary>
        /// Called before Shutdown the package (the Constellation connection is still open).
        /// </summary>
        public override void OnPreShutdown()
        {
            PackageHost.WriteInfo("Shutdown S-Sound endpoint '{0}'", this.EndpointName);
            this.Stop();
            this.dlnaDevice.StopRenderer();
            this.outPlayer.Dispose();
            foreach (WaveInPlayer item in this.waveInPlayers.Values)
            {
                item.Close();
            }
        }

        #region Players Control

        /// <summary>
        /// Plays the media ressource.
        /// </summary>
        /// <param name="uri">The URI.</param>
        [MessageCallback]
        public void PlayMediaRessource(string uri)
        {
            this.isInPlaylist = false;
            this.PlayWaveProvider(new MediaPlayer().ConfigureWith(uri));
        }


        /// <summary>
        /// Plays the M3U playlist.
        /// </summary>
        /// <param name="uri">The URI.</param>
        [MessageCallback]
        public void PlayM3UList(string uri)
        {
            var tracks = M3UReader.Read(uri);
            if (tracks.Count > 0)
            {
                this.PlayMediaRessourceList(tracks.ToArray());
            }
            else
            {
                PackageHost.WriteError("Can't read the playlist or the playlist is empty (uri: {0})", uri);
            }
        }

        /// <summary>
        /// Plays the media ressource list.
        /// </summary>
        /// <param name="uris">The uris.</param>
        [MessageCallback]
        public void PlayMediaRessourceList(string[] uris)
        {
            Func<int, bool?> playNextSound = (i) =>
            {
                if (i < uris.Length && this.isInPlaylist)
                {
                    var p = new MediaPlayer().ConfigureWith(uris[i]);
                    try
                    {
                        if (p.Load())
                        {
                            this.PlayWaveProvider(p);
                            return true;
                        }
                    }
                    catch { }
                    PackageHost.WriteError("Can't load the URI '{0}' in your playlist", uris[i]);
                    return false;
                }
                else
                {
                    return null;
                }
            };

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                this.isInPlaylist = true;

                bool? result;
                do { result = playNextSound(i++); } while (!result.HasValue || !result.Value);

                outPlayer.PlaybackStopped += (s, e) =>
                {
                    do { result = playNextSound(i++); } while (!result.HasValue || !result.Value);
                };
            });
        }

        /// <summary>
        /// Plays the MP3 stream.
        /// </summary>
        /// <param name="uri">The URI.</param>
        [MessageCallback]
        public void PlayMP3Streaming(string uri)
        {
            this.isInPlaylist = false;
            this.PlayWaveProvider(new StreamingPlayer().ConfigureWith(uri));
        }

        /// <summary>
        /// Plays the device input device.
        /// </summary>
        /// <param name="name">Device name or friendly name.</param>
        [MessageCallback]
        public void PlayWaveIn(string name)
        {
            if (this.waveInPlayers.ContainsKey(name))
            {
                this.isInPlaylist = false;
                this.PlayWaveProvider(this.waveInPlayers[name]);
            }
            else
            {
                PackageHost.WriteError("PlayWaveIn error : '{0}' not exist", name);
            }
        }

        /// <summary>
        /// Speeches the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        [MessageCallback]
        public void Speech(string text)
        {
            if (configuration == null || configuration.Cerevoice == null)
            {
                throw new Exception("No CereVoice configuration found. Check your settings!");
            }

            PackageHost.WriteInfo("Speeching '{0}'", text);
            CereVoice.CereVoiceCloudPortType cereClient = new CereVoice.CereVoiceCloudPortTypeClient();
            var response = cereClient.speakExtended(new CereVoice.speakExtendedRequest(configuration.Cerevoice.AccountID, configuration.Cerevoice.Password, configuration.Cerevoice.VoiceName, text, "mp3", configuration.Cerevoice.Bitrate, true, false));
            if (response.resultCode != 1)
            {
                PackageHost.WriteError("CereVoice request failed : " + response.resultDescription);
            }
            else
            {
                this.PlayWaveProvider(new MediaPlayer().ConfigureWith(response.fileUrl, true, configuration?.SpeechVolume > 0 ? (float)configuration.SpeechVolume : this.outPlayer.Volume));
            }
        }

        #endregion

        #region Wave Controls

        /// <summary>
        /// Plays the current player.
        /// </summary>
        [MessageCallback]
        public void Play()
        {
            if (this.outPlayer != null)
            {
                this.outPlayer.Play();
                PackageHost.WriteInfo("State: Play");
                PackageHost.PushStateObject<PlaybackState>("State", PlaybackState.Playing);
            }
        }

        /// <summary>
        /// Pauses the current player.
        /// </summary>
        [MessageCallback]
        public void Pause()
        {
            if (this.outPlayer != null)
            {
                this.outPlayer.Pause();
                PackageHost.WriteInfo("State: Pause");
                PackageHost.PushStateObject<PlaybackState>("State", PlaybackState.Paused);
            }
        }

        /// <summary>
        /// Stops the current player.
        /// </summary>
        [MessageCallback]
        public void Stop()
        {
            if (this.outPlayer != null)
            {
                if (this.loadingPlayer != null)
                {
                    this.loadingPlayer.Dispose();
                    this.loadingPlayer = null;
                }
                if (this.currentPlayer != null)
                {
                    this.currentPlayer.Dispose();
                    this.currentPlayer = null;
                }
                this.outPlayer.Stop();
                PackageHost.WriteInfo("State: Stop");
                PackageHost.PushStateObject<PlaybackState>("State", PlaybackState.Stopped);
                PackageHost.PushStateObject("CurrentPlayer", string.Empty);
            }
        }

        /// <summary>
        /// Sets the volume: 1.0 is full scale, 0.0 is silence
        /// </summary>
        /// <param name="level">The level.</param>
        [MessageCallback]
        public void SetVolume(float level)
        {
            if (level >= 0 && level <= 1)
            {
                this.outPlayer.Volume = level;
            }
            else
            {
                PackageHost.WriteError("SetVolume: Error, the level must be between 0.0 and 1.0");
            }
        }

        /// <summary>
        /// Set mute or un-mute the output device
        /// </summary>
        /// <param name="mute">if set to <c>true</c>, mute the output device.</param>
        [MessageCallback]
        public void SetMute(bool mute)
        {
            this.outDevice.AudioEndpointVolume.Mute = mute;
        }

        #endregion

        #region Private

        private void PlayWaveProvider(PlayerBase player)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (!player.UseNewWaveOut)
                        {
                            if (this.loadingPlayer != null)
                            {
                                this.loadingPlayer.Dispose();
                            }
                            this.loadingPlayer = player;
                        }

                        if (player.Load())
                        {
                            lock (syncLock)
                            {
                                if (!player.UseNewWaveOut)
                                {
                                    // Replace the current player by the new one
                                    this.loadingPlayer = null;
                                    this.Stop();
                                    System.Threading.Thread.Sleep(200);
                                    this.currentPlayer = player;
                                    this.outPlayer = this.CreateWasapiOut(this.outDevice);
                                    this.outPlayer.Init(player.WaveProvider);
                                    PackageHost.PushStateObject("CurrentPlayer", player.ToString());
                                    this.Play();
                                }
                                else
                                {
                                    // Create a new temporary WaveOut
                                    var newWaveOut = this.CreateWasapiOut(this.outDevice);
                                    newWaveOut.Init(player.WaveProvider);
                                    // No current player ?
                                    if (this.currentPlayer == null)
                                    {
                                        // Just play the new wave out !
                                        newWaveOut.Play();
                                    }
                                    else
                                    {
                                        // Create a copy of the current player to decrease the volume
                                        var currentWaveOutLowered = this.CreateWasapiOut(this.outDevice);
                                        var currentStreamLowered = new VolumeWaveProvider16(this.currentPlayer.WaveProvider);
                                        currentStreamLowered.Volume = this.outPlayer.Volume / 2;
                                        currentWaveOutLowered.Init(currentStreamLowered);
                                        // When the new wave out playback stopped, resume the original player and stop the lowered copy
                                        newWaveOut.PlaybackStopped += (s, e) =>
                                        {
                                            this.outPlayer.Play();
                                            currentWaveOutLowered.Stop();
                                            currentWaveOutLowered.Dispose();
                                        };
                                        // Now, play the new wave out and the lowered copy & stop the current player
                                        outPlayer.Stop();
                                        currentWaveOutLowered.Play();
                                        newWaveOut.Play();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PackageHost.WriteError("Error to play '{0}' : {1}", player.ToString(), ex.ToString());
                        player.SetStatus("Error");
                    }
                });
        }

        private void LoadWaveInPlayers()
        {
            this.waveInPlayers.Clear();
            var deviceEnumerator = new MMDeviceEnumerator();

            foreach (var device in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                try
                {
                    var deviceInConfig = configuration?.Inputs?.OfType<InputDeviceElement>()
                        .SingleOrDefault(d => d.InputDeviceName.Equals(device.FriendlyName, StringComparison.OrdinalIgnoreCase) || d.InputDeviceName.Equals(device.ID, StringComparison.OrdinalIgnoreCase));

                    var playerArgs = new WaveInPlayerArguments()
                    {
                        Device = device,
                        Source = new WasapiCapture(device, true),
                        FriendlyName = deviceInConfig?.Name ?? device.FriendlyName,
                        DeviceName = device.FriendlyName
                    };
                    PackageHost.WriteInfo("WaveIn: '{0}' ({1}) is loaded ", playerArgs.FriendlyName, playerArgs.DeviceName);

                    if (deviceInConfig != null)
                    {
                        playerArgs.AutoPlay = deviceInConfig.AutoPlay;
                        playerArgs.BufferDuration = deviceInConfig.BufferMilliseconds;
                        playerArgs.SignalThreshold = deviceInConfig.SignalThreshold;
                        playerArgs.HasSignalDuration = deviceInConfig.HasSignalDuration;
                        playerArgs.NoSignalDuration = deviceInConfig.NoSignalDuration;
                    }

                    var player = new WaveInPlayer().ConfigureWith(playerArgs);
                    if (player.Arguments.AutoPlay)
                    {
                        ((WaveInPlayer)player).Listening();
                    }
                    this.waveInPlayers.Add(playerArgs.FriendlyName, player);
                }
                catch (Exception ex)
                {
                    PackageHost.WriteError("Unable to load WaveInDevices : " + ex.ToString());
                }
            }

            PackageHost.PushStateObject("WaveInPlayers", this.waveInPlayers, metadatas: new Dictionary<string, object>() { { "EndpointName", this.EndpointName } });
        }

        private IWavePlayer CreateWasapiOut(MMDevice device)
        {
            return new WasapiOut(device, AudioClientShareMode.Shared, true, 0);
        }

        private MMDevice GetDevice(string deviceIdOrName = null)
        {
            MMDevice device = null;
            var deviceEnumerator = new MMDeviceEnumerator();
            // Get the device name from the configuration
            if (string.IsNullOrEmpty(deviceIdOrName) && !string.IsNullOrEmpty(configuration?.OutputDeviceName))
            {
                deviceIdOrName = configuration.OutputDeviceName;
            }
            // If the deviceName is not null
            if (!string.IsNullOrEmpty(deviceIdOrName))
            {
                foreach (MMDevice item in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    if (item.ID.Equals(deviceIdOrName, StringComparison.OrdinalIgnoreCase) ||
                        item.FriendlyName.Equals(deviceIdOrName, StringComparison.OrdinalIgnoreCase))
                    {
                        device = item;
                        break;
                    }
                }
            }
            else
            {
                // Otherwise, get the default device
                device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            // Return the device if found!
            if (device == null)
            {
                throw new ArgumentException("Unable to find the device");
            }
            return device;
        }

        private void PushAllDevices()
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            PackageHost.PushStateObject("InDevices",
                deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All).Select(d => new SSoundDevice(d)).ToList(),
                metadatas: new Dictionary<string, object>() { { "EndpointName", this.EndpointName } });
            PackageHost.PushStateObject("OutDevices",
                deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All).Select(d => new SSoundDevice(d)).ToList(),
                metadatas: new Dictionary<string, object>() { { "EndpointName", this.EndpointName } });
        }

        #endregion
    }
}

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

namespace SSound.Core.Players
{
    using NAudio.CoreAudioApi;
    using NAudio.Wave;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Wave-In (audio capture) player
    /// </summary>
    /// <seealso cref="SSound.Core.PlayerBase{SSound.Core.Players.WaveInPlayerArguments}" />
    public class WaveInPlayer : PlayerBase<WaveInPlayerArguments>
    {
        internal const int DEFAULT_SIGNAL_THRESHOLD = 5; // %
        internal const int DEFAULT_WAVEIN_BUFFER_TIME = 25; // ms
        internal const int DEFAULT_HAS_SIGNAL_PERIOD = 2000; // ms
        internal const int DEFAULT_NO_SIGNAL_PERIOD = 30000; // ms

        private bool isListening = false, isPlaying = false;
        private DateTime noSignalStartDate = DateTime.MinValue, signalStartDate = DateTime.MinValue;

        private IWaveIn waveIn = null;
        private BufferedWaveProvider waveProvider = null;

        /// <summary>
        /// Gets the wave provider.
        /// </summary>
        /// <value>
        /// The wave provider.
        /// </value>
        public override IWaveProvider WaveProvider
        {
            get { return this.waveProvider; }
        }

        /// <summary>
        /// Configures this player.
        /// </summary>
        public override void Configure()
        {
            // Config values
            if (this.Arguments.HasSignalDuration <= 0)
            {
                this.Arguments.HasSignalDuration = DEFAULT_HAS_SIGNAL_PERIOD;
            }
            if (this.Arguments.NoSignalDuration <= 0)
            {
                this.Arguments.NoSignalDuration = DEFAULT_NO_SIGNAL_PERIOD;
            }
            if (this.Arguments.BufferDuration <= 0)
            {
                this.Arguments.BufferDuration = DEFAULT_WAVEIN_BUFFER_TIME;
            }
            if (this.Arguments.SignalThreshold <= 0)
            {
                this.Arguments.SignalThreshold = DEFAULT_SIGNAL_THRESHOLD;
            }
            // Configure the Wave input
            this.waveIn = this.Arguments.Source;
            this.waveIn.DataAvailable += this.waveIn_DataAvailable;
            this.waveIn.RecordingStopped += this.waveIn_RecordingStopped;
            // create wave provider
            this.waveProvider = new BufferedWaveProvider(this.waveIn.WaveFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromMilliseconds(this.Arguments.BufferDuration)
            };
        }

        /// <summary>
        /// Listening the device
        /// </summary>
        public void Listening()
        {
            if (!this.isListening)
            {
                // start recording
                this.isListening = true;
                this.waveIn.StartRecording();
                this.SetStatus("Listening");
            }
        }

        /// <summary>
        /// Loads this player.
        /// </summary>
        /// <returns></returns>
        public override bool Load()
        {
            this.Listening();
            this.ResetSignalDetectionDates();
            this.isPlaying = true;
            this.SetStatus("Playing");
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.ResetSignalDetectionDates();
            this.isPlaying = false;
            this.SetStatus("Stop");
        }

        /// <summary>
        /// Closes this player.
        /// </summary>
        public void Close()
        {
            this.isListening = false;
            this.isPlaying = false;
            this.SetStatus("Stop");
            if (this.waveIn != null)
            {
                this.waveIn.StopRecording();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}<{1}>", base.ToString(), this.Arguments.FriendlyName);
        }

        private void ResetSignalDetectionDates()
        {
            this.noSignalStartDate = DateTime.MinValue;
            this.signalStartDate = DateTime.MinValue;
        }

        private void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (!this.isListening)
            {
                // dispose of wave input
                if (this.waveIn != null)
                {
                    this.waveIn.Dispose();
                    this.waveIn = null;
                }
                // drop wave provider
                this.waveProvider = null;
            }
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.isListening && this.waveProvider != null)
            {
                if (this.HasAudioSignal()) // ==> Signal detected
                {
                    this.noSignalStartDate = DateTime.MinValue;
                    if (!this.isPlaying && Manager.Instance.CurrentPlayer == null && this.Arguments.AutoPlay)
                    {
                        if (this.signalStartDate == DateTime.MinValue)
                        {
                            this.signalStartDate = DateTime.Now;
                        }
                        else if (DateTime.Now.Subtract(this.signalStartDate).TotalMilliseconds >= this.Arguments.HasSignalDuration)
                        {
                            this.ResetSignalDetectionDates();
                            this.isPlaying = true;
                            this.SetStatus("Signal detected, play !");
                            Manager.Instance.PlayWaveIn(this.Arguments.FriendlyName);
                            return;
                        }
                    }
                }
                else // ==> No signal
                {
                    this.signalStartDate = DateTime.MinValue;
                    if (this.isPlaying && this.noSignalStartDate == DateTime.MinValue)
                    {
                        this.noSignalStartDate = DateTime.Now;
                    }
                    else if (this.isPlaying && DateTime.Now.Subtract(this.noSignalStartDate).TotalMilliseconds >= this.Arguments.NoSignalDuration)
                    {
                        this.ResetSignalDetectionDates();
                        this.isPlaying = false;
                        this.SetStatus("No signal, playback stop");
                        Manager.Instance.Stop();
                        return;
                    }
                }

                if (this.isPlaying)
                {
                    // add received data to waveProvider buffer
                    this.waveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
                }
            }
        }

        private bool HasAudioSignal()
        {
            float volume = 0;
            for (int i = 0; i < this.Arguments.Device.AudioMeterInformation.PeakValues.Count; i++)
            {
                volume += this.Arguments.Device.AudioMeterInformation.PeakValues[i];
            }
            volume *= 100 / this.Arguments.Device.AudioMeterInformation.PeakValues.Count;
            return (volume >= this.Arguments.SignalThreshold);
        }
    }

    /// <summary>
    /// WaveInPlayer's Arguments
    /// </summary>
    public class WaveInPlayerArguments
    {
        [JsonIgnore]
        public MMDevice Device { get; set; }
        [JsonIgnore]
        public IWaveIn Source { get; set; }

        public string FriendlyName { get; set; }
        public string DeviceName { get; set; }
        public bool AutoPlay { get; set; }
        public int BufferDuration { get; set; }
        public int HasSignalDuration { get; set; }
        public int NoSignalDuration { get; set; }
        public double SignalThreshold { get; set; }

        public override string ToString()
        {
            return this.FriendlyName == this.DeviceName ? this.DeviceName : $"{this.FriendlyName} ({this.DeviceName})";
        }
    }
}

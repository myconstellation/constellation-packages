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
    using NAudio.CoreAudioApi;
    using NAudio.Wave;

    /// <summary>
    /// Represent a S-Sound In/Out device
    /// </summary>
    public class SSoundDevice
    {
        /// <summary>
        /// The device identifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The device friendly name.
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// The device wave format
        /// </summary>
        public WaveFormat MixFormat { get; set; }
        /// <summary>
        /// The device state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SSoundDevice"/> class.
        /// </summary>
        /// <param name="device">The MM device.</param>
        public SSoundDevice(MMDevice device)
        {
            this.ID = device.ID;
            this.FriendlyName = device.FriendlyName;
            this.State = device.State.ToString();
            if (device.State == DeviceState.Active)
            {
                this.MixFormat = device.AudioClient.MixFormat;
            }
        }
    }
}
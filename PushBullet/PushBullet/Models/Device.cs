/*
 *	 PushBullet Package for Constellation
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

namespace PushBullet.Models
{
    /// <summary>
    /// Represent a PushBullet's device
    /// </summary>
    public class Device : PushBulletDeletableBaseObject
    {
        /// <summary>
        /// Gets or sets the Icon to use for this device, can be an arbitrary string.
        /// Commonly used values are: "desktop", "browser", "website", "laptop", "tablet", "phone", "watch", "system".
        /// </summary>
        /// <value>
        /// The Icon to use for this device, can be an arbitrary string
        /// </value>
        [PushBulletProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the Name to use when displaying the device.
        /// </summary>
        /// <value>
        /// The Name to use when displaying the device.
        /// </value>
        [PushBulletProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the nickname was automatically generated from the manufacturer and model fields.
        /// </summary>
        /// <value>
        /// <c>true</c> if the nickname was automatically generated from the manufacturer and model fields (only used for some android phones)
        /// </value>
        [PushBulletProperty("generated_nickname")]
        public bool IsGeneratedNickname { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturer of the device.
        /// </summary>
        /// <value>
        /// The Manufacturer of the device.
        /// </value>
        [PushBulletProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the Model of the device.
        /// </summary>
        /// <value>
        /// The Model of the device.
        /// </value>
        [PushBulletProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the Version of the Pushbullet application installed on the device.
        /// </summary>
        /// <value>
        /// The Version of the Pushbullet application installed on the device.
        /// </value>
        [PushBulletProperty("app_version")]
        public int AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the fingerprint for the device, used by apps to avoid duplicate devices.
        /// Value is platform-specific.
        /// </summary>
        /// <value>
        /// The fingerprint for the device, used by apps to avoid duplicate devices.
        /// </value>
        [PushBulletProperty("fingerprint")]
        public string Fingerprint { get; set; }

        /// <summary>
        /// Gets or sets the Fingerprint for the device's end-to-end encryption key, used to determine which devices the current device (based on its own key fingerprint) will be able to talk to.
        /// </summary>
        /// <value>
        /// The Fingerprint for the device's end-to-end encryption key, used to determine which devices the current device (based on its own key fingerprint) will be able to talk to.
        /// </value>
        [PushBulletProperty("key_fingerprint")]
        public string FingerprintKey { get; set; }

        /// <summary>
        /// Gets or sets Platform-specific push token.
        /// </summary>
        /// <value>
        /// The Platform-specific push token.
        /// </value>
        [PushBulletProperty("push_token")]
        public string PushToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the devices has SMS capability (currently only true for type="android" devices)
        /// </summary>
        /// <value>
        ///   <c>true</c> if the devices has SMS capability, currently only true for type="android" devices.
        /// </value>
        [PushBulletProperty("has_sms")]
        public bool HasSMS { get; set; }
    }

    /// <summary>
    /// Represent a PushBullet's devices list
    /// </summary>
    public class DevicesList
    {
        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        [PushBulletProperty("devices")]
        public Device[] Devices { get; set; }
    }
}

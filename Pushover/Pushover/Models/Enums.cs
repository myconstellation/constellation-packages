/*
 *	 Pushover Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace Pushover
{
    /// <summary>
    /// Pushovers notification sounds
    /// </summary>
    public enum Sound
    {
        /// <summary>
        /// Pushover (default)
        /// </summary>
        Pushover = 0,
        /// <summary>
        /// Bike
        /// </summary>
        Bike = 1,
        /// <summary>
        /// Bugle
        /// </summary>
        Bugle = 2,
        /// <summary>
        /// Cash Register
        /// </summary>
        CashRegister = 3,
        /// <summary>
        /// Classical
        /// </summary>
        Classical = 4,
        /// <summary>
        /// Cosmic
        /// </summary>
        Cosmic = 5,
        /// <summary>
        /// Falling
        /// </summary>
        Falling = 6,
        /// <summary>
        /// Gamelan
        /// </summary>
        Gamelan = 7,
        /// <summary>
        /// Incoming
        /// </summary>
        Incoming = 8,
        /// <summary>
        /// Intermission
        /// </summary>
        Intermission = 9,
        /// <summary>
        /// Magic
        /// </summary>
        Magic = 10,
        /// <summary>
        /// Mechanical
        /// </summary>
        Mechanical = 11,
        /// <summary>
        /// Piano Bar
        /// </summary>
        PianoBar = 12,
        /// <summary>
        /// Siren
        /// </summary>
        Siren = 13,
        /// <summary>
        /// Space Alarm
        /// </summary>
        SpaceAlarm = 14,
        /// <summary>
        /// Tug Boat
        /// </summary>
        TugBoat = 15,
        /// <summary>
        /// Alien Alarm (long)
        /// </summary>
        Alien = 16,
        /// <summary>
        /// Climb (long)
        /// </summary>
        Climb = 17,
        /// <summary>
        /// Persistent (long)
        /// </summary>
        Persistent = 18,
        /// <summary>
        /// Pushover Echo (long)
        /// </summary>
        Echo = 19,
        /// <summary>
        /// Up Down (long)
        /// </summary>
        UpDown = 20,
        /// <summary>
        /// None (silent)
        /// </summary>
        None = 21
    }

    /// <summary>
    /// Pushover priority levels.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// The messages will be considered lowest priority and will not generate any notification. On iOS, the application badge number will be increased.
        /// </summary>
        Lowest = -2,
        /// <summary>
        /// The messages will be considered low priority and will not generate any sound or vibration, but will still generate a popup/scrolling notification depending on the client operating system.
        /// </summary>
        Low = -1,
        /// <summary>
        /// The messages will have the default priority. These messages trigger sound, vibration, and display an alert according to the user's device settings.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The messages bypass a user's quiet hours. These messages will always play a sound and vibrate (if the user's device is configured to) regardless of the delivery time. High-priority should only be used when necessary and appropriate.
        /// </summary>
        High = 1,
        /// <summary>
        /// Emergency-priority notifications are similar to high-priority notifications, but they are repeated until the notification is acknowledged by the user.
        /// </summary>
        Emergency = 2
    }
}

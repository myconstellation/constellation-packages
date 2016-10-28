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
    using Xbmc.Core.Model;

    /// <summary>
    /// XBMC Input key
    /// </summary>
    public enum InputKey
    {
        /// <summary>
        /// Back
        /// </summary>
        Back = 0,
        /// <summary>
        /// Select
        /// </summary>
        Select = 1,
        /// <summary>
        /// Home
        /// </summary>
        Home = 2,
        /// <summary>
        /// ContextMenu
        /// </summary>
        ContextMenu = 3,
        /// <summary>
        /// Info
        /// </summary>
        Info = 4,
        /// <summary>
        /// Down
        /// </summary>
        Down = 5,
        /// <summary>
        /// Left
        /// </summary>
        Left = 6,
        /// <summary>
        /// Right
        /// </summary>
        Right = 7,
        /// <summary>
        /// Up
        /// </summary>
        Up = 8
    }

    /// <summary>
    /// Notification request
    /// </summary>
    public class NotificationRequest
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public string Image { get; set; }
        /// <summary>
        /// Gets or sets the display time.
        /// </summary>
        /// <value>
        /// The display time.
        /// </value>
        public int DisplayTime { get; set; }
    }

    /// <summary>
    /// Represent the XBMC current state
    /// </summary>
    [StateObject]
    public class XbmcState
    {
        /// <summary>
        /// Gets or sets the XBMC host name.
        /// </summary>
        /// <value>
        /// The XBMC host name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the XBMC is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the XBMC is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; set; }
        /// <summary>
        /// Gets or sets the state of the player.
        /// </summary>
        /// <value>
        /// The state of the player.
        /// </value>
        public PlayerPropertyValue PlayerState { get; set; }
        /// <summary>
        /// Gets or sets the player item.
        /// </summary>
        /// <value>
        /// The player item.
        /// </value>
        public ListItemAll PlayerItem { get; set; } 
    }
}

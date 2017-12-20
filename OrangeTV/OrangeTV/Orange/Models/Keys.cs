/*
 *	 OrangeTV Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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
 
 namespace OrangeTV
{
    /// <summary>
    /// Keys availables for remote control
    /// </summary>
    public enum Key
    {
        [OrangeServiceId(116)]
        OnOff,
        [OrangeServiceId(512)]
        Key0,
        [OrangeServiceId(513)]
        Key1,
        [OrangeServiceId(514)]
        Key2,
        [OrangeServiceId(515)]
        Key3,
        [OrangeServiceId(516)]
        Key4,
        [OrangeServiceId(517)]
        Key5,
        [OrangeServiceId(518)]
        Key6,
        [OrangeServiceId(519)]
        Key7,
        [OrangeServiceId(520)]
        Key8,
        [OrangeServiceId(521)]
        Key9,
        [OrangeServiceId(402)]
        ChannelUp,
        [OrangeServiceId(403)]
        ChannelDown,
        [OrangeServiceId(115)]
        VolumeUp,
        [OrangeServiceId(114)]
        VolumeDown,
        [OrangeServiceId(113)]
        Mute,
        [OrangeServiceId(103)]
        Up,
        [OrangeServiceId(108)]
        Down,
        [OrangeServiceId(105)]
        Left,
        [OrangeServiceId(116)]
        Right,
        [OrangeServiceId(352)]
        OK,
        [OrangeServiceId(158)]
        Back,
        [OrangeServiceId(139)]
        Menu,
        [OrangeServiceId(164)]
        PlayPause,
        [OrangeServiceId(159)]
        FastForward,
        [OrangeServiceId(168)]
        FastBackward,
        [OrangeServiceId(167)]
        Record,
        [OrangeServiceId(393)]
        VOD,
    }

    /// <summary>
    /// PressKey modes
    /// </summary>
    public enum PressKeyMode
    {
        /// <summary>
        /// Press and release key
        /// </summary>
        [OrangeServiceId(0)]
        SinglePress,
        /// <summary>
        /// Press and hold key
        /// </summary>
        [OrangeServiceId(1)]
        PressKey,
        /// <summary>
        /// Release key
        /// </summary>
        [OrangeServiceId(2)]
        ReleaseKey
    }
}
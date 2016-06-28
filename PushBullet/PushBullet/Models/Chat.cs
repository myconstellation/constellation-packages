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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represent a PushBullet's chat
    /// </summary>
    public class Chat : PushBulletDeletableBaseObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether this chat is muted.
        /// </summary>
        /// <value>
        ///   If <c>true</c>, notifications from this chat will not be shown
        /// </value>
        [PushBulletProperty("muted")]
        public bool IsMuted { get; set; }

        /// <summary>
        /// Gets or sets the participant that the chat is with.
        /// </summary>
        /// <value>
        /// The participant that the chat is with.
        /// </value>
        [PushBulletProperty("with")]
        public ChatParticipant Participant { get; set; }        
    }

    /// <summary>
    /// Represent a PushBullet's chats list
    /// </summary>
    public class ChatsList
    {
        /// <summary>
        /// Gets or sets the pushes.
        /// </summary>
        /// <value>
        /// The pushes.
        /// </value>
        [PushBulletProperty("chats")]
        public Chat[] Chats { get; set; }
    }

    /// <summary>
    /// Represent a chat participant.
    /// </summary>
    public class ChatParticipant
    {
        /// <summary>
        /// Gets or sets the email address of the person.
        /// </summary>
        /// <value>
        /// The email address of the person.
        /// </value>
        [PushBulletProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the canonical email address of the person.
        /// </summary>
        /// <value>
        /// The canonical email address of the person.
        /// </value>
        [PushBulletProperty("email_normalized")]
        public string EmailNormalized { get; set; }

        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        /// <value>
        /// The name of the person.
        /// </value>
        [PushBulletProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the image to display for the person.
        /// </summary>
        /// <value>
        /// The image to display for the person.
        /// </value>
        [PushBulletProperty("image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the user identifier if this is a user.
        /// </summary>
        /// <value>
        /// If this is a user, the iden of that user.
        /// </value>
        [PushBulletProperty("iden")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of the participant (user or email).
        /// </summary>
        /// <value>
        /// The type of the participant (user or email).
        /// </value>
        [PushBulletProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChatParticpantType Type { get; set; }
    }

    /// <summary>
    /// Represent the type of chat participant "email" or "user"
    /// </summary>
    public enum ChatParticpantType
    {
        /// <summary>
        /// PushBullet user
        /// </summary>
        User,
        /// <summary>
        /// Non PushBullet user (email)
        /// </summary>
        Email
    }
}

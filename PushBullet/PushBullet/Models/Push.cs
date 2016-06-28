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
    /// Represent a PushBullet's push
    /// </summary>
    public class Push : PushBulletDeletableBaseObject
    {
        /// <summary>
        /// Gets or sets the type of the push.
        /// </summary>
        /// <value>
        /// The type of the push.
        /// </value>
        [PushBulletProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PushType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this push is dismissed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the push has been dismissed by any device or if any device was active when the push was received.
        /// </value>
        [PushBulletProperty("dismissed")]
        public bool IsDismissed { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier set by the client, used to identify a push in case you receive it from /v2/everything before the call to /v2/pushes has completed. 
        /// </summary>
        /// <value>
        /// The unique identifier set by the client, used to identify a push in case you receive it from /v2/everything before the call to /v2/pushes has completed.
        /// </value>
        [PushBulletProperty("guid")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the direction the push was sent in.
        /// </summary>
        /// <value>
        /// The direction the push was sent in.
        /// </value>
        [PushBulletProperty("direction")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PushDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the user iden of the sender.
        /// </summary>
        /// <value>
        /// The user iden of the sender.
        /// </value>
        [PushBulletProperty("sender_iden")]
        public string SenderUserId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the sender.
        /// </summary>
        /// <value>
        /// The email address of the sender.
        /// </value>
        [PushBulletProperty("sender_email")]
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the canonical email address of the sender.
        /// </summary>
        /// <value>
        /// The canonical email address of the sender.
        /// </value>
        [PushBulletProperty("sender_email_normalized")]
        public string SenderEmailNormalized { get; set; }

        /// <summary>
        /// Gets or sets the name of the sender.
        /// </summary>
        /// <value>
        /// The name of the sender.
        /// </value>
        [PushBulletProperty("sender_name")]
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the user iden of the receiver.
        /// </summary>
        /// <value>
        /// The user iden of the receiver.
        /// </value>
        [PushBulletProperty("receiver_iden")]
        public string ReceiverUserId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the receiver.
        /// </summary>
        /// <value>
        /// The email address of the receiver.
        /// </value>
        [PushBulletProperty("receiver_email")]
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// Gets or sets the canonical email address of the receiver.
        /// </summary>
        /// <value>
        /// The canonical email address of the receiver.
        /// </value>
        [PushBulletProperty("receiver_email_normalized")]
        public string ReceiverEmailNormalized { get; set; }

        /// <summary>
        /// Gets or sets the device iden of the target device, if sending to a single device.
        /// </summary>
        /// <value>
        /// Thee device iden of the target device, if sending to a single devicer.
        /// </value>
        [PushBulletProperty("target_device_iden")]
        public string TargetDeviceId { get; set; }

        /// <summary>
        /// Gets or sets the device iden of the sending device. Optionally set by the sender when creating a push.
        /// </summary>
        /// <value>
        /// The device iden of the sending device. Optionally set by the sender when creating a push.
        /// </value>
        [PushBulletProperty("source_device_iden")]
        public string SourcetDeviceId { get; set; }

        /// <summary>
        /// Gets or sets the iden of the client if the push was created by a client.
        /// </summary>
        /// <value>
        /// If the push was created by a client, set to the iden of that client.
        /// </value>
        [PushBulletProperty("client_iden")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the iden of that channelr if the push was created by a channel.
        /// </summary>
        /// <value>
        /// If the push was created by a channel, set to the iden of that channel.
        /// </value>
        [PushBulletProperty("channel_iden")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the list of guids (client side identifiers, not the guid field on pushes) for awake apps at the time the push was sent. If the length of this list is > 0, dismissed will be set to true and the awake app(s) must decide what to do with the notification.
        /// </summary>
        /// <value>
        /// The list of guids (client side identifiers, not the guid field on pushes) for awake apps at the time the push was sent.
        /// </value>
        [PushBulletProperty("awake_app_guids")]
        public string[] AwakeAppGuids { get; set; }

        /// <summary>
        /// Gets or sets the title of the push, used for all types of pushes
        /// </summary>
        /// <value>
        /// The title of the push.
        /// </value>
        [PushBulletProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the body, used for all types of pushes
        /// </summary>
        /// <value>
        /// The body of the push.
        /// </value>
        [PushBulletProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the URL, used for type="link" pushes
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [PushBulletProperty("url")]
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets the name of the file, used for type="file" pushes
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [PushBulletProperty("file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the mime type of the file, used for type="file" pushes
        /// </summary>
        /// <value>
        /// The mime type of the file.
        /// </value>
        [PushBulletProperty("file_type")]
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the file URL, used for type="file" pushes
        /// </summary>
        /// <value>
        /// The file URL.
        /// </value>
        [PushBulletProperty("file_url")]
        public string FileURL { get; set; }

        /// <summary>
        /// Gets or sets the image URL, used for type="file" pushes
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        [PushBulletProperty("image_url")]
        public string ImageURL { get; set; }

        /// <summary>
        /// Gets or sets the width of image in pixels, only present if ImageURL is set.
        /// </summary>
        /// <value>
        /// The width of image in pixels, only present if ImageURL is set.
        /// </value>
        [PushBulletProperty("image_width")]
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of image in pixels, only present if ImageURL is set.
        /// </summary>
        /// <value>
        /// The height of image in pixels, only present if ImageURL is set.
        /// </value>
        [PushBulletProperty("image_height")]
        public int ImageHeight { get; set; }
    }

    /// <summary>
    /// Represent request authorization to upload a file.
    /// </summary>
    public class RequestUploadAuthorization
    {
        /// <summary>
        /// Gets or sets the file name that will be used for the file.
        /// </summary>
        /// <value>
        /// The file name that will be used for the file.
        /// </value>
        [PushBulletProperty("file_name")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the file type that will be used for the file.
        /// </summary>
        /// <value>
        /// The file type that will be used for the file.
        /// </value>
        [PushBulletProperty("file_type")]
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the URL where the file will be available after it is uploaded.
        /// </summary>
        /// <value>
        /// The URL where the file will be available after it is uploaded.
        /// </value>
        [PushBulletProperty("file_url")]
        public string FileURL { get; set; }

        /// <summary>
        /// Gets or sets the URL to POST the file to.
        /// </summary>
        /// <value>
        /// The URL to POST the file to.
        /// </value>
        [PushBulletProperty("upload_url")]
        public string UploadURL { get; set; }
    }

    /// <summary>
    /// Represent a PushBullet's pushes list
    /// </summary>
    public class PushesList
    {
        /// <summary>
        /// Gets or sets the pushes.
        /// </summary>
        /// <value>
        /// The pushes.
        /// </value>
        [PushBulletProperty("pushes")]
        public Push[] Pushes { get; set; }
    }

    /// <summary>
    /// Represent the type of a push
    /// </summary>
    public enum PushType
    {
        /// <summary>
        /// Simple push with message
        /// </summary>
        Note,
        /// <summary>
        /// Push attached to file
        /// </summary>
        File,
        /// <summary>
        /// Push that contains link
        /// </summary>
        Link
    }

    /// <summary>
    /// Represent the direction the push was sent in
    /// </summary>
    public enum PushDirection
    {
        /// <summary>
        /// The push was sent from myself
        /// </summary>
        Self,
        /// <summary>
        /// The push was sent from Outgoing
        /// </summary>
        Outgoing,
        /// <summary>
        /// The push was sent from Incoming
        /// </summary>
        Incoming
    }

    /// <summary>
    /// Represent the target the push was sent to
    /// </summary>
    public enum PushTargetType
    {
        /// <summary>
        /// Send the push to a specific device.
        /// </summary>
        Device,
        /// <summary>
        /// Send the push to this email address. If that email address is associated with a Pushbullet user, we will send it directly to that user, otherwise we will fallback to sending an email to the email address (this will also happen if a user exists but has no devices registered).
        /// </summary>
        Email,
        /// <summary>
        /// Send the push to all subscribers to your channel that has this tag.
        /// </summary>
        Channel,
        /// <summary>
        /// Send the push to all users who have granted access to your OAuth client that has this iden.
        /// </summary>
        Client,
    }
}

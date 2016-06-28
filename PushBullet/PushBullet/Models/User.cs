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
    /// Represent a PushBullet's user
    /// </summary>
    public class User : PushBulletBaseObject
    {
        /// <summary>
        /// Gets or sets the Email address.
        /// </summary>
        /// <value>
        /// The Email address.
        /// </value>
        [PushBulletProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Canonical email address.
        /// </summary>
        /// <value>
        /// The Canonical email address.
        /// </value>
        [PushBulletProperty("email_normalized")]
        public string EmailNormalized { get; set; }

        /// <summary>
        /// Gets or sets the Full name if available.
        /// </summary>
        /// <value>
        /// The Full name if available.
        /// </value>
        [PushBulletProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL for image of user or placeholder imageL.
        /// </summary>
        /// <value>
        /// The URL for image of user or placeholder image.
        /// </value>
        [PushBulletProperty("image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the Maximum upload size in bytes.
        /// </summary>
        /// <value>
        /// The Maximum upload size in bytes.
        /// </value>
        [PushBulletProperty("max_upload_size")]
        public int MaxUploadSize { get; set; }

        /// <summary>
        /// Gets or sets the Number of users referred by this user.
        /// </summary>
        /// <value>
        /// The Number of users referred by this user.
        /// </value>
        [PushBulletProperty("referred_count")]
        public int ReferredCount { get; set; }

        /// <summary>
        /// Gets or sets the User iden for the user that referred the current user, if set.
        /// </summary>
        /// <value>
        /// The User iden for the user that referred the current user, if set.
        /// </value>
        [PushBulletProperty("referrer_iden")]
        public string ReferrerId { get; set; }
    }    
}

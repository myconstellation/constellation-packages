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
    using System;

    /// <summary>
    /// Represent a PushBullet deletable base object
    /// </summary>
    public class PushBulletDeletableBaseObject : PushBulletBaseObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether this item is active (not delete).
        /// </summary>
        /// <value>
        ///   <c>false</c> if the item has been deleted
        /// </value>
        [PushBulletProperty("active")]
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Represent a PushBullet base object
    /// </summary>
    public class PushBulletBaseObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for this item.
        /// </summary>
        /// <value>
        /// The unique identifier for this item.
        /// </value>
        [PushBulletProperty("iden")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the creation time in floating point seconds (unix timestamp).
        /// </summary>
        /// <value>
        /// The creation time in floating point seconds (unix timestamp).
        /// </value>
        [PushBulletProperty("created")]
        public float Created { get; set; }

        /// <summary>
        ///  Gets the creation date.
        /// </summary>
        /// <value>
        ///  The creation date.
        /// </value>
        public DateTime CreatedDate { get { return this.Created.ToDateTime(); } }

        /// <summary>
        /// Gets or sets the last modified time in floating point seconds (unix timestamp).
        /// </summary>
        /// <value>
        /// The last modified time in floating point seconds (unix timestamp).
        /// </value>
        [PushBulletProperty("modified")]
        public float Modified { get; set; }

        /// <summary>
        ///  Gets the last modified date.
        /// </summary>
        /// <value>
        ///  The last modified date.
        /// </value>
        public DateTime ModifiedDate { get { return this.Modified.ToDateTime(); } }
    }
}

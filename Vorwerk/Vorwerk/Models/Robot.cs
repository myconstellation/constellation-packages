/*
 *	 Vorwerk connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2020 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace Vorwerk.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a VR Robot
    /// </summary>
    public partial class Robot
    {
        [VorwerkProperty("serial")]
        public string Serial { get; set; }

        [VorwerkProperty("prefix")]
        public string Prefix { get; set; }

        [VorwerkProperty("name")]
        public string Name { get; set; }

        [VorwerkProperty("model")]
        public string Model { get; set; }

        [VorwerkProperty("firmware")]
        public string Firmware { get; set; }

        [VorwerkProperty("timezone")]
        public string Timezone { get; set; }

        [VorwerkProperty("secret_key")]
        public string SecretKey { get; set; }

        [VorwerkProperty("purchased_at")]
        public DateTimeOffset? PurchasedAt { get; set; }

        [VorwerkProperty("linked_at")]
        public DateTimeOffset LinkedAt { get; set; }

        [VorwerkProperty("nucleo_url")]
        public Uri NucleoUrl { get; set; }

        [VorwerkProperty("traits")]
        public List<object> Traits { get; set; }

        [VorwerkProperty("proof_of_purchase_url")]
        public string ProofOfPurchaseUrl { get; set; }

        [VorwerkProperty("proof_of_purchase_url_valid_for_seconds")]
        public int ProofOfPurchaseUrlValidForSeconds { get; set; }

        [VorwerkProperty("proof_of_purchase_generated_at")]
        public DateTimeOffset? ProofOfPurchaseGeneratedAt { get; set; }

        [VorwerkProperty("mac_address")]
        public string MacAddress { get; set; }

        [VorwerkProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [VorwerkProperty("latest_exploration_map_id")]
        public int? LatestExplorationMapId { get; set; }

        [VorwerkProperty("persistent_maps")]
        public List<object> PersistentMaps { get; set; }
    }
}


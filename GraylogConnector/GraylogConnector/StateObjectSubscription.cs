/*
 *	 Graylog connector for Constellation
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

namespace GraylogConnector
{
    using Constellation;
    using Constellation.Package;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class StateObjectSubscription
    {
        public string Sentinel { get; set; }
        public string Package { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public StateObjectSubscription(Action<Dictionary<string, object>> sendData, Subscription subscription)
            : this(subscription.Sentinel, subscription.Package, subscription.Name, subscription.Type)
        {
            this.RegisterSubscription(subscription,
                (stateObject) =>
                {
                    // Convert SO as GELF
                    Dictionary<string, object> data = this.ConvertStateObjectToGELF(stateObject);
                    // Send data
                    sendData(data);
                });
            PackageHost.WriteInfo("Subscription to '{0}/{1}/{2}/{3}' done.", subscription.Sentinel, subscription.Package, subscription.Name, subscription.Type);
        }

        public StateObjectSubscription(string sentinel = "*", string package = "*", string name = "*", string type = "*")
        {
            this.Sentinel = sentinel;
            this.Package = package;
            this.Name = name;
            this.Type = type;
        }

        protected void RegisterSubscription(Subscription subscription, Action<StateObject> onStateObjectUpdate, Action onInitialized = null)
        {
            // On StateObject update
            PackageHost.StateObjectUpdated += (s, e) =>
            {
                // Check subscription
                if ((e.StateObject.SentinelName == subscription.Sentinel || subscription.Sentinel == "*") &&
                   (e.StateObject.PackageName == subscription.Package || subscription.Package == "*") &&
                   (e.StateObject.Name == subscription.Name || subscription.Name == "*") &&
                   (e.StateObject.Type == subscription.Type || subscription.Type == "*"))
                {
                    // Check if the SO isn't exclude for this subscription
                    if (subscription.Exclusions != null &&
                        subscription.Exclusions.Count > 0 &&
                        subscription.Exclusions
                            .OfType<ExclusionElement>()
                            .Any(exclusion =>
                                (!string.IsNullOrEmpty(exclusion.Sentinel) && exclusion.Sentinel.Equals(e.StateObject.SentinelName, StringComparison.InvariantCultureIgnoreCase)) ||
                                (!string.IsNullOrEmpty(exclusion.Package) && exclusion.Package.Equals(e.StateObject.PackageName, StringComparison.InvariantCultureIgnoreCase)) ||
                                (!string.IsNullOrEmpty(exclusion.Name) && exclusion.Name.Equals(e.StateObject.Name, StringComparison.InvariantCultureIgnoreCase)) ||
                                (!string.IsNullOrEmpty(exclusion.Type) && exclusion.Type.Equals(e.StateObject.Type, StringComparison.InvariantCultureIgnoreCase))))
                    {
                        // SO exclude !
                        return;
                    }
                    else
                    {
                        // Otherwise execute action
                        onStateObjectUpdate(e.StateObject);
                    }
                }
            };
            // Subscribe to StateObject
            PackageHost.SubscribeStateObjects(subscription.Sentinel, subscription.Package, subscription.Name, subscription.Type);
            // Is init !
            if (onInitialized != null)
            {
                onInitialized();
            }
        }

        protected Dictionary<string, object> ConvertStateObjectToGELF(StateObject stateObject)
        {
            // Prepare StateObject's data
            Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "host", stateObject.SentinelName },
                        { "timestamp", Program.GetUnixTime(stateObject.LastUpdate) },
                        { "short_message", string.Format("{0}/{1}/{2} is updated", stateObject.SentinelName, stateObject.PackageName, stateObject.Name) },
                        { "package.name", stateObject.PackageName},
                        { "stateobject.name", stateObject.Name},
                        { "stateobject.type", stateObject.Type}
                    };
            // Expand metadatas
            if (stateObject.Metadatas != null)
            {
                foreach (var metadata in stateObject.Metadatas)
                {
                    data.Add(string.Concat(stateObject.PackageName, ".metadata.", metadata.Key), metadata.Value);
                }
            }
            // Expand value's properties  
            this.ExpandProperties(stateObject.PackageName, stateObject.Value, data);
            // Return GELF data
            return data;
        }

        protected void ExpandProperties(string rootKey, object input, Dictionary<string, object> data, string parentKey = "")
        {
            var jObject = input as JObject;
            // It's a JSON object ?
            if (jObject != null && jObject.HasValues)
            {
                // Foreach properties ...
                foreach (JProperty item in jObject.Properties())
                {
                    if (item.Value.HasValues)
                    {
                        // Expand sub-properties
                        this.ExpandProperties(rootKey, item.Value, data, string.Concat(parentKey, ".", item.Name));
                    }
                    else
                    {
                        string key = string.Concat(rootKey, parentKey, ".", item.Name);
                        if (item.Value is JValue)
                        {
                            // Add the property value
                            data.Add(key, this.FormatValue(((JValue)item.Value).Value));
                        }
                        else
                        {
                            // Format the value as String (including JArray !)
                            data.Add(key, item.Value.ToString());
                        }
                    }
                }
            }
            else
            {
                // It's a value
                data.Add(string.Concat(rootKey, ".value", parentKey), this.FormatValue(input));
            }
        }

        protected object FormatValue(object value)
        {
            // GELF doesn't support the boolean value 
            if (value is bool)
            {
                // Convert to bit !
                return ((bool)value) ? Program.TRUE : Program.FALSE;
            }
            return value;
        }
    }
}

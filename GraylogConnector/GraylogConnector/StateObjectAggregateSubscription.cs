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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class StateObjectAggregateSubscription : StateObjectSubscription
    {
        private const int DEFAULT_INTERVAL = 30; // seconds

        private object syncLock = new object();
        private HashSet<string> propertyToIncludeInfo = new HashSet<string>();
        private Dictionary<string, StateObjectAggregationData> aggregations = new Dictionary<string, StateObjectAggregationData>();

        public bool IsRecording { get; set; }

        public StateObjectAggregateSubscription(Action<Dictionary<string, object>> sendData, Subscription subscription)
            : base(subscription.Sentinel, subscription.Package, subscription.Name, subscription.Type)
        {
            if (subscription.Aggregation.Interval == 0)
            {
                subscription.Aggregation.Interval = DEFAULT_INTERVAL;
            }
            this.IsRecording = true;

            // Prepare aggregate value list
            foreach (AggregatePropertyElement aggregateProperty in subscription.Aggregation)
            {
                if (aggregateProperty.IncludeAggregateInfo)
                {
                    this.propertyToIncludeInfo.Add(aggregateProperty.PropertyName);
                }
            }

            // Register subscription
            this.RegisterSubscription(subscription,
                (stateObject) =>
                {
                    if (this.IsRecording)
                    {
                        try
                        {
                            lock (this.syncLock)
                            {
                                string soKey = string.Concat(stateObject.SentinelName, stateObject.PackageName, stateObject.Name);
                                if (!this.aggregations.ContainsKey(soKey))
                                {
                                    this.aggregations.Add(soKey, new StateObjectAggregationData(subscription));
                                }
                                this.aggregations[soKey].AddStateObject(this, stateObject);
                            }
                        }
                        catch (Exception ex)
                        {
                            PackageHost.WriteError("Error while updating '{0}/{1} {2}' : {3}", stateObject.SentinelName, stateObject.PackageName, stateObject.Name, ex.ToString());
                            this.IsRecording = false;
                        }
                    }
                });

            // Background task
            Task.Factory.StartNew(() =>
            {
                int nbSeconds = 0;
                while (PackageHost.IsRunning)
                {
                    if (nbSeconds == 0)
                    {
                        try
                        {
                            lock (syncLock)
                            {
                                foreach (var data in this.aggregations)
                                {
                                    data.Value.SendAggregateData(this.Package, this.propertyToIncludeInfo, sendData);                                    
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            PackageHost.WriteError("Error while sending '{0}/{1}/{2}/{3}' : {4}", subscription.Sentinel, subscription.Package, subscription.Name, subscription.Type, ex.ToString());
                        }
                    }
                    if (++nbSeconds == subscription.Aggregation.Interval)
                    {
                        nbSeconds = 0;
                    }
                    Thread.Sleep(1000);
                }
            });

            PackageHost.WriteInfo("Aggregate subscription to '{0}/{1}/{2}/{3}' on key(s) '{5}' every {4} second(s) done.", subscription.Sentinel, subscription.Package, subscription.Name, subscription.Type, subscription.Aggregation.Interval, string.Join(",", subscription.Aggregation.OfType<AggregatePropertyElement>().Select(ap => ap.PropertyName)));
        }

        private class StateObjectAggregationData
        {
            public DateTime FirstValueDate { get; set; }
            public Dictionary<string, List<double>> Values { get; set; }
            public Dictionary<string, object> GELFData { get; set; }

            public StateObjectAggregationData(Subscription subscription)
            {
                this.GELFData = null;
                this.FirstValueDate = DateTime.Now;
                this.Values = new Dictionary<string, List<double>>();
                foreach (AggregatePropertyElement aggregateProperty in subscription.Aggregation)
                {
                    this.Values.Add(aggregateProperty.PropertyName, new List<double>());
                }
            }

            public void AddStateObject(StateObjectAggregateSubscription subscription, StateObject stateObject)
            {
                if (this.GELFData == null)
                {
                    this.FirstValueDate = DateTime.Now;
                    this.GELFData = subscription.ConvertStateObjectToGELF(stateObject);
                    foreach (var key in this.Values.Keys)
                    {
                        this.Values[key].Clear();
                        this.Values[key].Add(Convert.ToDouble(this.GELFData[stateObject.PackageName + "." + key]));
                    }
                }
                else
                {
                    foreach (var key in this.Values.Keys)
                    {
                        var so = subscription.ConvertStateObjectToGELF(stateObject);
                        this.Values[key].Add(Convert.ToDouble(so[stateObject.PackageName + "." + key]));
                    }
                }
            }

            public void SendAggregateData(string package, HashSet<string> propertyToIncludeInfo, Action<Dictionary<string, object>> sendData)
            {
                if (this.GELFData != null)
                {
                    // Process the aggregate properties
                    foreach (var key in this.Values.Keys)
                    {
                        string dataKey =  package + "." + key;
                        // Replace by value average
                        this.GELFData[dataKey] = this.Values[key].Average();
                        // Add aggregate info
                        if (propertyToIncludeInfo.Contains(key))
                        {
                            this.GELFData.Add(dataKey + ".aggregate.first", this.Values[key].First());
                            this.GELFData.Add(dataKey + ".aggregate.last", this.Values[key].Last());
                            this.GELFData.Add(dataKey + ".aggregate.min", this.Values[key].Min());
                            this.GELFData.Add(dataKey + ".aggregate.max", this.Values[key].Max());
                            this.GELFData.Add(dataKey + ".aggregate.count", this.Values[key].Count);
                        }
                    }
                    if (propertyToIncludeInfo.Count > 0)
                    {
                        this.GELFData.Add(package + ".aggregate.startDate", Program.GetUnixTime(this.FirstValueDate));
                        this.GELFData.Add(package + ".aggregate.endDate", Program.GetUnixTime(DateTime.Now));
                    }

                    // Send data
                    sendData(this.GELFData);
                    this.GELFData = null;
                }
            }
        }
    }
}

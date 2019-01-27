/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016-2019 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace ZoneMinder
{
    /// <summary>
    /// ZoneMinder event search criterias
    /// </summary>
    public class EventSearchCriterias
    {
        public enum Criteria { EventId, MonitorId, StorageId, Name, Cause, StartDate, EndDate, Length, AlarmFrames, Width, Height, TotScore, AvgScore, MaxScore, Archived, Emailed, Notes, DiskSpace, Locked }
        public enum ConditionOperator { Equal, GreaterThan, GreaterOrEqual, LessThan, LessOrEqual /*, NotEqual*/}

        /// <summary>
        /// The field to compare.
        /// </summary>
        public Criteria Field { get; set; }
        /// <summary>
        /// The condition
        /// </summary>
        public ConditionOperator Condition { get; set; }
        /// <summary>
        ///The value compared to the field
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Field}{ConditionOperatorAsString()}{Value}";
        }

        /// <summary>
        /// Get the condition operator as string
        /// </summary>
        public string ConditionOperatorAsString()
        {
            switch (this.Condition)
            {
                //case ConditionOperator.NotEqual: // not implemented in ZM !
                //    return ":";
                case ConditionOperator.GreaterOrEqual:
                    return ">=:";
                case ConditionOperator.GreaterThan:
                    return ">:";
                case ConditionOperator.LessOrEqual:
                    return "<:";
                case ConditionOperator.LessThan:
                    return "<=:";
                case ConditionOperator.Equal:
                default:
                    return ":";
            }
        }
    }
}
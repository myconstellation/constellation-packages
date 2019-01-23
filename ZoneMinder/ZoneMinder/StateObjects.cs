/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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
    using Constellation.Package;

    [StateObject]
    public class Monitor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public MonitorFunction Function { get; set; }
        public bool Enabled { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal MaxFPS { get; set; }
        public string StreamPath { get; set; }
        public decimal SpaceUsed { get; set; }
        public AlarmState State { get; set; }
        public decimal FrameRate { get; set; }
        public int MinEventId { get; set; }
        public int MaxEventId { get; set; }
        public int TotalEvents { get; set; }
    }

    [StateObject]
    public class Host
    {
        public string URI { get; set; }
        public string Version { get; set; }
        public string APIVersion { get; set; }
        public double LoadAverageLastMinute { get; set; }
        public double LoadAverageLastFiveMinutes { get; set; }
        public double LoadAverageLastFifteenMinutes { get; set; }
        public bool DaemonStarted { get; set; }
        public decimal SpaceUsed { get; set; }
    }
}

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
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// ZoneMinder event
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The Event ID
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// The Monitor ID
        /// </summary>
        public int MonitorId { get; set; }
        /// <summary>
        /// The Storage ID
        /// </summary>
        public int StorageId { get; set; }
        /// <summary>
        /// The Event's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Event's cause
        /// </summary>
        public string Cause { get; set; }
        /// <summary>
        /// The Event's start datetime
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// The Event's datetime
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// The Event's width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The Event's height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The Event's lenght
        /// </summary>
        public decimal Length { get; set; }
        /// <summary>
        /// Number of frames for this event
        /// </summary>
        public int Frames { get; set; }
        /// <summary>
        /// Number of frames in alarm for this event
        /// </summary>
        public int AlarmFrames { get; set; }
        /// <summary>
        /// Total score for this event
        /// </summary>
        public int TotalScore { get; set; }
        /// <summary>
        /// Average score for this event
        /// </summary>
        public int AverageScore { get; set; }
        /// <summary>
        /// Maximum score for this event
        /// </summary>
        public int MaxScore { get; set; }
        /// <summary>
        /// The Event's notes
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Disk space used for this event record
        /// </summary>
        public long DiskSpace { get; set; }
        /// <summary>
        /// Is the event archived
        /// </summary>
        public bool Archived { get; set; }
        /// <summary>
        /// Is the event emailed
        /// </summary>
        public bool Emailed { get; set; }
        /// <summary>
        /// Is the event locked
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// Is the event terminated
        /// </summary>
        public bool Terminated { get; set; }
        /// <summary>
        /// The event's frames.
        /// </summary>
        public List<EventFrame> EventFrames { get; set; }

        /// <summary>
        /// Creates event from JSON.
        /// </summary>
        /// <param name="zmEvent">The ZM event.</param>
        /// <returns>Event</returns>
        public static Event CreateFromJSON(dynamic zmEvent)
        {
            var @event = new Event()
            {
                EventId = int.Parse(zmEvent.Event.Id.Value),
                Name = zmEvent.Event.Name.Value,
                MonitorId = int.Parse(zmEvent.Event.MonitorId.Value),
                StorageId = zmEvent.Event.StorageId == null ? -1 : int.Parse(zmEvent.Event.StorageId.Value),
                Cause = zmEvent.Event.Cause.Value,
                Notes = zmEvent.Event.Notes.Value,
                Terminated = zmEvent.Event.EndTime != null,
                StartTime = DateTime.ParseExact(zmEvent.Event.StartTime.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-us")),
                EndTime = zmEvent.Event.EndTime == null ? DateTime.MinValue : DateTime.ParseExact(zmEvent.Event.EndTime.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-us")), // null
                Length = decimal.Parse(zmEvent.Event.Length.Value, NumberStyles.Float, CultureInfo.InvariantCulture),
                Width = int.Parse(zmEvent.Event.Width.Value),
                Height = int.Parse(zmEvent.Event.Height.Value),
                Frames = zmEvent.Event.Frames == null ? 0 : int.Parse(zmEvent.Event.Frames.Value),
                AlarmFrames = zmEvent.Event.AlarmFrames == null ? 0 : int.Parse(zmEvent.Event.AlarmFrames.Value),
                AverageScore = int.Parse(zmEvent.Event.AvgScore.Value),
                DiskSpace = zmEvent.Event.DiskSpace == null ? -1 : long.Parse(zmEvent.Event.DiskSpace.Value),
                Archived = zmEvent.Event.Archived.Value == "1",
                Emailed = zmEvent.Event.Emailed.Value == "1",
                Locked = zmEvent.Event.Locked == null ? false : (zmEvent.Event.Locked.Value is bool ? (bool)zmEvent.Event.Locked.Value : zmEvent.Event.Locked.Value == "1"),
                MaxScore = int.Parse(zmEvent.Event.MaxScore.Value),
                TotalScore = int.Parse(zmEvent.Event.TotScore.Value),
            };
            if (zmEvent.Frame != null)
            {
                @event.EventFrames = new List<EventFrame>();
                foreach (var frame in zmEvent.Frame)
                {
                    @event.EventFrames.Add(new EventFrame()
                    {
                        Id = int.Parse(frame.Id.Value),
                        EventId = int.Parse(frame.EventId.Value),
                        FrameId = int.Parse(frame.FrameId.Value),
                        Type = frame.Type.Value,
                        TimeStamp = DateTime.ParseExact(frame.TimeStamp.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-us")),
                        Delta = decimal.Parse(frame.Delta.Value, NumberStyles.Float, CultureInfo.InvariantCulture),
                        Score = int.Parse(frame.FrameId.Value),
                    });
                }
            }
            return @event;
        }
    }

    /// <summary>
    /// Represent an event's frame
    /// </summary>
    public class EventFrame
    {
        /// <summary>
        /// The unique frame identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Event Id.
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// The frame id relative to this event.
        /// </summary>
        public int FrameId { get; set; }
        /// <summary>
        /// The frame's type (Normal or Alarm).
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The TimeStamp for this frame.
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// The delta in second from the first frame.
        /// </summary>
        public decimal Delta { get; set; }
        /// <summary>
        /// The score of this frame.
        /// </summary>
        public int Score { get; set; }
    }
}
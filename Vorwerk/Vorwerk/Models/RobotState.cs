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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a state of the robot
    /// </summary>

    public partial class RobotState
    {
        [VorwerkProperty("version")]
        public int Version { get; set; }

        [VorwerkProperty("reqId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int RequestId { get; set; }

        [VorwerkProperty("result")]
        public string Result { get; set; }

        [VorwerkProperty("error")]
        public ErrorCode Error { get; set; }

        [VorwerkProperty("data")]
        public object Data { get; set; }

        [VorwerkProperty("alert")]
        public AlertCode? Alert { get; set; }

        [VorwerkProperty("state")]
        public StateCode State { get; set; }

        [VorwerkProperty("action")]
        public ActionCode Action { get; set; }

        [VorwerkProperty("cleaning")]
        public Cleaning Cleaning { get; set; }

        [VorwerkProperty("details")]
        public Details Details { get; set; }

        [VorwerkProperty("availableCommands")]
        public AvailableCommands AvailableCommands { get; set; }

        [VorwerkProperty("availableServices")]
        public Dictionary<string, string> AvailableServices { get; set; }

        [VorwerkProperty("meta")]
        public Meta Meta { get; set; }

        /// <summary>
        /// Create RobotState from the json
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static RobotState FromJson(string json) => JsonConvert.DeserializeObject<RobotState>(json, VorwerkContractResolver.Settings);

        [JsonConverter(typeof(StringEnumConverter))]
        public enum StateCode
        {
            NotAvailable = 0,
            Idle = 1,
            Busy = 2,
            Paused = 3,
            Error = 4
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ActionCode
        {
            NoAction = 0,
            HouseCleaning = 1,
            SpotCleaning = 2,
            ManualCleaning = 3,
            Docking = 4,
            UserMenuActive = 5,
            CleaningSuspended = 6,
            Updating = 7,
            CopyLogs = 8,
            DeterminePosition = 9,
            IECTest = 10,
            MapCleaning = 11,
            ExploringMap = 12,
            AcquiringPersistentMapIDs = 13,
            CreatingMap = 14,
            SuspendedExploration = 15
        }

        [JsonConverter(typeof(CustomEnumJsonConverter<AlertCode>))]
        public enum AlertCode
        {
            Unknown,
            [VorwerkIds("ui_alert_dust_bin_full", "dustbin_full")]
            DustContainerFull,
            [VorwerkIds("ui_alert_recovering_location")]
            ReturningToStart,
            [VorwerkIds("maint_brush_change")]
            ChangeBrush,
            [VorwerkIds("maint_filter_change")]
            ChangeFilter,
            [VorwerkIds("clean_completed_to_start")]
            CleaningCompleted,
            [VorwerkIds("nav_floorplan_not_created")]
            FloorplanNotFound,
        }

        [JsonConverter(typeof(CustomEnumJsonConverter<ErrorCode>))]
        public enum ErrorCode
        {
            Unknown,
            [VorwerkIds("ui_alert_invalid")]
            NoMessage,
            [VorwerkIds("ui_alert_busy_charging")]
            BusyCharging,
            [VorwerkIds("ui_error_dust_bin_full")]
            DustContainerFull,
            [VorwerkIds("ui_error_brush_stuck")]
            BrushStuck,
            [VorwerkIds("ui_error_bumper_stuck")]
            BumperStuck,
            [VorwerkIds("ui_error_picked_up")]
            PickedUp,
            [VorwerkIds("ui_error_stuck")]
            Stuck,
            [VorwerkIds("ui_error_dust_bin_emptied")]
            DustContainerEmptied,
            [VorwerkIds("ui_error_dust_bin_missing")]
            DustContainerMissing,
            [VorwerkIds("ui_error_navigation_falling")]
            NavigationFalling,
            [VorwerkIds("ui_error_navigation_noprogress")]
            NavigationNoProgress,
            [VorwerkIds("ui_error_navigation_nomotioncommands")]
            NavigationNoMotionCommands,
            [VorwerkIds("ui_error_navigation_undockingfailed")]
            NavigationUndockingFailed
        }
    }

    public partial class AvailableCommands
    {
        [VorwerkProperty("start")]
        public bool Start { get; set; }

        [VorwerkProperty("stop")]
        public bool Stop { get; set; }

        [VorwerkProperty("pause")]
        public bool Pause { get; set; }

        [VorwerkProperty("resume")]
        public bool Resume { get; set; }

        [VorwerkProperty("goToBase")]
        public bool GoToBase { get; set; }
    }

    public partial class Cleaning
    {
        [VorwerkProperty("category")]
        public CleaningCategory Category { get; set; }

        [VorwerkProperty("mode")]
        public CleaningMode Mode { get; set; }

        [VorwerkProperty("modifier")]
        public CleaningModifier Modifier { get; set; }

        [VorwerkProperty("navigationMode")]
        public CleaningNavigation NavigationMode { get; set; }

        [VorwerkProperty("mapId")]
        public int MapId { get; set; }

        [VorwerkProperty("spotWidth")]
        public int SpotWidth { get; set; }

        [VorwerkProperty("spotHeight")]
        public int SpotHeight { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum CleaningNavigation
        {
            Unknown = 0,
            Normal = 1,
            ExtraCare = 2,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum CleaningMode
        {
            Normal = 0,
            Eco = 1,
            Turbo = 2,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum CleaningModifier
        {
            Unknown = 0,
            Normal = 1,
            Double = 2,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum CleaningCategory
        {
            Unkwown = 0,
            Manual = 1,
            House = 2,
            Spot = 3,
            Map = 4,
        }
    }

    public partial class Details
    {
        [VorwerkProperty("isCharging")]
        public bool IsCharging { get; set; }

        [VorwerkProperty("isDocked")]
        public bool IsDocked { get; set; }

        [VorwerkProperty("isScheduleEnabled")]
        public bool IsScheduleEnabled { get; set; }

        [VorwerkProperty("dockHasBeenSeen")]
        public bool DockHasBeenSeen { get; set; }

        [VorwerkProperty("charge")]
        public int Charge { get; set; }
    }

    public partial class Meta
    {
        [VorwerkProperty("modelName")]
        public string ModelName { get; set; }

        [VorwerkProperty("firmware")]
        public string Firmware { get; set; }
    }
}


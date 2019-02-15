/*
 *	 PoolCop connector for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018-2019 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace PoolCop.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the PoolCop
    /// </summary>
    public partial class PoolCop
    {
        /// <summary>
        /// The current temperatures
        /// </summary>
        [PoolCopilotProperty("temperature")]
        public Temperature Temperature { get; set; }

        /// <summary>
        /// The pump pressure (in kPa)
        /// </summary>
        [PoolCopilotProperty("pressure")]
        public int Pressure { get; set; }

        /// <summary>
        /// The pH measured if installed 
        /// </summary>
        [PoolCopilotProperty("pH")]
        public double PH { get; set; }

        /// <summary>
        /// The ORP value if installed
        /// </summary>
        [PoolCopilotProperty("orp")]
        public int Orp { get; set; }

        /// <summary>
        /// The ioniser if installed
        /// </summary>
        [PoolCopilotProperty("ioniser")]
        public int Ioniser { get; set; }

        /// <summary>
        /// The battery voltage
        /// </summary>
        [PoolCopilotProperty("voltage")]
        public double Voltage { get; set; }

        /// <summary>
        /// The water level if installed
        /// </summary>
        [PoolCopilotProperty("waterlevel")]
        public WaterlevelState Waterlevel { get; set; }
        
        /// <summary>
        /// The PoolCop date
        /// </summary>
        [PoolCopilotProperty("date")]
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// The PoolCop configuration
        /// </summary>
        [PoolCopilotProperty("conf")]
        public Configuration Configuration { get; set; }

        /// <summary>
        /// The PoolCop equipments status
        /// </summary>
        [PoolCopilotProperty("status")]
        public Status Status { get; set; }

        /// <summary>
        /// The PoolCop auxiliaries
        /// </summary>
        [PoolCopilotProperty("aux")]
        public List<Auxiliary> Auxiliaries { get; set; }

        /// <summary>
        /// Filtration and Auxiliaries timers
        /// </summary>
        [PoolCopilotProperty("timers")]
        public Dictionary<string, Timer> Timers { get; set; }

        /// <summary>
        /// PoolCop Alerts
        /// </summary>
        [PoolCopilotProperty("alerts")]
        public List<Alert> Alerts { get; set; }

        /// <summary>
        /// PoolCop History
        /// </summary>
        [PoolCopilotProperty("history")]
        public History History { get; set; }

        /// <summary>
        /// PoolCop settings
        /// </summary>
        [PoolCopilotProperty("settings")]
        public Settings Settings { get; set; }

        /// <summary>
        /// Data related to the network configuration
        /// </summary>
        [PoolCopilotProperty("network")]
        public Network Network { get; set; }

        /// <summary>
        /// Links to the PoolCopilot Application
        /// </summary>
        [PoolCopilotProperty("links")]
        public Links Links { get; set; }
    }

    /// <summary>
    /// Auxiliaries configuration
    /// </summary>
    public partial class Auxiliary
    {
        /// <summary>
        /// The physical ID of Auxiliary
        /// </summary>
        [PoolCopilotProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The Aux is ON (true) or OFF (false)
        /// </summary>
        [PoolCopilotProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// The Auxiliary label
        /// </summary>
        [PoolCopilotProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Is the Auxiliary slaved to the filtration ?
        /// </summary>
        [PoolCopilotProperty("slave")]
        public bool Slave { get; set; }

        /// <summary>
        /// Represents if the auxiliary should work or not, from monday to sunday
        /// </summary>
        [PoolCopilotProperty("days")]
        public List<bool> Days { get; set; }

        /// <summary>
        /// Is the Auxiliary switchable (true) or locked (false) ? 
        /// </summary>
        [PoolCopilotProperty("switchable")]
        public bool Switchable { get; set; }
    }

    /// <summary>
    /// The equipment Installed or not
    /// </summary>
    public partial class Configuration
    {
        /// <summary>
        /// The PoolCop version
        /// </summary>
        [PoolCopilotProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Is the ORP Control installed or not
        /// </summary>
        [PoolCopilotProperty("orp")]
        public bool Orp { get; set; }

        /// <summary>
        /// Is the PH Control installed or not
        /// </summary>
        [PoolCopilotProperty("pH")]
        public bool PH { get; set; }

        /// <summary>
        /// Is the Water Level Control installed or not
        /// </summary>
        [PoolCopilotProperty("waterlevel")]
        public bool Waterlevel { get; set; }

        /// <summary>
        /// Is the Ioniser installed or not
        /// </summary>
        [PoolCopilotProperty("ioniser")]
        public bool Ioniser { get; set; }

        /// <summary>
        /// Is the AutoChlor installed or not
        /// </summary>
        [PoolCopilotProperty("autochlor")]
        public bool Autochlor { get; set; }

        /// <summary>
        /// Is the air temperature probe installed or not
        /// </summary>
        [PoolCopilotProperty("air")]
        public bool Air { get; set; }
    }

    /// <summary>
    /// The PoolCop history date
    /// </summary>
    public partial class History
    {
        /// <summary>
        /// The last backwash date
        /// </summary>
        [PoolCopilotProperty("backwash")]
        public DateTimeOffset BackwashDate { get; set; }

        /// <summary>
        /// The last refill date
        /// </summary>
        [PoolCopilotProperty("refill")]
        public DateTimeOffset RefillDate { get; set; }

        /// <summary>
        /// The last PH measure date
        /// </summary>
        [PoolCopilotProperty("ph_measure")]
        public DateTimeOffset PhMeasureDate { get; set; }
    }

    /// <summary>
    /// Links to the PoolCopilot Application
    /// </summary>
    public partial class Links
    {
        /// <summary>
        /// The Portfolio page
        /// </summary>
        [PoolCopilotProperty("portfolio")]
        public Link Portfolio { get; set; }

        /// <summary>
        /// The Panel page URL
        /// </summary>
        [PoolCopilotProperty("panel")]
        public Link Panel { get; set; }

        /// <summary>
        /// The PoolCop's dashboard page
        /// </summary>
        [PoolCopilotProperty("dashboard")]
        public Link Dashboard { get; set; }
    }

    /// <summary>
    /// Represent a HTTP link
    /// </summary>
    public partial class Link
    {
        /// <summary>
        /// The URL to the link
        /// </summary>
        [PoolCopilotProperty("href")]
        public Uri Href { get; set; }
    }

    /// <summary>
    /// The data related to the network configuration
    /// </summary>
    public partial class Network
    {
        /// <summary>
        /// The PoolCopilot MAC Address.
        /// </summary>
        [PoolCopilotProperty("mac_address")]
        public string MacAddress { get; set; }

        /// <summary>
        /// The PoolCopilot firmware version.
        /// </summary>
        [PoolCopilotProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// The DNS server used on the local network.
        /// </summary>
        [PoolCopilotProperty("dns")]
        public string Dns { get; set; }

        /// <summary>
        /// Is The PoolCop communicating now ?
        /// </summary>
        [PoolCopilotProperty("connected")]
        public bool Connected { get; set; }

        /// <summary>
        /// The latest data received by PoolCopilot Server
        /// </summary>
        [PoolCopilotProperty("connected_at")]
        public DateTimeOffset ConnectedAt { get; set; }

        /// <summary>
        /// The IP Address used by PoolCop to access PoolCopilot
        /// </summary>
        [PoolCopilotProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// The IP Address calling the API
        /// </summary>
        [PoolCopilotProperty("remote")]
        public string Remote { get; set; }
    }

    /// <summary>
    /// The PoolCop status
    /// </summary>
    public partial class Status
    {
        /// <summary>
        /// The pump is ON (true) or OFF (false)
        /// </summary>
        [PoolCopilotProperty("pump")]
        public bool Pump { get; set; }

        /// <summary>
        /// The current valve position
        /// </summary>
        [PoolCopilotProperty("valveposition")]
        public ValvePosition ValvePosition { get; set; }

        /// <summary>
        /// The pump speed in case of multispeed pump
        /// </summary>
        [PoolCopilotProperty("pumpspeed")]
        public int PumpSpeed { get; set; }

        /// <summary>
        /// The current running status
        /// </summary>
        [PoolCopilotProperty("poolcop")]
        public RunningStatus RunningStatus { get; set; }

        /// <summary>
        /// The current water valve position
        /// </summary>
        [PoolCopilotProperty("watervalve")]
        public WaterValvePosition WaterValve { get; set; }

        /// <summary>
        /// Is the PH control in progress
        /// </summary>
        [PoolCopilotProperty("ph_control")]
        public bool PhControl { get; set; }

        /// <summary>
        /// Is the ORP control in progress
        /// </summary>
        [PoolCopilotProperty("orp_control")]
        public bool OrpControl { get; set; }

        /// <summary>
        /// Is the Autochlor control in progress
        /// </summary>
        [PoolCopilotProperty("autochlor")]
        public bool Autochlor { get; set; }

        /// <summary>
        /// Is the Ioniser control in progress
        /// </summary>
        [PoolCopilotProperty("ioniser")]
        public bool Ioniser { get; set; }

        /// <summary>
        /// The Aux1 is ON or OFF
        /// </summary>
        [PoolCopilotProperty("aux1")]
        public bool Aux1 { get; set; }

        /// <summary>
        /// The Aux2 is ON or OFF
        /// </summary>
        [PoolCopilotProperty("aux2")]
        public bool Aux2 { get; set; }

        /// <summary>
        /// The Aux3 is ON or OFF
        /// </summary>
        [PoolCopilotProperty("aux3")]
        public bool Aux3 { get; set; }

        /// <summary>
        /// The Aux4 is ON or OFF
        /// </summary>
        [PoolCopilotProperty("aux4")]
        public bool Aux4 { get; set; }

        /// <summary>
        /// The Aux5 is ON or OFF
        /// </summary>
        [PoolCopilotProperty("aux5")]
        public bool Aux5 { get; set; }

        /// <summary>
        /// The Aux6 is ON or OFF
        /// </summary>
        [PoolCopilotProperty("aux6")]
        public bool Aux6 { get; set; }

        /// <summary>
        /// If the filtration is on forced mode
        /// </summary>
        [PoolCopilotProperty("forced")]
        public Forced ForcedMode { get; set; }
    }

    /// <summary>
    /// Filtration forced mode
    /// </summary>
    public partial class Forced
    {
        /// <summary>
        /// The forced mode 24, 48 or 72H
        /// </summary>
        [PoolCopilotProperty("mode")]
        public ForcedPumpMode Mode { get; set; }

        /// <summary>
        /// The remaining hours for forced mode
        /// </summary>
        [PoolCopilotProperty("remaining_hours")]
        public int RemainingHours { get; set; }
    }

    /// <summary>
    /// The temperatures.
    /// </summary>
    public partial class Temperature
    {
        /// <summary>
        /// The Water temperature.
        /// </summary>
        [PoolCopilotProperty("water")]
        public double Water { get; set; }

        /// <summary>
        /// The Air temperature.
        /// </summary>
        [PoolCopilotProperty("air")]
        public double Air { get; set; }
    }

    /// <summary>
    /// PoolCop alert.
    /// </summary>
    public partial class Alert
    {
        /// <summary>
        /// The Alert code ID
        /// </summary>
        [PoolCopilotProperty("id")]
        public int Code { get; set; }

        /// <summary>
        /// The Alert description
        /// </summary>
        [PoolCopilotProperty("name")]
        public string Description { get; set; }
        
        /// <summary>
        /// The Alert date 
        /// </summary>
        [PoolCopilotProperty("date")]
        public DateTimeOffset Date { get; set; }
    }

    /// <summary>
    /// Represent a Timer
    /// </summary>
    public partial class Timer
    {
        /// <summary>
        /// Is the timer enabled
        /// </summary>
        [PoolCopilotProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Timer start date
        /// </summary>
        [PoolCopilotProperty("start")]
        public DateTimeOffset Start { get; set; }

        /// <summary>
        /// TTimer stop date
        /// </summary>
        [PoolCopilotProperty("stop")]
        public DateTimeOffset Stop { get; set; }
    }
}


/*
 *	 Paradox .NET library
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Sebastien Warin under one or more contributor
 *	 license agreements. Sebastien Warin licenses this file to you under
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

namespace Paradox.Events
{
    /// <summary>
    /// Non-Reportable event type
    /// </summary>
    public enum NonReportableEventType
    {
        TLMTrouble = 0,
        SmokeDetectorReset = 1,
        ArmWithNoEntryDelay = 2,
        ArmInStayMode = 3,
        ArmInAwayMode = 4,
        FullArmWhenInStayMode = 5,
        VoiceModuleAccess = 6,
        RemoteControlAccess = 7,
        PCFailToCommunicate = 8,
        Midnight = 9,
        NEwareUserLogin = 10,
        NEwareUserLogout = 11,
        UserInitiatedCallup = 12,
        ForceAnswer = 13,
        ForceHangup = 14
    }
}
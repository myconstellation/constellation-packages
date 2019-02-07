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

namespace Paradox
{
    /// <summary>
    /// Paradox events group
    /// </summary>
    public enum EventGroup
    {
        ZoneOK = 0,
        ZoneOpen = 1,
        ZoneTampered = 2,
        ZoneInFire = 3,
        NonReportableEvent = 4,
        UserCodeEnteredOnKeypad = 5,
        UserCardAccessOnDoor = 6,
        BypassProgrammingAccess = 7,
        TXDelayZoneAlarm = 8,
        ArmingWithMaster = 9,
        ArmingWithUserCode = 10,
        ArmingWithKeyswitch = 11,
        SpecialArming = 12,
        DisarmWithMaster = 13,
        DisarmWithUserCode = 14,
        DisarmWithKeyswitch = 15,
        DisarmAfterAlarmWithMaster = 16,
        DisarmAfterAlarmWithUserCode = 17,
        DisarmAfterAlarmWithKeyswitch = 18,
        AlarmCancelledWithMaster = 19,
        AlarmCancelledWithUserCode = 20,
        AlarmCancelledWithKeyswitch = 21,
        SpecialDisarmEvents = 22,
        ZoneBypassed = 23,
        ZoneInAlarm = 24,
        FireAlarm = 25,
        ZoneAlarmRestore = 26,
        FireAlarmRestore = 27,
        EarlyToDisarmByUser = 28,
        LateToDisarmByUser = 29,
        SpecialAlarm = 30,
        DuressAlarmByUser = 31,
        ZoneShutdown = 32,
        ZoneTamper = 33,
        ZoneTamperRestore = 34,
        SpecialTamper = 35,
        TroubleEvent = 36,
        TroubleRestore = 37,
        ModuleTrouble = 38,
        ModuleTroubleRestore = 39,
        FailToCommunicateOnTelephoneNumber = 40,
        LowBatteryOnZone = 41,
        ZoneSupervisionTrouble = 42,
        LowBatteryOnZoneRestored = 43,
        ZoneSupervisionTroubleRestored = 44,
        SpecialEvents = 45,
        EarlyToArmByUser = 46,
        LateToArmByUser = 47,
        UtilityKey = 48,
        RequestForExit = 49,
        AccessDenied = 50,
        DoorLeftOpenAlarm = 51,
        DoorForcedAlarm = 52,
        DoorLeftOpenRestore = 53,
        DoorForcedOpenRestore = 54,
        IntellizoneTriggered = 55,
        ZoneExcludedOnForceArming = 56,
        ZoneWentBackToArmStatus = 57,
        NewModuleAssignedOnCombus = 58,
        ModuleManuallyRemovedFromCombus = 59,
        //FutureUse = 60,
        //FutureUse = 61,
        AccessGrantedToUser = 62,
        AccessDeniedToUser = 63,
        Status1 = 64,
        Status2 = 65,
        Status3 = 66
    }
}
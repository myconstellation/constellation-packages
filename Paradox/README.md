# Paradox connector for Constellation

This package connects a Paradox security system to Constellation by using the PRT3 interface module.

### StateObjects
* A StateObject for each area loaded (type of [AreaInfo](Paradox/Models/AreaInfo.cs))
* A StateObject for each user loaded (type of [AreaInfo](Paradox/Models/UserInfo.cs))
* A StateObject for each zone loaded (type of [AreaInfo](Paradox/Models/ZoneInfo.cs))
* Status1 : the current Status1 of the system (type of [Status1EventType](Paradox.Core/System Events/Enums/Status1EventType.cs))
* Status2 : the current Status2 of the system (type of [Status2EventType](Paradox.Core/System Events/Enums/Status2EventType.cs))

### MessageCallbacks

The package expose four MessageCallbacks to allow you to arm and disarm the system from your applications or objects connected to your Constellation :

*  bool AreaArm([ArmingRequestData](Paradox/Models/ArmingRequestData.cs) request) : Arms the area and return the boolean result.
*  bool AreaDisarm([ArmingRequestData](Paradox/Models/ArmingRequestData.cs) request) : Arms the area and return the boolean result.
*  void RefreshArea([Area](Paradox.Core/Base Events/Enums/Area.cs) request) : Force to refresh the specified area status.
*  void RefreshAll() : Force to refresh all items.

Also the package sends messages named "AlarmEvent" to each event of the system. These messages are sent in the group with the name of your package instance (Paradox by default)

Each "AlarmEvent" message contains at least a "Type" property which is the following:

| Type                    | Others properties                                              | Description   |
| ------------------------| -------------------------------------------------------------- |---------------|
| ConnectionStateChanged  | State (bool)                                                   | When the PRT3 connection changed |
| InterfaceError          | Exception (string)                                             | When the communication with PRT3 raise an error  |
| Armed                   | Date (DateTime), Text (string), Status (Status1EventType)      | When the system is armed (the Status property contains the arming type: Full, Force, Instant, etc.)  |
| Alarm                   | Date (DateTime), Text (string), Status (Status1EventType)      | When the system is in alarm (the Status property contains the alarm type : Strobe, Silent, Fire, etc.) |
| Arming                  | Date (DateTime), Text (string), User (UserInfo)                | When a user arms the system  |
| Disarming               | Date (DateTime), Text (string), User (UserInfo)                | When a user disarms the system |
| UserCodeEnteredOnKeypad | Date (DateTime), Text (string), User (UserInfo)                | When a user enters his code on a keyboard  |
| SpecialArming           | Date (DateTime), Text (string), ArmingType (SpecialArmingType) | When the system is armed with a special type (auto arming, etc.)   |
| AccessDenied            | Date (DateTime), Text (string), User (UserInfo)                | When access is denied for a user |
| AccessGranted           | Date (DateTime), Text (string), User (UserInfo)                | When access is granted for a user  |
| AlarmCancelled          | Date (DateTime), Text (string), User (UserInfo)                | When an alarm is cancelled
| NonReportableEvent      | Date (DateTime), Text (string), Event (NonReportableEventType) | Non reportable event (AC or Battery failure, combus fault, etc.)  |
| ZoneInAlarm             | Date (DateTime), Text (string), Zone (ZoneInfo)                | When a zone is in alarm   |
| ZoneAlarmRestored       | Date (DateTime), Text (string), Zone (ZoneInfo)                | When a zone in alarm is restored   |
| ZoneChanged             | Date (DateTime), Text (string), Zone (ZoneInfo)                | When a zone status changed |

So as example, if you want to receive in real time all the Paradox events in a web page y using the Constellation Javascript API, just type :
```
constellation.registerMessageCallback("AlarmEvent", function (msg) {
    console.log("Paradox event : ", msg.Data);
});
constellation.subscribeMessages("Paradox");
```

### Installation

This package work on Linux (tested on amd64 and arm64) or Windows.

Declare the package in a Sentinel by specifiying the COM port to the Paradox PRT3 module (for example COM1 on Windows or /dev/ttyUSB0 on Linux) and the number of Areas, Users and Zones to load :
```xml
<package name="Paradox">
  <settings>
    <setting key="PortCom" value="COM4" />
    <setting key="numberofAreas" value="1" />
    <setting key="numberofZones" value="14" />
    <setting key="numberofUsers" value="5" />
  </settings>
</package>
```

License
----

Apache License
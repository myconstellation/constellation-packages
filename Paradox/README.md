# Paradox connector for Constellation

This package connects a Paradox security system to Constellation & Home Assistant by using the PRT3 interface module.

New in version 2.0 : supports HomeAssistant MQTT Alarm Panel integration

### StateObjects

The package publishes different StateObjects in real time :

* A StateObject for each area loaded (type of [AreaInfo](Paradox/Models/AreaInfo.cs))
* A StateObject for each user loaded (type of [UserInfo](Paradox/Models/UserInfo.cs))
* A StateObject for each zone loaded (type of [ZoneInfo](Paradox/Models/ZoneInfo.cs))
* Status1 : the current Status1 of the system (type of [Status1EventType](Paradox.Core/System%20Events/Enums/Status1EventType.cs))
* Status2 : the current Status2 of the system (type of [Status2EventType](Paradox.Core/System%20Events/Enums/Status2EventType.cs))

### MessageCallbacks

The package expose four MessageCallbacks to allow you to arm and disarm the system from your applications or objects connected to your Constellation :

*  bool AreaArm([ArmingRequestData](Paradox/Models/ArmingRequestData.cs) request) : Arms the area and return the boolean result.
*  bool AreaDisarm([ArmingRequestData](Paradox/Models/ArmingRequestData.cs) request) : Arms the area and return the boolean result.
*  void RefreshArea([Area](Paradox.Core/Base%20Events/Enums/Area.cs) request) : Force to refresh the specified area status.
*  void RefreshAll() : Force to refresh all items.

Also the package sends messages called "AlarmEvent" for each systeme event. These messages are sent in the group named with name of your package instance (Paradox by default)

Each "AlarmEvent" message contains at least a "Type" property and others properties depending to the event type :

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

For example, if you want to receive in real time all the Paradox events in a web page by using the Constellation Javascript API, just type :
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

### Home Assistant Alarm Panel integration

Add the MQTT integration, then provide your broker�s hostname (or IP address) and port and (if required) the username and password that Home Assistant should use :

1. Browse to your Home Assistant instance.
2. Go to Settings > Devices & Services.
3. In the bottom right corner, select the Add Integration button.
4. From the list, select MQTT.
5. Follow the instructions on screen to complete the setup.

Then add the "HomeAssistant" setting on your Paradox Constellation package :

```xml
<setting key="HomeAssistant">
    <content>
          {
            "Enable": true,
            "Mqtt":
            {
              "Server": "mqtt.demo.net",
              "Port": 1883,
              "Username": "demo",
              "Password": "demo"
            },
            "Zones":
            [
              {
                "Id": 1,
                "Label": "My Door",
                "Area": "Living room",
                "Manufacturer": "Becuwe",
                "Model": "IM9700",
                "Type": "door"
              },
   
              {
                "Id": 2,
                "Label": "Kitchen Motion",
                "Area": "Kitchen",
                "Manufacturer": "Paradox",
                "Model": "DG75",
                "Type": "motion"
              },
              {
                "Id": 4,
                "Label": "Kitchen Window",
                "Area": "Kitchen",
                "Manufacturer": "Becuwe",
                "Model": "IM9700",
                "Type": "window"
              },   
              {
                "Id": 8,
                "Label": "EVO192 protection",
                "Area": "Garage",
                "Manufacturer": "Paradox",
                "Model": "EVO192",
                "Type": "tamper"
              }
            ]
          }
    </content>
</setting>
```

See the `Paradox\HomeAssistant\HomeAssistantConfiguration.cs` file for more information about the schema.

License
----

Apache License
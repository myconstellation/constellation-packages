# Constellation Package for Xiaomi Smart Home

This package get the report from Xiaomi Smart Homme equipements (motion sensor, magnet sensor, temperature/humidity sensor)

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
	<package name="XiaomiSmartHome">
        <settings>
            <setting key="GatewayIP" value="xx.xx.xx.xx" />
            <setting key="HeartbeatLog" value="true" />
            <setting key="ReportLog" value="true" />
        </settings>
    </package>
```

### Settings
 - GatewayIP (string - required) : IP adress of the Xiaomi Gateway (for exemple : 192.168.0.10)
 - HeartbeatLog (Boolean - optional) : Print the heartbeat log to Constellation console
 - ReportLog (Boolean - optional) : Print the report log to Constellation console

 You can add settings with your equipement SID to give a name to the StateObject, for example :

```xml
	<package name="XiaomiSmartHome">
        <settings>
            <setting key="GatewayIP" value="xx.xx.xx.xx" />
            <setting key="HeartbeatLog" value="true" />
            <setting key="ReportLog" value="true" />
            <setting key="158d0001a2dda3" value="TempSalon" />
        </settings>
    </package>
```
 The StateObject for equipement with SID 158d0001a2dda3 will have TempSalon as name.             

License
----

Apache License

# Constellation Package for Xiaomi Smart Home

This package get the report from Xiaomi Smart Homme equipements.
Currently this package is only compatible with :
- MiJia Smart Home Gateway (3rd Generation Edition) : https://xiaomi.eu/store/xiaomi/mijia-smart-home/xiaomi-mijia-smart-home-gateway-3rd-generation-edition-ytc4003cn/
- MiJia Smart Human Body Movement Sensor : https://xiaomi.eu/store/xiaomi/mijia-smart-home/xiaomi-mijia-smart-human-body-movement-sensor-rtcgq01lm/
- MiJia Window/Door Magnetic Field Sensor : https://xiaomi.eu/store/xiaomi/mijia-smart-home/xiaomi-mijia-window-door-magnetic-field-sensor-ytc4005cn/
- MiJia Smart Temperature & Humidity Sensor : https://xiaomi.eu/store/xiaomi/mijia-smart-home/xiaomi-mijia-smart-temperature-and-humidity-sensor-wsdcgq01lm/

### Installation :


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

### MessageCallbacks

FindEquipementsList  : Get the equipements SID list link to the gateway
ReadEquipement : Get equipement values (require equipement SID)

License
----

Apache License
# Yeelight package for Constellation

This package allow you to control Xiaomi Yeelight smart-bulbs.

### MessageCallbacks
 - Discover() : Discover Yeelight devices through LAN
 - GetAllProps(string deviceOrGroupName) : Get all the available properties for a bulb.
 - GetProp(string deviceOrGroupName, PROPERTIES property) : Get a single property by his key.
 - GetProps(string deviceOrGroupName, PROPERTIES property) : Get multiple properties by their keys.
 - SetBrightness(string deviceOrGroupName, int brightness, int? smooth) : set the brigtness intensity.
 - SetColorTemperature(string deviceOrGroupName, int temperature, int? smooth) : change the temperature of the white light.
 - SetDefault(string deviceOrGroupName) : Save the current device state as default.
 - SetHSVColor(string deviceOrGroupName, int hue, int sat, int? smooth) : change the HSV color.
 - SetRGBColor(string deviceOrGroupName, int red, int green, int blue, int? smooth) : change the RGB color.
 - SetPower(string deviceOrGroupName, bool state) : change the power state to a specified state.
 - Toggle(string deviceOrGroupName) : reverse the current power state
 - TurnOff(string deviceOrGroupName, int? smooth = null) : Turn off the device
 - TurnOn(string deviceOrGroupName, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal) : Turn on the device

  For each message callback, the first parameter "deviceOrGroupName" refers to the name of the device or deviceGroup in the package configuration.


### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Yeelight" />
```

### Devices

Add setting named "Devices" with JSON array. For each JSON object in the array, you must provide :
* a name that will be used to control the device, 
* its hostname(or ip adress) 
* and optionnaly a port number, by default the port 55443 is used if no value.

For example :

```xml
<package name="Yeelight">
  <settings>
    <setting key="Devices">
      <content>
        [{
            "HostName": "192.168.0.123",
            "Port": "55443",
            "Name": "Livingroom"
        },{
            "HostName": "yeelight_bedroom",
            "Name": "Bedroom"
        }]
      </content>
    </setting>
  </settings>
</package>
```

You can also declare another setting named "DeviceGroups" which allows to control multiple devices at a time. You must provide : 
* a name for the group
* the list of devices the group contains. The device name is the name defined in de "Devices" setting.

```xml
<setting key="DeviceGroups">
    <content>
    [{
        "Name": "All",
        "Devices": ["Livingroom", "Bedroom"] 
	}]
    </content>
</setting>
```


Every devices properties are pushed as StateObject and contain the list of the properties of the device. Each time a device notifys a property change, the StateObject is updated.

License
----

Apache License
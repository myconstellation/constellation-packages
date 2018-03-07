# Yeelight package for Constellation

This package allow you to control Xiaomi Yeelight smart-bulbs.

### MessageCallbacks
 - GetAllProps(string name) : Get all the available properties for a bulb.
 - GetProp(string name, string propertyKey) : Get a single property by his key.
 - GetProps(string name, List<object>) : Get multiple properties by their keys.
 - SetBrightness(string name, int brightness, int? smooth) : set the brigtness intensity.
 - SetColorTemperature(string name, int red, int green, int blue, int? smooth) : change the RGB color.
 - SetPower(string name, bool state) : change the power state to a specified state.
 - Toggle(string name) : reverse the current power state


 For each message callback, the first parameter "name" refers to the name of the bulb in the package configuration.


### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Yeelight" />
```

### Bulbs

Add setting named "Bulbs" with JSON array. For each JSON object in the array, you must provide :
* a name that will be used to control the device, 
* its hostname(or ip adress) 
* and optionnaly a port number, by default the port 55443 is used if no value.

For example :

```xml
<package name="Yeelight">
  <settings>
    <setting key="Bulbs">
      <content>
        [{
            "HostName": "192.168.0.123",
            "Port": "55443",
            "Name": "Salon"
        },{
            "HostName": "yeelight_bedroom",
            "Name": "Bedroom"
        }]
      </content>
    </setting>
  </settings>
</package>
```



Every devices properties are pushed as StateObject and contain the list of the properties of the bulb. Each time a device notifys a property change, the StateObject is updated.

License
----

Apache License
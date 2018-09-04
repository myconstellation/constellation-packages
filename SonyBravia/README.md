# SonyBravia package for Constellation

This package allow you to control Sony Bravia devices.
For now, only controls through IRCC are supported, feel free to implement other controls ! (Android TV etc.)

### MessageCallbacks

 - SendIRCCCode() : Send an IRCC Code to the device

### State Objects

This package does not exposes any State Objects

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="SonyBravia" />
```

### Configuration

Three parameters : 
- [required] Hostname : the hostname (or ip adress) of the device
- [optional] Port : the port of the device's IRCC service (default : 80)
- [optional] PinCode : the pin code used to connect to the device (default : "0000")

To define a pin Code, you must follow these steps : 
1) Go in your TV settings and enable IP Control : Network > Home Network Setup > IP Control > Simple IP Control.
2) Set Authentication to "Normal and Pre-shared Key"
3) Choose a Pre-shared key. (Pin Code)

License
----

Apache License
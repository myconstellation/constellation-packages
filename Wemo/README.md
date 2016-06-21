# Constellation Package for Belkin/Wemo

This package connect your  Belkin/Wemo devices into your Constellation.

Note : in this version 1.0, only Wemo Switch and Insight are supported !

All your Belkin/Wemo devices are pushed as StateObjects with all informations.

By default, all devices are refresh each second and a new network discovery is launch every 10 minutes but you can customize this settings for your Constellation.

### MessageCallbacks
  - SetSwitchState(serialNumber, state) : Sets the switch state of your Wemo Switch / Insight.
  - Discover() : Rescan your network to discover new Wemo device.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Wemo" />
```
License
----

Apache License

# Relay Board Package for Constellation

This package connect your SainSmart USB RelayBoard to Constellation.

### StateObjects
  - The state of each relay are pushed as StateObjects
  - The RelayBoard informations

### MessageCallbacks
  - SetSwitch(relayId, relayState) : Set the state for a specific relay.

### Installation

Make sure FTDI D2XX drivers are installed: http://www.ftdichip.com/Drivers/D2XX.htm

Declare the package in a Sentinel with the following configuration :
```xml
<package name="RelayBoard">
  <settings>
	<setting key="SerialNumber" value="xxxx" />
	<setting key="RelayCount" value="8" />
  </settings>
</package>
```

If the SerialNumber is not set, the first board found is used ! The RelayCount is optional (by default: 8).

License
----

Apache License
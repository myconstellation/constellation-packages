# Constellation Package for Vera (Z-Wave)

This package connect your Vera to Constellation.

All your Z-Wave devices are pushed as StateObjects with all informations.

### MessageCallbacks
  - RunScene(sceneId) : Runs the scene
  - SetSwitchState(DeviceRequest) : Sets the state of the switch.
  - SetDimmableLevel(DeviceRequest) : Sets the state and dimmable level.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Vera">
	<settings>
		<setting key="VeraHost" value="<< IP/DNS VERA >>" />
	</settings>
</package>
```
License
----

Apache License
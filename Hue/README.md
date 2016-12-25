# Hue Package for Constellation

Control your Hue lighting system into your Constellation.

### StateObjects
  - Each Hue light are pushed as StateObjects.

### MessageCallbacks
  - SetState(lightId, state) : Sets the state.
  - SetColor(lightId, r, g, b) : Sets the color.
  - Set(lightId, state, hue, saturation, bri) : Sets the light state.
  - SetBrightness(lightId, brightness) : Sets the brightness.
  - ShowAlert(lightId, r, g, b, duration) : Shows the alert.
  - SetCommandToAll(command) : Sets the command to all.
  - SendCommandTo(command, lightList) : Sends the command to a list of lights.

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="Hue">
  <settings>
    <setting key="BridgeAddress" value="192.168.x.x" />
    <setting key="BridgeUsername" value="83b7780291a6ceffbe0bd049104df" />
  </settings>
</package>
```
License
----

Apache License
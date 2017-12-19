# Orange TV connector for Constellation

Connect your Orange Set-top box (Orange LiveBox TV) to Constellation

### StateObjects
  - State : the current state of the Set-top box (with the current standby state, current context, current played media, etc.)

### MessageCallbacks
  - SwitchTo(string epgId) : Switches to EPG identifier
  - SwitchToChannel(Channel channel) : Switches to channel
  - SendKey(Key key, PressKeyMode mode = PressKeyMode.SinglePress) : Sends the remote controller key

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="OrangeTV">
  <settings>
    <setting key="Hostname" value="192.168.x.x" />
  </settings>
</package>
```

License
----

Apache License
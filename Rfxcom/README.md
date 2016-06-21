# RFXCom Package for Constellation

This package connect your RFXCOM into your Constellation. All Temperature & Humdity sensor are pushed as StateObjects and you can also send message to Somfy devices.

### StateObjects (currently supported in version 1.0) :
  - RFXCOM device status is pushed as StateObject
  - All Temperature sensors (packet type 0x50) are pushed as StateObject (Oregon THR128/138, THC138, THC238/268, THN132, THWR288, THRN122, THN122, AW129/131, THWR800, RTHN318, La Crosse TX2, TX3, TX4, TX17, WS2300, TS15C, Viking 02811, RUBiCSON, TFA 30.3133)
  - All Temperature & Humidity sensors (packet type 0x52) are pushed as StateObject (Oregon THGN122/123, THGN132, THGR122/228/238/268, THGR810, THGN800, THGR810, RTGR328, THGR328, WTGR800, THGR918/928, THGRN228, THGN500, TFA TS34C, Cresta, WT260,WT260H,WT440H,WT450,WT450H, Viking 02035,02038 (02035 has no humidity), Rubicson, EW109)

For other devices compliants with the RFXcom device, you need to fork this Constellation package and add a [RfxPacket](RfxCom.Core/RfxPacket.cs) class to parse data and push StateObject to Constellation. For example, see the [TemperatureSensor packet class](RfxCom.Core/Packets/TemperatureSensor.cs).

### MessageCallbacks
  - RefreshStatus() : Refreshes the RFXCOM device status.
  - SendMessage(hexMessage) : Sends the message with the RFXCOM device.
  - SetProtocols(protocolsEnabled) : Sets the protocols enabled (the other will be disabled).
  - SendRFYCommand(command, id1-3, unitcode) : Sends the RFY command (for Somfy device).

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="rfxcom">
  <settings>
	<setting key="PortName" value="COM3" />
  </settings>
</package>
```

You can also enable the verbose mode, customize the name of the StateObject pushed, set the protocols enabled on RFXCOM device and forward all packet receive by the RFXCOM device to a Constellation group 

```xml
<package name="rfxcom">
  <settings>
	<setting key="PortName" value="COM3" />
    <setting key="Verbose" value="true" />
    <setting key="ProtocolsEnabled" value="Oregon Scientific,X10" />
	<Setting name="ForwardRawMessageToGroup" value="MyRFXGroup" />
    <setting key="StateObjectCustomNames">
      <content>{ "TemperatureHumiditySensor_21762": "Bathroom Sensor", "TemperatureHumiditySensor_64001": "Bedroom Sensor", "TemperatureSensor_22529": "Outdoor Temperature" }</content>
    </setting>
  </settings>
</package>
```

License
----

Apache License
# Huawei mobile router package for Constellation

Connect your Orange Flybox / Airbox, Bouygues 4G Box or any Huawei mobile router to Constellation.

### StateObjects
  - DeviceInformation : Huawei device information
  - DeviceSignal : Cell ID and signal quality parameters: RSRQ, RSRP, RSSI, SINR, RSCP, Ec/Io
  - MonitoringStatus : Router status
  - MonthlyStatistics : Router monthly statistics
  - PinStatus : Extended SIM information
  - PLMNInformations : Public Land Mobile Network information
  - TrafficStatistics : Traffic transferred
  - WlanBasicSettings : WiFi setup information
  - Notification : Current system notification
  - SMSList : SMS list on the SIM

### MessageCallbacks
  - SendSMS(string phoneNumber, string content) : Sends the SMS (one or more phone numbers separated by commas)
  - SetSMSRead(int smsIndex) : Sets the SMS read.
  - DeleteSMS(int smsIndex) : Deletes the SMS.

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="HuaweiMobileRouter">
  <settings>
    <setting key="Host" value="192.168.1.1" />
    <setting key="Username" value="admin" />
    <setting key="Password" value="P@ssw0rd!" />
  </settings>
</package>
```

### Receiving SMS in Constellation

You can forward incoming SMS received by your Huawei router to a Constellation group.

To do this, define the group name where your SMS will be sent in the package settings.

You can also choose to delete or keep SMS after forwarding.

```xml
<setting key="ForwardIncomingSMSTo" value="MySMSGroup" />
<setting key="KeepSMSCopy" value="false" />
```

License
----

Apache License
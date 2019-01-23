# ZoneMinder package for Constellation

Connect your ZoneMinder (>= 1.29) video security system to Constellation.

Documentation : https://developer.myconstellation.io/package-library/zoneminder/

### MessageCallbacks
 - Restart() : Restarts ZoneMinder.
 - ChangeState(string state) : Changes the state of ZoneMinder.
 - ForceAlarm(int monitorId) : Forces the alarm.
 - CancelForcedAlarm(int monitorId) : Cancels the forced alarm.
 - SetMonitorFunction(int monitorId, MonitorFunction function) : Sets the monitor function.

### StateObjects

 - "Host" : represent your ZoneMinder instance (URI, version, CPU load, etc..) 
 - One StateObject per camera (with ID, name, type, monitor function, Stream URI, state, events)

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="ZoneMinder">
  <settings>
    <add key="RootUri" value="http://zoneminder" />
    <add key="Username" value="admin" />
    <add key="PasswordHash" value="4F56EF3FCEF3F995F03D1E37E2D692D420111476" />
    <add key="SecretHash" value="T9TyLzZ7yJBvmKx8TyLefhVVtz8AUZXf" />
  </settings>
</package>
```

For more information read this : https://developer.myconstellation.io/package-library/zoneminder/#Installation_du_package_Constellation

License
----

Apache License
# Constellation Package to Ping

This package allow you to ping a network equipement or to check if a port is open.

### MessageCallbacks
 - Check_Ping(string Target) : send a ping to a ip address and return boolean.
 - Check_Port(string IP_Address, Int32 Port) : check if port is open on ip address and return boolean.

 ### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Pings" />
```

License
----

Apache License
# Mikrotik Package for Constellation

This package queries and pushes informations about your Mikrotik device to Constellation

### StateObjects
  - All interfaces (Ethernet, Wireless, VLAN, etc ...)
  - System resource infos
  - IP addresses
  - Queues Simple
  - DHCP Server Leases
  - CapsMan Registration table (wireless clients connected to yours CAPs)

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="Mikrotik">
 <settings>
   <setting key="Address" value="192.168.88.1" />
   <setting key="Username" value="constellation" />
   <setting key="Password" value="password" />
 </settings>
</package>
```

By default the package queries your Mikrotik device every 5 seconds but you can customize this interval :
```xml
<setting key="QueryInterval" value="5000" />
```

License
----

Apache License
# NetworkTools package for Constellation

This package allow you to ping a network equipement, check if a port or port range is open, check an HTTP address, resolve DNS host, send Wake on LAN packet.

### MessageCallbacks
 - Ping(string host, int timeout = 5000) : Pings the specified host and return the response time.
 - CheckPort(string host, int port, int timeout = 5000) : Check a port's status by entering an address and port number above and return the response time.
 - CheckHttp(string address) : Checks the HTTP address and return the response time.
 - ScanPort(string host, int startPort, int endPort, int timeout = 100) : Scans TCP port range to discover which TCP ports are open on your target host.
 - WakeUp(string host, string macAddress) : Wakes up the specified host.
 - DnsLookup(string host) : Resolves a host name or IP address.

 ### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="NetworkTools" />
```

License
----

Apache License
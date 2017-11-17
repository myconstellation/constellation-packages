# NetworkTools package for Constellation

This package allow you to ping a network equipement, check if a port or port range is open, check an HTTP address, resolve DNS host, send Wake on LAN packet.

You can also monitor your devices, web sites and services.

### MessageCallbacks
 - Ping(string host, int timeout = 5000) : Pings the specified host and return the response time.
 - CheckPort(string host, int port, int timeout = 5000) : Check a port's status by entering an address and port number above and return the response time.
 - CheckHttp(string address, int timeout = 100000) : Checks the HTTP address and return the response time.
 - ScanPort(string host, int startPort, int endPort, int timeout = 100) : Scans TCP port range to discover which TCP ports are open on your target host.
 - WakeUp(string macAddress) : Wakes up the specified host by MAC address.
 - DnsLookup(string host) : Resolves a host name or IP address.

 ### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="NetworkTools" />
```

### Monitoring

Add setting named "Monitoring" with JSON array. For each JSON object in the array, you must provide a name for the resource to monitor and a type of monitoring : Ping, Tcp or Http.

For example :

```xml
<package name="NetworkTools">
  <settings>
    <setting key="Monitoring">
      <content>
        [
           { "Name": "Google Public DNS", "Type": "Ping", "Hostname": "8.8.8.8", "Interval": 10 },
           { "Name": "Local Constellation Service", "Type": "Tcp",  "Hostname": "localhost", "Port": 8088 },
           { "Name": "Github.com", "Type": "Http", "Address": "https://github.com/myconstellation", "Regex": "Constellation", "Interval": 30 }
        ]
      </content>
    </setting>
  </settings>
</package>
```

To monitor a network equipement with ICMP echo :
```json
{ "Name": "Ping My Machine", "Type": "Ping", "Hostname": "myhostname.mydomain.com" }
```

By default the resource is check every minute but you can override this interval (in second):
```json
{ "Name": "Ping My Machine", "Type": "Ping", "Hostname": "myhostname.mydomain.com", "Interval":10 }
```

To monitor a network TCP service :
```json
{ "Name": "My Web Server", "Type": "TCP", "Hostname": "myhostname.mydomain.com", "Port": 80, "Interval":10 }
```

To monitor a Web page :
```json
{ "Name": "Check Sebastien.warin.fr", "Type": "Http", "Address": "http://sebastien.warin.fr" }
```

You can also add a regex to check the HTTP response content w/ or wo/ custom interval :
```json
{ "Name": "Check Sebastien.warin.fr", "Type": "Http", "Address": "http://sebastien.warin.fr", "Regex": "Le blog personnel et technique de Sebastien Warin", "Interval": 30 }
```

Every resource is pushed as StateObject and contain the result (boolean) and the response time (long).

License
----

Apache License
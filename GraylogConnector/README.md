# Graylog connector for Constellation

The package sends the StateObjects, Package logs, Sentinel updates or Package states to a GELF server (Graylog, logstash, ...).

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Graylog">
  <graylogConfiguration xmlns="urn:GraylogConnector" sendPackageLogs="true" sendPackageStates="true" sendSentinelUpdates="true">
    <subscriptions>           
      <subscription package="HWMonitor" name="/intelcpu/0/load/0">
        <aggregation>
          <aggregateProperty propertyName="Value" />
        </aggregation>
      </subscription>
      <subscription package="HWMonitor" name="/ram/load/0">
        <aggregation>
          <aggregateProperty propertyName="Value" />
        </aggregation>
      </subscription>
      <subscription package="ddwrt">
        <exclusions>
          <exclusion type="ddwrt.LANStatus" />
          <exclusion type="ddwrt.StatInterface" />
          <exclusion type="ddwrt.WirelessClientStatus" />
        </exclusions>
      </subscription>
      <subscription package="ddwrt" type="ddwrt.StatInterface">
        <aggregation>
          <aggregateProperty includeAggregateInfo="true" propertyName="ReceiveBytes" />
          <aggregateProperty includeAggregateInfo="true" propertyName="ReceivePackets" />
          <aggregateProperty includeAggregateInfo="true" propertyName="TransmitBytes" />
          <aggregateProperty includeAggregateInfo="true" propertyName="TransmitPackets" />
          <aggregateProperty includeAggregateInfo="true" propertyName="BandwidthIn" />
          <aggregateProperty includeAggregateInfo="true" propertyName="BandwidthOut" />
        </aggregation>
      </subscription>
      <subscription package="NetAtmo" />
      <subscription package="Vera">
        <exclusions>
          <exclusion name="Home Energy Monitor" />
        </exclusions>
      </subscription>
      <subscription package="Nest" /> 
    </subscriptions>
    <outputs>
      <gelfOutput name="My GELF UDP server" host="graylog.mydomain.com" port="12203" protocol="Udp" />
    </outputs>
  </graylogConfiguration>
</package>
```

  - The package must be configure with an "Enable ControlHub" access key to subscribe to package logs, sentinel updates and package states.
  - Add your GELF server into the <outputs> section
  - Add your StateObject subscriptions into the <subscriptions> section

For each subscription you can specify the sentinel name, package name, stateobject name and the stateobject type. 

You can also specify StateObject to exclude for a subscription and aggregate property of your StateObject.

For more information go to the Developer Portal : https://developer.myconstellation.io

### StateObjects
  - Nothing

### MessageCallbacks
  - Nothing

License
----

Apache License

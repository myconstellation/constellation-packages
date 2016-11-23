# Constellation Package for Vera (Z-Wave)

This package monitor your system's hardware, operating-system and networking software via SNMP.

### StateObjects
  - All your SNMP devices registered in the package's settings are pushed as StateObjects.

### MessageCallbacks
  - CheckAgent(host, [community]) : Checks the SNMP agent for the specified host.
  - ScanDevice(host, [community]) : Scans the SNMP device and return all SNMP informations.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Snmp">
	<settings>
		<setting key="snmpConfiguration">
		  <content>
			<snmpConfiguration queryInterval="00:00:10" multipleStateObjectsPerDevice="false">
			  <devices>
				<device host="myDevice.domain.com" />
				<device host="192.168.0.1" />
				<device host="192.168.0.10" community="demo" />
			  </devices>
			</snmpConfiguration>
		  </content>
		</setting>	
	</settings>
</package>

You can add yours devices to monitoring through SNMP by adding the "device" tag.

For each device, you must define the host (IP or DNS hostname) and optionally the community name ('public' if not set).

```
License
----

Apache License
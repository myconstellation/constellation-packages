# Modbus connector for Constellation

This is a Modbus TCP/RTU connector for Constellation.

The package allows you to read and write Modbus registers and coils from your Constellation-connected applications. Also the package can periodically query registers to fill objects that will be published as StateObject in your Constellation.

For more information about Modbus :
- http://www.simplymodbus.ca/index.html
- https://ipc2u.com/articles/knowledge-base/detailed-description-of-the-modbus-tcp-protocol-with-command-examples/

The package is based on the EasyModbusTCP library written by Stefan Roﬂmann. http://easymodbustcp.net/en/

### MessageCallbacks
  - ReadCoils(int startingAddress, int quantity) : This command is requesting the ON/OFF status of discrete coils (FC=01).
  - ReadDiscreteInputs(int startingAddress, int quantity) : This command is requesting the ON/OFF status of discrete inputs (FC=02).
  - ReadHoldingRegisters(int startingAddress, int quantity) : This command is requesting the content of analog output holding registers (FC=03).
  - ReadInputRegisters(int startingAddress, int quantity) : This command is requesting the content of analog input register (FC=04).
  - WriteSingleCoil(int startingAddress, bool value) : This command is writing the contents of discrete coil (FC=05).
  - WriteSingleRegister(int startingAddress, int value) : This command is writing the contents of analog output holding register (FC=06).
  - ReadWriteMultipleRegisters(int startingAddressRead, int quantityRead, int startingAddressWrite, int[] values) : This command is reading and writing the contents of analog output holding registers (FC=03 + FC=16).
  - WriteMultipleCoils(int startingAddress, bool[] values) : This command is writing the contents of a series of discrete coils (FC=15).
  - WriteMultipleRegisters(int startingAddress, int[] values) : This command is writing the contents of analog output holding registers (FC=16).

### StateObjects
  - All Modbus devices defined in the "Devices" setting to be request periodically (see below).

### Installation

 - Declare the package in a Sentinel with the following configuration :
```xml
<package name="Modbus">
    <settings>
        <setting key="TcpAddress" value="192.168.0.1" />
        <setting key="TcpPort" value="26" />
    </setting>
</package>
```
In this case the package communicate to the Modbus via tcp://192.168.0.1:26 

- You can also used the Modbus RTU by specifying the SerialPort's name and baud rate :

```xml
<package name="Modbus">
    <settings>
        <setting key="RtuSerialPort" value="COM3" />
        <setting key="RtuBaudRate" value="9200" />
    </setting>
</package>
```

- If you have multiple ModbusTCP gateway you can declare multiple instance of the package for each gateway :

```xml
<package name="ModbusGateway1" filename="Modbus.zip">
    <settings>
        <setting key="TcpAddress" value="192.168.0.1" />
        <setting key="TcpPort" value="26" />
    </setting>
</package>
<package name="ModbusGateway2" filename="Modbus.zip">
    <settings>
        <setting key="TcpAddress" value="192.168.0.2" />
        <setting key="TcpPort" value="26" />
    </setting>
</package>
```

- With the above configurations you have access to MessageCallbacks to read or write registrers and coils on the bus. To enable periodic Modbus devices polling to publish StateObjects, you have set the "Devices" setting (More information below).

### Settings

 - TcpAddress (string) : The Modbus TCP gateway address
 - TcpPort (int) : The Modbus TCP gateway port
 - RtuSerialPort (string) : The Modbus RTU serial port
 - RtuBaudRate (int) : The Modbus RTU serial baud rate (9600 by default)
 - ConnectionTimeout (int) : The connection timeout in millisecond (5000 by default)
 - ReconnectionDefaultInterval (int) : The reconnection default interval in second (30 by default)
 - ReconnectionMaxInterval (int) : The reconnection max interval in second (3600 by default)
 - Verbose (bool) : Enable the verbose mode (false by default)
 - ModbusDebug (bool) : Enble the RS485 debug mode (false by default)
 - Devices (json) : The modbus devices to request and publish as StateObject (optional)
 - ModbusDeviceDefinitions (json) : The modbus devices definitions to request

You must either define the "TcpAddress" and "TcpPort" settings in the case of ModbusTCP or the "RtuSerialPort" setting in the case of Modbus RTU.

If none of these settings are correctly defined, the package will throw an error on startup.

 ### Request Modbus data

To query Modbus devices periodically you must first describe them in the "ModbusDeviceDefinitions" setting and then define the device list.

Each Modbus device type is described in a JSON array in the "ModbusDeviceDefinitions" setting. A definition object contains the following properties:
* Name : the name of the device's type
* Description : the description the device (used to describe the StateObject type related)
* Properties : the properties of the device. A property is also a JSOn object with the following properties:
  * Name : the property's name
  * Description : the property's description (used to describe the StateObject type related)
  * RegisterType : the type of the Modbus register to read (can be "Holding" or "Input")
  * Address : the Data Address of the first register to read.
  * Length : the total number of registers requested (optional, 1 by default).
  * Ratio : the ratio to apply to the raw value (optional, 1 by default).
  * Type : the property's type (can be "Boolean", "Float" or "Int").

Once you have defined your modbus devices definitions, you can define the list of devices to query in the "Device" setting.

This is a JSON array and for each device you need to specified the name (used as StateObject's name), the Modbus ID (slave ID), the request interval in second and the device type defined above.

 ### Example : request the DZT-6001 energy meter

 For example, the DZT-6001 is a RS485/Modbus Single phase kWh meter (https://www.dutchmeters.com/index.php/product/dzt-6001/).

 The following read out commands can be given:

| ID | Reg. Adress | Reg. length | Answer | Convert to | Notes
|----|---|---|---|---|---|
| 1. Voltage | 0000 | 0001 | HEX | Decimal | divide by 10 to read V. If answer is: 91F = 2335 = 233,5V
| 2. Current | 0001 | 0001 | HEX | Decimal | divide by 10 to read A.
| 3. Active power | 0003 | 0001 | HEX | Decimal | Result is W ñ divide by 1000 to get kW
| 4. Reactive power | 0004 | 0001 | HEX | Decimal | Result is W ñ divide by 1000 to get kW
| 5. Apparent power | 0005 | 0001 | HEX | Decimal | Result is W ñ divide by 1000 to get kW
| 6. Active energy | 0007 | 000a | HEX | Decimal | Result divide by 100 to get kW. 5 blocks of 4 bite; Total; T1; T2; T3; T4
| 7. Reactive energy | 0011 | 000a | HEX | Decimal | Result divide by 100 to get kW. 5 blocks of 4 bite; Total; T1; T2; T3; T4

We can define "ModbusDeviceDefinitions" setting like this :

 ```json
[
    {
      Name: "DTZ.EM6001", Description:"DZT 6001 80A Single phase kWh meter", Properties: [
        { Name: "Volt", RegisterType: "Holding", Address: "0", Length: 1, Ratio:0.1, Type:"Float", Description: "Voltage (V)" },
        { Name: "Ampere", RegisterType: "Holding", Address: "1", Length: 1, Ratio:0.1, Type:"Float", Description: "Current (A)" },
        { Name: "Hz", RegisterType: "Holding", Address: "2", Length: 1, Ratio:0.1, Type:"Float", Description: "Frequency (Hz)" },
        { Name: "Watt", RegisterType: "Holding", Address: "3", Length: 1, Type:"Float", Description: "Active power (W)" },
        { Name: "var", RegisterType: "Holding", Address: "4", Length: 1, Type:"Float", Description: "Reactive power (var)" },
        { Name: "VA", RegisterType: "Holding", Address: "5", Length: 1, Type:"Float", Description: "Apparent power (VA)" },
        { Name: "PF", RegisterType: "Holding", Address: "6", Length: 1, Ratio:0.1, Type:"Float", Description: "Power Factor" },
        { Name: "kWh_TotalIn", RegisterType: "Holding", Address: 7, Length: 2, Ratio:0.01, Type:"Float", Description: "Total imported active energy (kWh)" },
        { Name: "Kvarh_TotalIn", RegisterType: "Holding", Address: "11", Length: 2, Ratio:0.01, Type:"Float", Description: "Total imported reactive energy (kvarh)" }
      ]
    }
]
```

And then if we have on our Modbus 3 DTZ devices energy meter that we want to query every 5 seconds for each one, we can define the setting "Device" like this:
 ```json
[
    { Name: "My counter 1", DeviceType: "DTZ.EM6001", SlaveID: 1, RequestInterval: 5 },
    { Name: "My counter 2", DeviceType: "DTZ.EM6001", SlaveID: 2, RequestInterval: 5 },
    { Name: "My counter 3", DeviceType: "DTZ.EM6001", SlaveID: 3, RequestInterval: 5 },
]
```

In your Constellation you will have 3 StateObjects named "My counter X" (1,2 and 3) of type "DTZ.EM6001" with 9 properties updated every 5 seconds.

 ### Modbus device definition known

The default value for the "ModbusDeviceDefinitions" setting  contains the definition for the following devices :

- DTZ-6001 80A Single phase kWh meter (https://www.dutchmeters.com/index.php/product/dzt-6001/)
- MCI CONTAX 1M 100A Single phase kWh meter (https://www.mci-compteur-electrique.fr/mesure-electrique/compteurs-denergie-monophase/432-contax-1m-100a)
- MCI CONTAX S TRI 2M 3 phase kWh meter (https://www.mci-compteur-electrique.fr/mesure-electrique/compteurs-denergie-triphases-sur-ti/433-contax-s-tri-2m)
- Carlo Gavazzi EM11x Series (https://www.camax.co.uk/blog/carlo-gavazzis-new-em100-series-is-available-at-camax-uk)

```json
[
  {
    Name: "MCI.Contax1M", Description:"MCI CONTAX 1M 100A", Properties: [
      { Name: "Hz", RegisterType: "Holding", Address: "130", Length: 1, Ratio:0.01, Type:"Float", Description: "Frequency (Hz)" },
      { Name: "Volt", RegisterType: "Holding", Address: "131", Length: 1, Ratio:0.01, Type:"Float", Description: "Voltage (V)" },
      { Name: "Ampere", RegisterType: "Holding", Address: "139", Length: 2, Ratio:0.001, Type:"Float", Description: "Current (A)" },
      { Name: "Watt", RegisterType: "Holding", Address: "140", Length: 2, Type:"Float", Description: "Active power (W)" },
      { Name: "var", RegisterType: "Holding", Address: "148", Length: 2, Type:"Float", Description: "Reactive power (var)" },
      { Name: "VA", RegisterType: "Holding", Address: "150", Length: 2, Type:"Float", Description: "Apparent power (VA)" },
      { Name: "PF", RegisterType: "Holding", Address: "158", Length: 1, Ratio:0.001, Type:"Float", Description: "Power Factor" },
      { Name: "kWh_TotalIn", RegisterType: "Holding", Address: "A000", Length: 2, Ratio:0.01, Type:"Float", Description: "Total imported active energy (kWh)" },
      { Name: "Kvarh_TotalIn", RegisterType: "Holding", Address: "A01E", Length: 2, Ratio:0.01, Type:"Float", Description: "Total imported reactive energy (kvarh)" },
    ]
  },
  {
    Name: "MCI.ContaxTri2M", Description:"MCI CONTAX S TRI 2M", Properties: [
      { Name: "Hz", RegisterType: "Holding", Address: "2", Length: 1, Ratio:0.01, Type:"Float", Description: "Frequency (Hz)" },
      { Name: "Volt_L1", RegisterType: "Holding", Address: "1000", Length: 2, Ratio:0.01, Type:"Int", Description: "L1-N PhaseA Voltage (V)" },
      { Name: "Volt_L2", RegisterType: "Holding", Address: "1002", Length: 2, Ratio:0.01, Type:"Float", Description: "L2-N PhaseB Voltage (V)" },
      { Name: "Volt_L3", RegisterType: "Holding", Address: "1004", Length: 2, Ratio:0.01, Type:"Float", Description: "L3-N PhaseC Voltage (V)" },
      { Name: "Ampere_L1", RegisterType: "Holding", Address: "100C", Length: 2, Ratio:0.001, Type:"Float", Description: "PhaseA current" },
      { Name: "Ampere_L2", RegisterType: "Holding", Address: "100E", Length: 2, Ratio:0.001, Type:"Float", Description: "PhaseB current" },
      { Name: "Ampere_L3", RegisterType: "Holding", Address: "1010", Length: 2, Ratio:0.001, Type:"Float", Description: "PhaseC current" },
      { Name: "PF", RegisterType: "Holding", Address: "1012", Length: 1, Ratio:0.001, Type:"Float", Description: "A+B+C Power Factor" },
      { Name: "PF_L1", RegisterType: "Holding", Address: "1013", Length: 1, Ratio:0.001, Type:"Float", Description: "PhaseA Power Factor" },
      { Name: "PF_L2", RegisterType: "Holding", Address: "1014", Length: 1, Ratio:0.001, Type:"Float", Description: "PhaseB Power Factor" },
      { Name: "PF_L3", RegisterType: "Holding", Address: "1015", Length: 1, Ratio:0.001, Type:"Float", Description: "PhaseC Power Factor" },
      { Name: "Watt", RegisterType: "Holding", Address: "6000", Length: 2, Type:"Float", Description: "A+B+C Active power (W)" },
      { Name: "Watt_L1", RegisterType: "Holding", Address: "6002", Length: 2, Type:"Float", Description: "PhaseA Active power (W)" },
      { Name: "Watt_L2", RegisterType: "Holding", Address: "6004", Length: 2, Type:"Float", Description: "PhaseB Active power (W)" },
      { Name: "Watt_L3", RegisterType: "Holding", Address: "6006", Length: 2, Type:"Float", Description: "PhaseC Active power (W)" },
      { Name: "var", RegisterType: "Holding", Address: "6000", Length: 2, Type:"Float", Description: "A+B+C Reactive power (var)" },
      { Name: "var_L1", RegisterType: "Holding", Address: "6002", Length: 2, Type:"Float", Description: "PhaseA Reactive power (var)" },
      { Name: "var_L2", RegisterType: "Holding", Address: "6004", Length: 2, Type:"Float", Description: "PhaseB Reactive power (var)" },
      { Name: "var_L3", RegisterType: "Holding", Address: "6006", Length: 2, Type:"Float", Description: "PhaseC Reactive power (var)" },
      { Name: "kWh_TotalIn", RegisterType: "Holding", Address: "7000", Length: 2, Ratio:0.1, Type:"Float", Description: "Total imported active energy (kWh)" },
      { Name: "Kvarh_TotalIn", RegisterType: "Holding", Address: "7100", Length: 2, Ratio:0.1, Type:"Float", Description: "Total imported reactive energy (kvarh)" },
      { Name: "OutputRelay", RegisterType: "Holding", Address: "8003", Length: 1, Type:"Boolean", Description: "Remote control output" }
    ]
  },
  {
    Name: "DTZ.EM6001", Description:"DZT 6001 80A Single phase kWh meter", Properties: [
      { Name: "Volt", RegisterType: "Holding", Address: "0", Length: 1, Ratio:0.1, Type:"Float", Description: "Voltage (V)" },
      { Name: "Ampere", RegisterType: "Holding", Address: "1", Length: 1, Ratio:0.1, Type:"Float", Description: "Current (A)" },
      { Name: "Hz", RegisterType: "Holding", Address: "2", Length: 1, Ratio:0.1, Type:"Float", Description: "Frequency (Hz)" },
      { Name: "Watt", RegisterType: "Holding", Address: "3", Length: 1, Type:"Float", Description: "Active power (W)" },
      { Name: "var", RegisterType: "Holding", Address: "4", Length: 1, Type:"Float", Description: "Reactive power (var)" },
      { Name: "VA", RegisterType: "Holding", Address: "5", Length: 1, Type:"Float", Description: "Apparent power (VA)" },
      { Name: "PF", RegisterType: "Holding", Address: "6", Length: 1, Ratio:0.1, Type:"Float", Description: "Power Factor" },
      { Name: "kWh_TotalIn", RegisterType: "Holding", Address: 7, Length: 2, Ratio:0.01, Type:"Float", Description: "Total imported active energy (kWh)" },
      { Name: "Kvarh_TotalIn", RegisterType: "Holding", Address: "11", Length: 2, Ratio:0.01, Type:"Float", Description: "Total imported reactive energy (kvarh)" }
    ]
  },
  {
    Name: "CarloGavazzi.EM11x", Description:"Carlo Gavazzi EM11x Series", Properties: [
      { Name: "Volt", RegisterType: "Holding", Address: "0", Length: 2, Ratio:0.1, Type:"Float", Description: "Voltage (V)" },
      { Name: "Ampere", RegisterType: "Holding", Address: "2", Length: 2, Ratio:0.001, Type:"Float", Description: "Current (A)" },
      { Name: "Watt", RegisterType: "Holding", Address: "4", Length: 2, Ratio:0.1, Type:"Float", Description: "Active power (W)" },
      { Name: "VA", RegisterType: "Holding", Address: "6", Length: 2, Ratio:0.1, Type:"Float", Description: "Apparent power (VA)" },
      { Name: "var", RegisterType: "Holding", Address: "8", Length: 2, Type:"Float", Description: "Reactive power (var)" },
      { Name: "Wdmd", RegisterType: "Holding", Address: "A", Length: 2, Type:"Float", Description: "Requested average power calculate for the set interval (W)" },
      { Name: "WdmdPeak", RegisterType: "Holding", Address: "C", Length: 2, Type:"Float", Description: "Maximum requested power reached since last reset (W)" },
      { Name: "PF", RegisterType: "Holding", Address: "E", Length: 1, Ratio:0.001, Type:"Float", Description: "Power Factor" },
      { Name: "Hz", RegisterType: "Holding", Address: "F", Length: 2, Ratio:0.1, Type:"Float", Description: "Frequency (Hz)" },
      { Name: "kWh_TotalIn", RegisterType: "Holding", Address: "10", Length: 2, Ratio:0.1, Type:"Float", Description: "Total imported active energy (kWh)" },
      { Name: "Kvarh_TotalIn", RegisterType: "Holding", Address: "12", Length: 2, Ratio:0.1, Type:"Float", Description: "Total imported reactive energy (kvarh)" },
      { Name: "kWh_PartialIn", RegisterType: "Holding", Address: "14", Length: 2, Ratio:0.1, Type:"Float", Description: "Partial imported active energy (kWh)" },
      { Name: "Kvarh_PartialIn", RegisterType: "Holding", Address: "16", Length: 2, Ratio:0.1, Type:"Float", Description: "Partial imported reactive energy  (kvarh)" },
      { Name: "kWh_Tariff1In", RegisterType: "Holding", Address: "18", Length: 2, Ratio:0.1, Type:"Float", Description: "Active energy with tariff 1 (kWh)" },
      { Name: "kWh_Tariff2In", RegisterType: "Holding", Address: "1A", Length: 2, Ratio:0.1, Type:"Float", Description: "Active energy with tariff 2 (kWh)" },
      { Name: "kWh_TotalOut", RegisterType: "Holding", Address: "20", Length: 2, Ratio:0.1, Type:"Float", Description: "Total exported active energy (kWh)" },
      { Name: "Kvarh_TotalOut", RegisterType: "Holding", Address: "22", Length: 2, Ratio:0.1, Type:"Float", Description: "Total exported reactive energy (kvarh)" },
      { Name: "HourCounter", RegisterType: "Holding", Address: "2C", Length: 2, Ratio:0.01, Type:"Float", Description: "Counter uptime (hour)" }
    ]
  }
]
```

License
----

Apache License
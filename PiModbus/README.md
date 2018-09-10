# Raspberry Pi Modbus connector for Constellation

Raspberry Pi Modbus connector for Constellation

### MessageCallbacks
  - ReadModbusRegister(registerAddress, registerCount, slaveId=1) : Request input registers from the specified address

### StateObjects
  - All Modbus devices configurated in settings to be request periodically

### Installation

 - Enable Serial hardware (UART)
```
sudo raspi-config
```

 - Install dependencies
```
sudo apt-get install automake libtool
```

 - Compile & Install libmodbus for RPi
```
git clone https://github.com/sebastienwarin/libmodbus
./autogen.sh
./configure
make
sudo make install
```

 - Declare the package in a RPi's Sentinel with the following configuration :
```xml
<package name="Modbus" enable="true" autoStart="true">
    <settings>
        <setting key="PinDE" value="23" />
        <setting key="PinRE" value="24" />
    </setting>
    </settings>
</package>
```

By default the UART port used is "/dev/ttyAMA0" at 9600 bauds but you can overwrite this default settings :

```xml
<package name="Modbus" enable="true" autoStart="true">
    <settings>
        <setting key="Port" value="/dev/ttyAMA0" />
        <setting key="BaudRate" value="9600" />
        <setting key="PinDE" value="23" />
        <setting key="PinRE" value="24" />
        <setting key="ModbusDebug" value="false" />
    </setting>
    </settings>
</package>
```

### Settings

 - Port (string) : the UART port (/dev/ttyAMA0 by default)
 - BaudRate (int) : the UART baud rate (9600 by default)
 - PinDE (int) : the BCM pin number the Max485's DE
 - PinRE (int) : the BCM pin number the Max485's RE
 - ModbusDebug (bool) : Enble the RS485 debug mode (false by default)
 - Devices (json) : The modbus devices to request and publish as StateObject (optional)

 ### Request Modbus data
 
 To request periodically your Modbus device, you need to declare them into the "Devices" setting.

 For each device, you need to specified the Modbus ID (slave ID), the register address and the number of registers to requests and the interval. Then the registers read are encapsulated in a StateObject with a set of typed property.

 ```json
 [
  {
    "Name": "<device/SO name>",
    "SlaveID": <device id on the modbus>,
    "RegisterAddress": <register address>,
    "RegistersCount": <number of registers to request>,
    "RequestInterval": <the interval in millisecond to request and refresh data>,
    "StateObjectTypeName": "<the type of this stateobject>",
    "Properties": [
         { "Name": "<Property name>", "Selector": "<python code to select the value in the results>", "Type":"<type of the property>", "Description": "<description of the property>" },
         ....
     ]
  },
  .....
]
```

 ### Example : request the DZT-6001 energy meter

 For example, the DZT-6001 is a RS485/Modbus Single phase kWh meter (https://www.dutchmeters.com/index.php/product/dzt-6001/).

 The following read out commands can be given:

| ID | Reg. Adress | Reg. length | Answer | Convert to | Notes
|----|---|---|---|---|---|
| 1. Voltage | 0000 | 0001 | HEX | Decimal | divide by 10 to read V. If answer is: 91F = 2335 = 233,5V
| 2. Current | 0001 | 0001 | HEX | Decimal | divide by 10 to read A.
| 3. Active power | 0003 | 0001 | HEX | Decimal | Result is W – divide by 1000 to get kW
| 4. Reactive power | 0004 | 0001 | HEX | Decimal | Result is W – divide by 1000 to get kW
| 5. Apparent power | 0005 | 0001 | HEX | Decimal | Result is W – divide by 1000 to get kW
| 6. Active energy | 0007 | 000a | HEX | Decimal | Result divide by 100 to get kW. 5 blocks of 4 bite; Total; T1; T2; T3; T4
| 7. Reactive energy | 0011 | 000a | HEX | Decimal | Result divide by 100 to get kW. 5 blocks of 4 bite; Total; T1; T2; T3; T4
| 8. Baud rate | 002a | 0001 | HEX | Decimal | 01=1200; 02=2400; 03=4800; 04=9600
| 9. Meter ID | 002b | 0001 | HEX | Decimal |

So you can declare your DTZ to be requete each 5 seconds with the following configuration :

 ```xml
    <package name="Modbus" enable="true" autoStart="true">
            <settings>
              <setting key="PinDE" value="23" />
              <setting key="PinRE" value="24" />
              <setting key="ModbusDebug" value="false" />
              <setting key="Devices">
                <content>[{
                    "Name": "My DTZ",
                    "SlaveID": 1,
                    "RegisterAddress": 0,
                    "RegistersCount": 19,
                    "RequestInterval": 5000,
                    "StateObjectTypeName": "DZT",
                    "Properties": [
                            { "Name": "Volt", "Selector": "registers[0]/10.0", "Type":"Float", "Description": "Voltage (V)" },
                            { "Name": "Ampere", "Selector": "registers[1]/10.0", "Type":"Float", "Description": "Current (A)" },
                            { "Name": "Hz", "Selector": "registers[2]/10.0", "Type":"Float", "Description": "Frequency (Hz)" },
                            { "Name": "Watt", "Selector": "registers[3]", "Type":"Float", "Description": "Active power (W)" },
                            { "Name": "var", "Selector": "registers[4]", "Type":"Float", "Description": "Reactive power (var)" },
                            { "Name": "VA", "Selector": "registers[5]", "Type":"Float", "Description": "Apparent power (VA)" },
                            { "Name": "PF", "Selector": "registers[6]/10.0", "Type":"Float", "Description": "Power Factor" },
                            { "Name": "kWh_Total_In", "Selector": "(registers[7]+registers[8])/100.0", "Type":"Float", "Description": "Total imported active energy (kWh)" },
                            { "Name": "Kvarh_Total_In", "Selector": "(registers[17]+registers[18])/100.0", "Type":"Float", "Description": "Total imported reactive energy (kvarh)" }
                      ]
                   }
                ]</content>
              </setting>
            </settings>
          </package>
```

Explanations :
 - The SlaveID is #1, requested each 5 seconds (5000ms)
 - We read from the register address 0 to 25 (to get the value 1 to 8 : 1+1+1+1+1+A+A = 25 registers)
 - The result StateObject type will be "DTZ" describe with 9 properties :
    - Volt (Voltage (V)) as Float selected from the expression "registers[0]/10.0" (the first register divide by 10 to read V)
    - Ampere (Current (A)) as Float selected from the expression "registers[1]/10.0" (the 2nd register divide by 10 to read A)
    - Hz (Frequency (Hz)) as Float selected from the expression "registers[2]/10.0" (the 3th register divide by 10 to read Hz)
    - Watt (Active power (W)) as Float selected from the expression "registers[3]" (the 4th register to read W)
    - var (Reactive power (var)) as Float selected from the expression "registers[4]" (the 5th register to read var)
    - VA (Apparent power (VA)) as Float selected from the expression "registers[5]"  (the 6th register to read VA)
    - PF (Power Factor) as Float selected from the expression "registers[6]/10.0" (the 7th register divide by 10 to %)
    - kWh_Total_In (Total imported active energy (kWh)) as Float selected from the expression "(registers[7]+registers[8])/100.0" (the 8th and 9th registers divide by 100 to read the Total imported active energy)
    - Kvarh_Total_In (Total imported reactive energy (kvarh)) as Float selected from the expression "(registers[17]+registers[18])/100.0" (the 18th and 19th registers divide by 100 to read the Total imported reactive energy)


License
----

Apache License
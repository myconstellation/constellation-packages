# LightSensor Package for Constellation

This package is designed to run on a Raspberry (version 2 or 3) and measured the ambiant luminosity and push the value as StateObject on Constellation.

This package support the TSL2561 sensor and a simple Photoresistor. 

### StateObjects
  - Light : resistive sensor with RC timing if the EnablePhotoResistor = true
  - Lux : TSL2561 data informations (complex object with Lux, IR and broadband values) if the EnableTSL2561 = true

### Installation

Declare the package in a RPi's Sentinel with the following configuration :
```xml
<package name="LightSensor" />
```

### Settings
 - Interval (int) : Interval to read sensors in second (optional, by default 10s)
 - PhotoResistorPin (int) : The photoresistor's pin (BCM mode)
 - PhotoResistorMaxValue (int) : The max value for the photo resistor sensor (optional, by default 30000)
 - EnablePhotoResistor (bool) : Enable the photo resistor sensor (optional, by default: true)
 - EnableTSL2561 (int) : Enable the TSL2561 sensor (optional, by default: true)

License
----

Apache License
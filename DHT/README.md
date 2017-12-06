# DHT Package for Constellation

This package pushes the temperature and humidity sensor to Constellation.

This package works with DHT11, DHT22 or AM2302 for Raspberry Pi.

### StateObjects
  - Temperature : current temerature in °C
  - Humidity : current relative humidity in %

### Installation

Before deploy this package on a Raspberry Pi :

  - Activate I2C
```
sudo raspi-config
```

 - Install dependencies
```
sudo apt-get install build-essential python-dev git python-pip RPi.GPIO
```

 - Install Adafruit_Python_DHT library
```
git clone https://github.com/adafruit/Adafruit_Python_DHT.git
cd Adafruit_Python_DHT
sudo python setup.py install
```

 - Declare the package in a RPi sentinel with the following configuration :
```xml
<package name="DHT">
  <settings>
    <setting key="SensorPin" value="4" />
    <setting key="SensorType" value="DHT22" />
  </settings>
</package>
```

### Settings
  - SensorPin (int) : The sensor's pin (BCM mode)
  - SensorType (string) : The sensor's type (type should be set to DHT11, DHT22, or AM2302)
  - Interval (int) : Interval to read sensors in second (optional, 1000ms by default)

License
----

Apache License
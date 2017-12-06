#!/usr/bin/env python
import Constellation
import Adafruit_DHT
import time
 
def Start():
    sensorType = Constellation.GetSetting("SensorType")
    if sensorType == "DHT11":
        sensor = Adafruit_DHT.DHT11
    elif sensorType == "DHT22":
        sensor = Adafruit_DHT.DHT22
    elif sensorType == "AM2302":
        sensor = Adafruit_DHT.AM2302
    else:
        Constellation.WriteError("Sensor type not supported ! Check your settings")
        return
    sensorPin = int(Constellation.GetSetting("SensorPin"))
    Constellation.WriteInfo("%s on #%d is ready !" % (sensorType, sensorPin))
    lastSend = 0
    while Constellation.IsRunning:
        ts = int(round(time.time()))
        if ts - lastSend >= int(Constellation.GetSetting("Interval")):
            humidity, temperature = Adafruit_DHT.read_retry(sensor, sensorPin)
            if humidity is not None and temperature is not None:
                Constellation.PushStateObject("Temperature", round(temperature, 2), lifetime = int(Constellation.GetSetting("Interval")) * 2)
                Constellation.PushStateObject("Humidity", round(humidity, 2), lifetime = int(Constellation.GetSetting("Interval")) * 2)
                lastSend = ts
            else:
                Constellation.WriteError("Failed to get reading")                
        time.sleep(1)

Constellation.Start(Start)
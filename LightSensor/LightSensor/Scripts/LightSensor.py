#!/usr/bin/env python
import Constellation
import RPi.GPIO as GPIO, time, os      

def OnExit():
    GPIO.cleanup()

def RCtime (RCpin):
    reading = 0
    GPIO.setup(RCpin, GPIO.OUT)
    GPIO.output(RCpin, GPIO.LOW)
    time.sleep(0.1) 
    GPIO.setup(RCpin, GPIO.IN)
    # This takes about 1 millisecond per loop cycle
    while (GPIO.input(RCpin) == GPIO.LOW):
        if (reading > int(Constellation.GetSetting("PhotoResistorMaxValue"))):
            break
        reading += 1
    return reading
 
def Start():
    GPIO.setmode(GPIO.BCM)
    Constellation.OnExitCallback = OnExit
    lastSend = 0
    currentValue = 0
    count = 0
    if(bool(Constellation.GetSetting("EnablePhotoResistor") == 'true')):
        Constellation.WriteInfo("LightSensor is ready !")
    while Constellation.IsRunning:
        if(bool(Constellation.GetSetting("EnablePhotoResistor") == 'true')):
            currentValue = currentValue + RCtime(int(Constellation.GetSetting("PhotoResistorPin")))
            count = count + 1
            ts = int(round(time.time()))
            if ts - lastSend >= int(Constellation.GetSetting("Interval")):
                avg = int(round(currentValue / count))
                Constellation.PushStateObject("Light", avg, lifetime = int(Constellation.GetSetting("Interval")) * 2)
                currentValue = 0
                count = 0
                lastSend = ts
        time.sleep(1)

Constellation.Start(Start)
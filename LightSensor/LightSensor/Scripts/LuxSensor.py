#!/usr/bin/env python
import Constellation
import os, re, subprocess, time, stat

# Const
EXECUTABLE_FILENAME = "GetTSL2561"

def DoMeasure():
    # Start process
    process = subprocess.Popen("./" + EXECUTABLE_FILENAME, stdout=subprocess.PIPE)
    # Reading  output
    for line in iter(process.stdout.readline, ''):
        # Parse line
        matchObj = re.match('RC: (\d*)\((.*)\), broadband: (\d*), ir: (\d*), lux: (\d*)', line)
        if matchObj:
            # Reading value
            returnCode = int(matchObj.group(1))
            broadband = int(matchObj.group(3))
            ir = int(matchObj.group(4))
            lux = int(matchObj.group(5))
            # Push StateObject
            if returnCode != 0:
                Constellation.WriteWarn("Unknow return code %s : %s" % (returnCode, line))
            else:
                Constellation.PushStateObject("Lux", { "Broadband": broadband, "IR" : ir, "Lux" : lux }, "LightSensor.Lux", lifetime = int(Constellation.GetSetting("Interval")) * 2)
        else:
            Constellation.WriteError("Unable to parse the output: %s" % line)

def Start():	
    # Make the "GetTSL2561" file as executable
    st = os.stat(EXECUTABLE_FILENAME)
    os.chmod(EXECUTABLE_FILENAME, st.st_mode | stat.S_IEXEC)
    # Describe StateObject
    Constellation.DescribeStateObjectType("LightSensor.Lux", "Lux data informations", [
        { 'Name':'Broadband', 'Type':'int' },
        { 'Name':'IR', 'Type':'int' },
        { 'Name':'Lux', 'Type':'int' }
    ])
    Constellation.DeclarePackageDescriptor()
    # Main loop
    if(bool(Constellation.GetSetting("EnableTSL2561") == 'true')):
        Constellation.WriteInfo("LuxSensor is ready !")
    lastSend = 0
    while Constellation.IsRunning:
        if(bool(Constellation.GetSetting("EnableTSL2561") == 'true')):
            ts = int(round(time.time()))
            if ts - lastSend >= int(Constellation.GetSetting("Interval")):
                DoMeasure()
                lastSend = ts
        time.sleep(1)

Constellation.Start(Start)
#!/usr/bin/env python
import Constellation
import os, subprocess, time, stat, json
from threading import RLock

# Const
EXECUTABLE_FILENAME = "modbus"

# Sync lock object
syncLock = RLock()

@Constellation.MessageCallback()
def ReadModbusRegister(registerAddress, registerCount, slaveId=1):
    '''
    Request input registers from the specified address

    :param int registerAddress: The Data Address of the first register requested
    :param int registerCount: The total number of registers requested
    :param int slaveId: The Slave Address [default: 1]
    :return list:List of register value
    '''
    debug = Constellation.GetSetting("ModbusDebug") == "true"
    # Create command
    cmdLine = ["./" + EXECUTABLE_FILENAME, str(slaveId), Constellation.GetSetting("Port"),  Constellation.GetSetting("BaudRate"), Constellation.GetSetting("PinDE"), Constellation.GetSetting("PinRE"), str(registerAddress), str(registerCount) ]
    if debug:
        cmdLine.append("debug")
        Constellation.WriteWarn("Executing command: %s" % cmdLine)
    # Execute command
    with syncLock:
        process = subprocess.Popen(cmdLine, stdout=subprocess.PIPE)
    # Reading output
    result = []
    for line in iter(process.stdout.readline, ''):
        if debug:
            Constellation.WriteWarn(line)
        if not debug or (debug and line.startswith("Result:")):
            result = (line[7:] if debug else line).split(';')
    # Return result
    return map(int, filter(None, result))

def Start():	
    # Make the "modbus" file as executable
    st = os.stat(EXECUTABLE_FILENAME)
    os.chmod(EXECUTABLE_FILENAME, st.st_mode | stat.S_IEXEC)
    # Loading devices configuration
    lastQuery = {}
    try:
        if Constellation.GetSetting("Devices"):
            for device in json.loads(Constellation.GetSetting("Devices")):
                Constellation.WriteInfo("Registering '%s' (interval: %d ms)" % (device["Name"], device["RequestInterval"]))
                lastQuery[device["Name"]] = 0
                typeDescriptor = []
                for prop in device["Properties"]:
                    typeDescriptor.append({ "Name": prop["Name"], "Type":prop["Type"] if "Type" in prop else "", "Description":prop["Description"] if "Description" in prop else "" })
                Constellation.DescribeStateObjectType("Modbus.%s" % device["StateObjectTypeName"], "%s's datas" % device["StateObjectTypeName"], typeDescriptor)
            Constellation.DeclarePackageDescriptor()
    except Exception, e:
        Constellation.WriteError("Error while loading the device's configuration : %s" % str(e))
    # Request loop
    while Constellation.IsRunning:
        try:
            if Constellation.GetSetting("Devices"):
                for device in json.loads(Constellation.GetSetting("Devices")):                
                    ts = int(round(time.time()))
                    if (ts - lastQuery[device["Name"]]) >= (device["RequestInterval"] / 1000):
                        lastQuery[device["Name"]] = ts
                        try:
                            registers = ReadModbusRegister(device["RegisterAddress"], device["RegistersCount"], device["SlaveID"])
                            result = {}
                            for prop in device["Properties"]:
                                try:
                                    result[prop["Name"]] = eval(prop["Selector"])
                                except Exception, e:
                                    Constellation.WriteError("Error on the property '%s' of '%s' : %s" % (prop["Name"], device["Name"], str(e)))
                            Constellation.PushStateObject(device["Name"], result, "Modbus.%s" % device["StateObjectTypeName"], lifetime=(device["RequestInterval"] / 1000) * 2, metadatas={ "SlaveID": device["SlaveID"] })
                        except Exception, e:
                            Constellation.WriteError("Error while requesting %s : %s" % (device["Name"], str(e)))
        except Exception, e:
            Constellation.WriteError("Error while reading configuration : %s" % str(e))
    time.sleep(1)

Constellation.Start(Start)
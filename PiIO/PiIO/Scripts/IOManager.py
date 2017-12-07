import Constellation
import RPi.GPIO as GPIO, time, json

DEFAULT_BOUNCE_TIME = 10

inputs = []
outputs = []

def onEventDetect(channel):
    input = [i for i in inputs if i['Pin'] == channel]
    if any(input):
        Constellation.PushStateObject(input[0]['Name'], bool(GPIO.input(input[0]['Pin'])), metadatas={ "Mode":"Input", "Pin":input[0]['Pin'] })

@Constellation.MessageCallback()
def DigitalWrite(name, value):
    '''
    Write a HIGH or a LOW value to a digital pin.

    :param string name: Name of the digital pin defined in the configuration
    :param bool value: The value applied to the digital pin
    '''
    output = [o for o in outputs if o['Name'] == name]
    if any(output):
        Constellation.WriteInfo("Setting #%d to %s" % (output[0]['Pin'], value))
        GPIO.output(output[0]['Pin'], value)
        Constellation.PushStateObject(output[0]['Name'], value, metadatas={ "Mode":"Output", "Pin":output[0]['Pin'] })   
    else:
        Constellation.WriteError("Unknown output !")

def OnExit():
    GPIO.cleanup()

def OnStart():
    global inputs
    global outputs
    Constellation.OnExitCallback = OnExit
    # Read the configuration
    mode = None
    try:
        config = json.loads(Constellation.GetSetting("Configuration"))        
        if config["PinMode"] == 'BCM':
            mode = GPIO.BCM
        elif config["PinMode"] == 'BOARD':
            mode = GPIO.BOARD
        else:
            Constellation.WriteError("Invalid pin mode. Should be set to BCM or BOARD")
            return
        inputs = config['Inputs']
        outputs = config['Outputs']
    except Exception, e:
        Constellation.WriteError("Error while reading the configuration : %s" % str(e))
        return
    # Init I/O
    GPIO.setmode(mode)
    Constellation.PushStateObject("Mode", "BCM" if mode == GPIO.BCM else "BOARD")
    for i in inputs:
        Constellation.WriteInfo("Setting pin #%d as input" % i['Pin'])
        GPIO.setup(i['Pin'], GPIO.IN) 
        GPIO.add_event_detect(i['Pin'], GPIO.BOTH, callback=onEventDetect, bouncetime=i['BounceTime'] if 'BounceTime' in i else config['BounceTime'] if 'BounceTime' in config else DEFAULT_BOUNCE_TIME)
        if 'Pull' in i:
            GPIO.setup(i['Pin'], GPIO.IN, pull_up_down= GPIO.PUD_UP if i['Pull'].lower() == "up" else GPIO.PUD_DOWN)
        else:
            GPIO.setup(i['Pin'], GPIO.IN)
        Constellation.PushStateObject(i['Name'], bool(GPIO.input(i['Pin'])), metadatas={ "Mode":"Input", "Pin":i['Pin'] })
    for o in outputs:
        Constellation.WriteInfo("Setting pin #%d as output" % o['Pin'])
        if 'InitialState' in o:
            GPIO.setup(o['Pin'], GPIO.OUT, initial=o['InitialState'])
        else:
            GPIO.setup(o['Pin'], GPIO.OUT)
        Constellation.PushStateObject(o['Name'], bool(GPIO.input(o['Pin'])), metadatas={ "Mode":"Output", "Pin":o['Pin'] })  
    # Ready!
    Constellation.WriteInfo("PiIO is ready with %d input(s) and %d output(s)" % (len(inputs), len(outputs)))

Constellation.Start(OnStart);
'''
 Gammu Package for Constellation
 Web site: http://www.myConstellation.io
 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 
 Licensed to Constellation under one or more contributor
 license agreements. Constellation licenses this file to you under
 the Apache License, Version 2.0 (the "License"); you may
 not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing,
 software distributed under the License is distributed on an
 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 KIND, either express or implied. See the License for the
 specific language governing permissions and limitations
 under the License.
'''

#!/usr/bin/env python
import Constellation, datetime, gammu, sys, time

# Gammu API : http://wammu.eu/docs/manual/python/gammu.html

global sm

def AttachIncomingEvents():
    # Set callback handler for incoming notifications
    sm.SetIncomingCallback(ProcessIncomingEvent)
    # Enable notifications from incoming SMS
    try:
        sm.SetIncomingSMS()
    except gammu.ERR_NOTSUPPORTED:
        Constellation.WriteWarn('Incoming SMS notification is not supported.')
    # Enable notifications from calls
    try:
        sm.SetIncomingCall()
    except gammu.ERR_NOTSUPPORTED:
         Constellation.WriteWarn('Incoming calls notification is not supported.')
    # Enable notifications from cell broadcast
    try:
        sm.SetIncomingCB()
    except gammu.ERR_NOTSUPPORTED:
         Constellation.WriteWarn('Incoming CB notification is not supported.')
    except gammu.ERR_SOURCENOTAVAILABLE:
         Constellation.WriteWarn('Cell broadcasts support not enabled in Gammu.')
    # Enable notifications for incoming USSD
    try:
        sm.SetIncomingUSSD()
    except gammu.ERR_NOTSUPPORTED:
        Constellation.WriteWarn('Incoming USSD notification is not supported.')

def ProcessIncomingEvent(sm, type, data):
    # type of action, one of Call, SMS, CB, USSD
    if type == "SMS":
        smss = sm.GetSMS(data["Folder"], data["Location"])
        for sms in smss:
            ProcessIncomingSMS(sms)
    elif type == "Call":
        ProcessIncomingCall(data)
    else:
        Constellation.WriteInfo("Received incoming event type %s, data: %s" % (type, str(data)))

@Constellation.MessageCallback()
def SendMessage(number, text):
    "Sends a text message to a specifed number"
    # Composition du SMS
    message = {
        'Text': text,
        'SMSC': {'Location': 1},
        'Number': number
    }
    # Envoi
    Constellation.WriteInfo("Sending '%s' to %s" % (text, number))
    sm.SendSMS(message)

@Constellation.MessageCallback()
def Call(number):
    "Calls the specifed number"
    Constellation.WriteInfo("Calling '%s'" % number)
    sm.DialVoice(number)

def ProcessIncomingSMS(sms):
    Constellation.WriteInfo("IncomingSMS from '%s' : %s" % (sms["Number"], sms["Text"]))
    Constellation.SendMessage(Constellation.GetSetting("IncomingEventGroupName"), "IncomingSMS", { 'Number': sms['Number'], 'Text' : sms['Text'] }, Constellation.MessageScope.group)
    sm.DeleteSMS(Location = sms['Location'], Folder = 0)

def ProcessIncomingCall(call):
    Constellation.WriteInfo("IncomingCall from '%s' (StatusCode: %s - CallID: %s)" % (call["Number"], call["StatusCode"], call["CallID"]))
    Constellation.SendMessage(Constellation.GetSetting("IncomingEventGroupName"), "IncomingCall", call['Number'], Constellation.MessageScope.group)

def DeleteAllSMS():
    status = sm.GetSMSStatus()
    remain = status['SIMUsed'] + status['PhoneUsed'] + status['TemplatesUsed']
    start = True
    while remain > 0:
        if start:
            sms = sm.GetNextSMS(Start = True, Folder = 0)
            start = False
        else:
            sms = sm.GetNextSMS(Location = sms[0]['Location'], Folder = 0)
        remain = remain - len(sms)
        Constellation.WriteInfo("Deleting SMS received at %s from '%s' : %s" % (str(sms[0]['DateTime']), sms[0]["Number"], sms[0]["Text"]))
        sm.DeleteSMS(Location = sms[0]['Location'], Folder = 0)

def Start():
    global sm
    # Init du GSM
    Constellation.WriteInfo("Initializing GSM")
    sm = gammu.StateMachine()
    sm.ReadConfig(Filename = Constellation.GetSetting("GammuConfigurationFilename"))
    sm.Init()
    # Deleting SMS
    DeleteAllSMS()
    # Attach to incoming events
    AttachIncomingEvents()
    # Ready
    Constellation.WriteInfo("GSM package is started")
    while Constellation.IsRunning:
        q = sm.GetSignalQuality()
        Constellation.PushStateObject("SignalQuality", q['SignalPercent'])
        time.sleep(10)

Constellation.Start(Start)
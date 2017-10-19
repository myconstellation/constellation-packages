'''
 Constellation Python proxy
 Version: 1.8.3.17264 (21 Sep 2017)
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

import inspect, json, sys, datetime, time, uuid, zmq
from collections import namedtuple
from enum import Enum
from threading import Thread

reload(sys)
sys.setdefaultencoding("utf-8")

class MessageScope(Enum):
    none = 0
    group = 1
    package = 2
    sentinel = 3
    others = 4
    all = 5

global IsRunning
global Settings
global OnExitCallback
global OnConnectionChanged
global OnLastStateObjectsReceived
global HasControlManager
global IsConnected
global IsStandAlone
global SentinelName
global PackageName
global PackageInstanceName
global PackageVersion
global PackageAssemblyVersion
global ConstellationClientVersion
global LastStateObjects

IsRunning = False
Settings = None
OnExitCallback = None
OnConnectionChanged = None
OnLastStateObjectsReceived = None
HasControlManager = None
IsConnected = None
IsStandAlone = None
SentinelName = None
PackageName = None
PackageInstanceName = None
PackageVersion = None
PackageAssemblyVersion = None
ConstellationClientVersion = None
LastStateObjects = None

_exitCode = None
_messageCallbacks = []
_stateObjectCallbacks = []
_messageCallbacksList = {}
_stateObjectCallbacksList = {}
_ctx = zmq.Context()
_socket = _ctx.socket(zmq.PAIR)
_socket.linger = 1000 #ms
_socket.connect("tcp://127.0.0.1:" + str(int(sys.argv[1])))
time.sleep(1.0)
_poller = zmq.Poller()
_poller.register(_socket, zmq.POLLIN)
_socket.send_string("Init")

def ConvertJsonToObject(data, tupleName = 'X', onTupleCreated = None):
    def _json_object_hook(d):
        tuple = namedtuple(tupleName, d.keys())
        if onTupleCreated:
            onTupleCreated(tuple)
        return tuple(*d.values()) 
    return json.loads(json.dumps(data) if isinstance(data, dict) else data, object_hook=_json_object_hook)

def MessageCallback(key = None, isHidden = False, returnType = None):
    def _registar(func):
        _messageCallbacksList[func.__name__] = { 'Func': func, 'Key' : key, 'DeclareCallback' : not isHidden, 'ReturnType' : returnType }
        return func
    return _registar

def StateObjectLink(sentinel = '*', package = '*', name = '*', type ='*'):
    def _registar(func):
        _stateObjectCallbacksList[func.__name__] = { 'Func': func, 'Sentinel' : sentinel, 'Package' : package, 'Name' : name, 'Type' : type }
        return func
    return _registar

def DeclarePackageDescriptor():
    _socket.send_json({ 'Function' : 'DeclarePackageDescriptor' })

def WriteInfo(msg):
    WriteLog('Info', msg)

def WriteWarn(msg):
    WriteLog('Warn', msg)

def WriteError(msg):
    WriteLog('Error', msg)

def WriteLog(level, msg):
    _socket.send_json({ 'Function' : 'WriteLog', 'Level' : level, 'Message' : str(msg).encode() })

def PushStateObject(name, value, type = "", metadatas = {}, lifetime = 0):
    _socket.send_json({ 'Function' : 'PushStateObject', 'Name': str(name), 'Value': value, 'Type': str(type), 'Metadatas' : metadatas, 'Lifetime' : lifetime })

def SendMessage(to, key, value, scope = MessageScope.package):
    _socket.send_json({ 'Function' : 'SendMessage', 'Scope':  scope.value, 'To' : str(to), 'Key': str(key), 'Value' : value, 'SagaId' : '' })

def SendMessageWithSaga(callback, to, key, value, scope = MessageScope.package):
    sagaId = str(uuid.uuid1())
    def _msgCallback(k, context, data):
        if(k == "__Response" and context.SagaId == sagaId):
            if not data:
                callback(context) if (callback.func_code.co_argcount > 0 and callback.func_code.co_varnames[callback.func_code.co_argcount - 1] == 'context') else callback()
            else:
                if isinstance(data, list):
                    if (callback.func_code.co_argcount > 0 and callback.func_code.co_varnames[callback.func_code.co_argcount - 1] == 'context'):
                        data.append(context)
                    callback(*data)
                else:
                    callback(data, context) if (callback.func_code.co_argcount > 0 and callback.func_code.co_varnames[callback.func_code.co_argcount - 1] == 'context') else callback(data)
            _messageCallbacks.remove(_msgCallback)
    _messageCallbacks.append(_msgCallback)
    _socket.send_json({ 'Function' : 'SendMessage', 'Scope':  scope.value, 'To' : str(to), 'Key': str(key), 'Value' : value, 'SagaId' : sagaId })

def SubscribeMessages(group):
    _socket.send_json({ 'Function' : 'SubscribeMessages', 'Group' : str(group) })

def UnSubscribeMessages(group):
    _socket.send_json({ 'Function' : 'UnSubscribeMessages', 'Group' : str(group) })
    
def RefreshSettings():    
    _socket.send_json({ 'Function' : 'GetSettings' })

def RequestStateObjects(sentinel = '*', package = '*', name = '*', type ='*'):    
    _socket.send_json({ 'Function' : 'RequestStateObjects', 'Sentinel' : sentinel, 'Package' : package, 'Name' : name, 'Type' : type })

def SubscribeStateObjects(sentinel = '*', package = '*', name = '*', type ='*'):    
    _socket.send_json({ 'Function' : 'SubscribeStateObjects', 'Sentinel' : sentinel, 'Package' : package, 'Name' : name, 'Type' : type })

def PurgeStateObjects(name = '*', type ='*'):    
    _socket.send_json({ 'Function' : 'PurgeStateObjects', 'Name' : name, 'Type' : type })

def RegisterStateObjectLinks():
    for key in _stateObjectCallbacksList:
        soLink = _stateObjectCallbacksList[key]
        RegisterStateObjectCallback(soLink['Func'], soLink['Sentinel'], soLink['Package'], soLink['Name'], soLink['Type'])

def RegisterStateObjectCallback(func, sentinel = '*', package = '*', name = '*', type ='*', request = True, subscribe = True):
    def _soCallback(stateobject):
        if((sentinel == stateobject.SentinelName or sentinel == '*') and (package == stateobject.PackageName or package == '*') and (name == stateobject.Name or name == '*') and (type == stateobject.Type or type == '*')):
            func(stateobject)
    _stateObjectCallbacks.append(_soCallback)
    if request == True:
        RequestStateObjects(sentinel, package, name, type)
    if subscribe == True:
        SubscribeStateObjects(sentinel, package, name, type)

def GetSetting(key):
    if key in Settings:
        return Settings[key]
    else:
        return None

def RegisterMessageCallbacks():
    for key in _messageCallbacksList:
        func = _messageCallbacksList[key]['Func']
        RegisterMessageCallback(_messageCallbacksList[key]['Key'] if _messageCallbacksList[key]['Key'] else func.__name__, func, _messageCallbacksList[key]['DeclareCallback'], str(func.__doc__) if func.__doc__ else '', _messageCallbacksList[key]['ReturnType'])

def RegisterMessageCallback(key, func, declareCallback = False, description = '', returnType = None):
    def _msgCallback(k, context, data):
        if(k == key):
            returnValue = None
            if not data:
                returnValue = func(context) if (func.func_code.co_argcount > 0 and func.func_code.co_varnames[func.func_code.co_argcount - 1] == 'context') else func()
            else:
                if isinstance(data, list):
                    if (func.func_code.co_argcount > 0 and func.func_code.co_varnames[func.func_code.co_argcount - 1] == 'context'):
                        data.append(context)
                    returnValue = func(*data)
                else:
                    returnValue = func(data, context) if (func.func_code.co_argcount > 0 and func.func_code.co_varnames[func.func_code.co_argcount - 1] == 'context') else func(data)
            if context.IsSaga and returnValue <> None:
                SendResponse(context, returnValue)
    _socket.send_json({ 'Function' : 'RegisterMessageCallback', 'Key' : str(key), "DeclareCallback": bool(declareCallback), 'Description' : str(description), 'Arguments' : inspect.getargspec(func).args , 'ReturnType' : returnType if returnType else '' })
    _messageCallbacks.append(_msgCallback)

def SendResponse(context, value):
    if not context:    
        WriteError("Invalid context")
    elif not context.IsSaga:
        WriteError("No Saga on this context")
    else:
       _socket.send_json({ 'Function' : 'SendMessage', 'Scope':  MessageScope.package.value, 'To' : context.Sender.ConnectionId if context.Sender.Type == 0 else context.Sender.FriendlyName, 'Key': '__Response', 'Value' : value, 'SagaId' : context.SagaId })

def Shutdown():
    _socket.send_json({ 'Function' : 'Shutdown' })

def _onReceiveMessage(key, context, data):
    try:
        for mc in _messageCallbacks:
            mc(key, context, data)
    except Exception, e:
        WriteError("Error while dispatching message '%s': %s" % (key, str(e)))

def _onStateObjectUpdated(stateObject):
    try:
        for callback in _stateObjectCallbacks:
            callback(stateObject)
    except Exception, e:
        WriteError("Error while dispatching StateObject : %s" % str(e))

def _exit(exitCode = 0):
    global IsRunning
    global _exitCode
    WriteWarn("Exiting %s with code:%s" % (sys.argv[0], exitCode))
    if OnExitCallback:
        try:
            OnExitCallback(exitCode)
        except:
            pass
    time.sleep(0.1) 
    if _exitCode == 0:
        _exitCode = exitCode
        IsRunning = False
    else:
        IsRunning = False
        sys.exit(exitCode)

def _dispatcherMessage():
    global Settings
    global HasControlManager
    global IsConnected
    global IsStandAlone
    global SentinelName
    global PackageName
    global PackageInstanceName
    global PackageVersion
    global PackageAssemblyVersion
    global ConstellationClientVersion
    global LastStateObjects
    lastPing = datetime.datetime.now()
    while IsRunning:
        try:
            socks = dict(_poller.poll(1000))
            if ((datetime.datetime.now() > lastPing) and (datetime.datetime.now() - lastPing).seconds >= 30):
                WriteError("No ping from the Package Host since %s seconds" % (datetime.datetime.now() - lastPing).seconds)
                _exit(2)                
                break
            if socks:
                message = _socket.recv_json()
                if message['Type'] == "PING":
                    lastPing = datetime.datetime.now()
                    _socket.send_string("PONG")
                elif message['Type'] == "PACKAGESTATE":
                    HasControlManager = message['HasControlManager']
                    IsConnected = message['IsConnected']
                    IsStandAlone = message['IsStandAlone']
                    SentinelName = message['SentinelName']
                    PackageName = message['PackageName']
                    PackageInstanceName = message['PackageInstanceName']
                    PackageVersion = message['PackageVersion']
                    PackageAssemblyVersion = message['PackageAssemblyVersion']
                    ConstellationClientVersion = message['ConstellationClientVersion']
                elif message['Type'] == "LASTSTATEOBJECTS":
                    LastStateObjects = []
                    for so in message['StateObjects']:
                        LastStateObjects.append(ConvertJsonToObject(so, 'StateObject'))
                    if OnLastStateObjectsReceived:
                        try:
                            OnLastStateObjectsReceived()
                        except Exception, e:
                            WriteError("Error while invoking the OnLastStateObjectsReceived event handler : %s" % str(e))
                elif message['Type'] == "CONNECTIONSTATE":
                    IsConnected = message['IsConnected']
                    if OnConnectionChanged:
                        try:
                            OnConnectionChanged()
                        except Exception, e:
                            WriteError("Error while invoking the OnConnectionChanged event handler : %s" % str(e))
                elif message['Type'] == "MSG":
                    def _addSendResponse(tuple):
                        tuple.SendResponse = lambda ctx, rsp: SendResponse(ctx, rsp)
                    context = ConvertJsonToObject(message['Context'], 'MessageContext', _addSendResponse)
                    if 'Data' in message:
                        try:
                            data = ConvertJsonToObject(message['Data'])
                        except:
                            data = message['Data']
                        _onReceiveMessage(message['Key'], context,  data)
                    else:
                        _onReceiveMessage(message['Key'], context, "")
                elif message['Type'] == "SETTINGS":
                    Settings = message['Settings']
                elif message['Type'] == "STATEOBJECT":
                    try:                        
                        so = ConvertJsonToObject(message['StateObject'], 'StateObject')
                    except:
                        WriteError("Unable to deserialize the StateObject")
                    _onStateObjectUpdated(so)
                elif message['Type'] == "EXIT":
                    _exit(0)
                    break
        except Exception, e:
            WriteError("Internal loop error : %s" % str(e))

def Start(onStart = None):
    global _exitCode
    _exitCode = 0
    StartAsync()
    if onStart:
        msgCb = len(_messageCallbacks)
        try:
            onStart()
        except Exception, e:
            WriteError("Fatal error: %s" % str(e))
            _exit(1)
        if len(_messageCallbacks) > msgCb:
            DeclarePackageDescriptor()
    while IsRunning:
        time.sleep(0.1)
    if _exitCode:
        sys.exit(_exitCode)

def StartAsync():
    global IsRunning
    RegisterStateObjectLinks()
    RegisterMessageCallbacks()
    if len(_messageCallbacks) > 0:
        DeclarePackageDescriptor()
    t1 = Thread(target = _dispatcherMessage)
    t1.setDaemon(True)
    IsRunning = True
    t1.start()
    RefreshSettings()
    while IsRunning and Settings is None:
        time.sleep(1.0)
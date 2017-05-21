# Gammu Package for Constellation

This package can receive and send SMS the Python-Gammu. You can also call a number or receive incoming call but without voice processing !

### StateObjects
  - SignalQuality : GSM signal quality

### MessageCallbacks
  - Call(number) : Calls the specifed number
  - SendMessage(number, text) : Sends a text message to a specifed number

The package send the Incoming Call & SMS to the group defined with the setting "IncomingEventGroupName".

### Installation

First install gammu, for example :
```
apt-get install gammu
```

Then configure gammu. See [https://wammu.eu/docs/manual/quick/index.html#quick](https://wammu.eu/docs/manual/quick/index.html#quick)

Now install python-gammu module :
```
pip install python-gammu
```

And declare the package in a Sentinel with the following configuration :
```xml
<package name="Gammu" />
```

License
----

Apache License
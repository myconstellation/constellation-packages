# Digital I/O manager for Raspberry Pi

This package is an interface to the I/O of a Raspberry Pi for Constellation.

The state of each input / output is synchronized in State Objects and you have a MessageCallback to drive outputs from anything !

### MessageCallbacks
  - DigitalWrite(name, value):  Write a HIGH or a LOW value to a digital pin by specify the name of the digital pin defined in the configuration and the the value applied to the digital pin (a boolean)

### StateObjects
  - The state (boolean) of each digital inputs defined in the configuration
  - The state (boolean) of each digital outputs defined in the configuration

### Installation

Declare the package in a Sentinel with the following configuration (as example) :
```xml
<package name="PiIO">
  <settings>
    <setting key="Configuration">
      <content>
        {
          "PinMode": "BCM",
          "Inputs": [{
              "Name": "Button1",
              "Pin": 4
          }, {
              "Name": "Button2",
              "Pin": 25,
              "Pull":"Down"
          }, {
              "Name": "Button3",
              "Pin": 27,
              "Pull":"Up"
          }],
          "Outputs": [{
              "Name": "Relay1",
              "Pin": 17
          }, {
              "Name": "Relay2",
              "Pin": 18,
              "InitialState": true
          }]
        }
      </content>
    </setting>
  </settings>
</package>
```

### Configuration

All digitals I/O must be declare in the JSON configuration under the setting named "Configuration".

The JSON have 3 properties :
  - PinMode : must be "BCM" or "BOARD" (More information about the pin numbering : https://sourceforge.net/p/raspberry-gpio-python/wiki/BasicUsage/)
  - Inputs : an array of Digital Inputs
  - Outputs : an array of Digital Outputs

#### Inputs

An input have 3 properties :
  - Name : the name (id) of the input as string (must be unique)
  - Pin : the pin number of the input (int)
  - Pull : can be "Up" or "Down" to configure the input as PullUp or PullDown. This property is optional. (for more information : https://sourceforge.net/p/raspberry-gpio-python/wiki/Inputs/)

#### Outputs

An outputs have 3 properties too :
  - Name : the name (id) of the output as string (must be unique)
  - Pin : the pin number of the output (int)
  - InitialState : the initial state (an boolean) of the output. This property is optional. (for more information : https://sourceforge.net/p/raspberry-gpio-python/wiki/Outputs/)

#### Example

The I/O configuration given as an example above is :
```json
{
  "PinMode": "BCM",
  "Inputs": [{
      "Name": "Button1",
      "Pin": 4
  }, {
      "Name": "Button2",
      "Pin": 25,
      "Pull":"Down"
  }, {
      "Name": "Button3",
      "Pin": 27,
      "Pull":"Up"
  }],
  "Outputs": [{
      "Name": "Relay1",
      "Pin": 17
  }, {
      "Name": "Relay2",
      "Pin": 18,
      "InitialState": true
  }]
}
```

Explanations
 - We use the BCM pin numbering
 - We defined 3 inputs :
   - "Button1" on the pin #4 (by default as pull-up)
   - "Button2" on the pin #25 as pull-down
   - "Button3" on the pin #27 as pull-up
 - We defined 2 outputs
   - Relay1 on the pin #17
   - Relay2 on the pin #18 (when the package started, this output is set to HIGH because the InitialState = true)

Now, we have 5 StateObjects in our Constellation (one by I/O) and we can invoke the MessageCallback "DigitalWrite" to drive out 2 outputs from anything !

License
----

Apache License
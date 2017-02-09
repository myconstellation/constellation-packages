# Panasonic TV Package for Constellation

Package to control Jeedom. You can send a command by ID or control scene by ID.

### SceneControl
 - Start : Start the scene
 - Stop : Stop the scene
 - Activer : Activate the scene
 - Desactiver : Desactivate the scene

 ### SendCommand
 - Switch : Trigger basic command without value
 - Slider : Set slide according to value
 - Message : Set title according to value and message according to value2
 - Color : Set color according to value

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
          <package name="Jeedom" enable="true">
            <settings>
              <setting key="ServerUrl" value="IP Adress of Jeedom" />
              <setting key="ApiKey" value="API Key of Jeedom" />
            </settings>
          </package>
        </packages>
```

### Settings
 - ServerUrl (string - required) : IP adress of Jeedom without http but with /jeedom if needed
 - ApiKey (string - required) : Api key of Jeedom

License
----

Apache License

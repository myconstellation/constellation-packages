# Constellation Package for Nest

This package connect your Nest devices into your Constellation.

### StateObjects

All your Nest structures, thermostats, smoke detectors and cameras are pushed as StateObjects with all detailled informations.

### MessageCallbacks
  - SetAwayMode(string structureId, bool isAway) : Sets the away mode for the specified structure.
  - SetTemperatureScale(string thermostatId, bool isFahrenheit) : Sets the temperature scale (C/F) on a thermostat
  - SetTargetTemperature(string thermostatId, double temperature) : Sets the target temperature for for the specified thermostat
  - SetThermostatLabel(string thermostatId, string label) : Sets the thermostat label to give it a customized nickname
  - SetCameraStreamingStatus(string cameraId, bool isStreaming) : Set the streaming status on a camera
  - SetProperty(string path, string propertyName, object value) : Sets the property for a specified path

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="Nest">
  <settings>
	<setting key="AccessToken" value="<access key>" />    
  </settings>
</package>
```
To create the Access Token :

* Go to https://developer.nest.com/ and sign-in
* Create a new product named "Constellation" for example
* Check the permission you want to have on Constellation (Thermostat R/W, Away R/W, Smoke detector, etc..)
* Once your product is created, run the `GetNestAccessToken.ps1` commandlet. You will prompt the client ID & secret of your product.
* A Web page is opened to approve the product then display a PIN-base auth code 
* Copy this PIN auth code to the Powerhell script and you will have the Access Token directly in your clipboard !

License
----

Apache License

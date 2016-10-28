# ForecastIO Package for Constellation

Global weather service for Constellation.

### StateObjects
  - Each station registred in the setting are push as StateObjects.

### MessageCallbacks
  - GetWeatherForecast(longitude, latitude) : Gets the weather forecast for a given GPS location.

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="ForecastIO">
  <settings>
    <setting key="forecastIOConfigurationSection">
        <content>
        <forecastIOConfigurationSection xmlns="urn:ForecastIO" apiKey="xxxxxxxxxxxxxxxxx" refreshInterval="00:30:00">
          <stations>
            <station name="Paris" longitude="2.35" latitude="48.853" />
            <station name="Lille" longitude="3.05" latitude="50.629" />
            <station name="London" longitude="-0.14" latitude="51.557" />
          </stations>
        </forecastIOConfigurationSection>
        </content>
    </setting>
  </settings>
</package>
```
License
----

Apache License
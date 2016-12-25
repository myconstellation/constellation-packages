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

You can also set the desired language and the requested units with the attributes `language` and `unit` :

```xml
<forecastIOConfigurationSection xmlns="urn:ForecastIO" apiKey="xxxxxxxxxxxxxxxxx" refreshInterval="00:30:00" unit="si" language="fr">
```

`language` may be:

* ar: Arabic
* az: Azerbaijani
* be: Belarusian
* bs: Bosnian
* ca: Catalan
* cs: Czech
* de: German
* el: Greek
* en: English (which is the default)
* es: Spanish
* et: Estonian
* fr: French
* hr: Croatian
* hu: Hungarian
* id: Indonesian
* it: Italian
* is: Icelandic
* kw: Cornish
* nb: Norwegian Bokmal
* nl: Dutch
* pl: Polish
* pt: Portuguese
* ru: Russian
* sk: Slovak
* sl: Slovenian
* sr: Serbian
* sv: Swedish
* tet: Tetum
* tr: Turkish
* uk: Ukrainian
* x-pig-latin: Igpay Atinlay
* zh: simplified Chinese
* zh-tw: traditional Chinese

`units` should be one of the following:

* auto: automatically select units based on geographic location
* ca: same as si, except that windSpeed is in kilometers per hour
* uk2: same as si, except that nearestStormDistance and visibility are in miles and windSpeed is in miles per hour
* us: Imperial units (the default)
* si: SI units


License
----

Apache License
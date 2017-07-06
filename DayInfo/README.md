# DayInfo Package for Constellation

Get the name day (in French) and sunset/sunrise informations.

### StateObjects
  - NameDay : the name day for the current day
  - SunInfo : sunset/sunrise informations for the current day
  - Date : the date of the current day (jour numéro mois)
  - Time : current time (HH:mm)

### MessageCallbacks
  - GetNameDay : Gets the name day for the given day
  - GetSunInfo : Calculate the Universal Coordinated Time (UTC) of sunset and sunrise for the given day at the given location on earth

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="DayInfo>
  <settings>
    <setting key="TimeZone" value="1" />
    <setting key="Latitude" value="50,4130" />
    <setting key="Longitude" value="3,0852" />
  </settings>
</package>
```
License
----

Apache License
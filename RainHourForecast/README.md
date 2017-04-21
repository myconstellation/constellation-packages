# Constellation Package for Meteo France rain hour forecast

This package get the rain forecast for the next hour from Meteo France.

Note : to get rain forecast for a town, you need his id.

### MessageCallbacks
  - FindId(Postal_Code) : Get id for a specify postal code. If multiple towns share the same postal code, you get multiple id.
  - RainForecast(Town_ID) : Get rain forecast for the next hour for the specify town's id.

### StateObjects

This package create one StateObject by town's id in settings. Each StateObject have the town's id as name.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="RainHourForecast">
	<settings>
		<setting key="Ville" value="751110" />
	</settings>
</package>
```

You can specify multiple towns i.e. :

```<setting key="Ville" value="751120,751110" />```

By default the package queries the rain forecast every 1 hour but you can customize this interval (the value is in minutes) :

```<setting key="RefreshInterval" value="60" />```

By default the package use two url from meteo france to get town's id and town's rain forecast :

```
	<setting key="IdUrl" value="http://www.meteofrance.com/mf3-rpc-portlet/rest/lieu/facet/pluie/search/{0}" />
	<setting key="ForecastUrl" value="http://www.meteofrance.com/mf3-rpc-portlet/rest/pluie/{0}" />
```
----

Apache License
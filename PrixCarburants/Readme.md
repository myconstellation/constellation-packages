# PrixCarburants Package for Constellation

This package get fuel prices in EUR of http://prix-carburants.gouv.fr/ and pushes it to Constellation
See the complete OpenData specification in https://www.prix-carburants.gouv.fr/rubrique/opendata/

### Message Callbacks
* BestPriceInArea (lat, lon, fuel, range) : Get the cheapest gas station for a fuel type in a given radius
* FindInArea(lat, lon, range) : Find all Gas station in a given radius
* GetPrice(id, fuel) : Get price of a fuel type in a station

The "fuel" parameter accepts the following values : Gazole, SP95, SP98, GPLc, E10, E85

### StateObjects
* {StationId} : informations about a specific gas station
* Cheapest-{FuelName} : cheapest station for the specified fuel type
* InArea : all the stations in the specified area


### Settings
```xml
	<Setting name="station-ids" type="String" description="ID of the gas stations (separated by commas)" isRequired="false" />
	<Setting name="interval" type="Int32" description="Refresh interval (in hours)" isRequired="false" defaultValue="6" />
	<Setting name="longitude" type="Double" description="Longitude of the place you want to get the prices" isRequired="false" defaultValue="" />
	<Setting name="latitude" type="Double" description="Latitude of the place you want to get the prices" isRequired="false" defaultValue="" />
	<Setting name="range" type="Int32" description="Range (in meters) around the place you want to get the prices" isRequired="false" defaultValue="1000" />
	<Setting name="cheapest-fuel-types" type="String" description="Types of the cheapest fuel to find within the specified coordinates and range (possible values : Gazole, SP95, SP98, GPLc, E10, E85, separated by commas)" />
```

By default the package queries the api every 6 hours but you can customize this interval.

Icon from https://openclipart.org/detail/74419/petrolpump

License
----

Apache License

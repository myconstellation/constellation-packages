# Pollution package for Constellation

This package get data about pollution via https://aqicn.org/.
Get your api key from https://aqicn.org/data-platform/token/#/
Le package met à jours le SO «Pollution Ip» toutes les trois heures.

### MessageCallbacks
- RecupererPollutionVille(string ville) : 
	- Get pollution from your city.
	- Créé un SO «Pollution {votreVille}»
	
- RecupererPollutionIp() : 
	- Get pollution from where you server is.
	- Créé un SO «Pollution Ip»

- RecupererPollutionGeoloc(latitude, longitude) : 
	- Get pollution by GPS coordinates.
	- Créé un SO «Position Geoloc»

- RecupererStationsGeoloc(latitudeZone1, longitudeZone1, latitudeZone2, longitudeZone2) : 
	- Get pollution for a geolocated zone.
	- Créé un SO «Stations Geoloc»

- RecupererStationsParNom(keyword) : 
	- Get pollution by station name.
	- Créé un SO «Stations Nom»
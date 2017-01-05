# Constellation Package for Squeezebox

This package get informations (stations, traffic and schedules) from RATP (thank to Pierre Grimaud  - https://github.com/pgrimaud).

### MessageCallbacks
  - GetStations(LineType type, int ligneId) : get stations for a line
  - GetSchedule(LineType type, string ligneId, string stationId, string destinationId) : get the schedule.
  - GetTraffic(LineType type, string ligneId) : get the current traffic.

License
----

Apache License

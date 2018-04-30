# BetaSeries Package for Constellation

Synchronize your favorite TV-shows with BetaSeries

### StateObjects
  - Planning : the planning

### MessageCallbacks
  - GetPlanning : Gets the planning.
  - MarkEpisodeAsSeen : Marks an episode from the planning as seen.

### Settings

Declare the package in a Sentinel with the following configuration :
	- DeveloperKey : your developer key, get yours on [BetaSeries API](https://www.betaseries.com/api/)
    - Login : your BetaSeries user login
    - Password : your BetaSeries user password
	- PlanningPullInterval: the interval between two automatic planning retrieval (optional, default: 60 minutes)

### License
----

Apache License
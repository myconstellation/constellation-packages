# PoolCop package for Constellation

Connect your swimming pool to Constellation with PoolCop by using the PoolCopilot API or the PoolCop local service

More information on https://www.poolcop.com/

### StateObjects
  - API : PoolCop API informations (expiration date, remaining calls, ID, level, etc...)
  - Pool : Pool informations (name, location, timezone, image, etc...)
  - PoolCop : PoolCop information (temperatures, pH, ORP, Ioniser, ioniser, water level, equipment configuration and status, auxiliaries, timers, history, alerts, settings, etc...)

### MessageCallbacks
  - ClearAlarm() : Clears alarm
  - SwitchAuxiliary(int auxId) : Switches the auxiliary state
  - SwitchPumpState() : Switches the pump state

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="PoolCop">
  <settings>
    <setting key="PoolCopilotAPISecretKey" value="xxxxxxxxxxxxxxxxx" />
    <setting key="PoolCopilotAPILanguage" value="fr" />
    <setting key="Interval" value="30" />
  </settings>
</package>
```

Only the "PoolCopilotAPISecretKey" setting is required, others are optional :
* PoolCopilotAPILanguage : the language used for alerts and other message from the API (supported: 'en', 'fr', 'de' and 'es')
* Interval : the Polling interval in second (30sec by default)

License
----

Apache License
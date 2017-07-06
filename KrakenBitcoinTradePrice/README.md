# KrakenBitcoinTradePrice Package for Constellation

This package get Bitcoin trade price in EUR of https://www.kraken.com/ and pushes it to Constellation

### StateObjects
  - KrakenBitcoinTradePrice

By default the package queries the api every 5 minutes but you can customize this interval :
```xml
<setting key="Interval" value="5" />
```

License
----

Apache License
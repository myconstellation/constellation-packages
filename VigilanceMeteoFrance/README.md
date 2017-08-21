# Constellation Package for Meteo France vigilance

This package get the vigilances for departements from Meteo France.

### MessageCallbacks
  - GetVigilance(Departement) : Get vigilances for a departement.

### StateObjects

This package create one StateObject by departements' number in settings. Each StateObject have the departement's number as name.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="VigilanceMeteoFrance">
    <settings>
        <setting key="Departements">
            <content>
                <departements xmlns="http://schemas.myconstellation.io/Constellation/1.8/PackageManifest">
                    <departement number="75" />
                    <departement number="29" />
                </departements>
            </content>
        </setting>
    </settings>
</package>
```

By default the package queries the vigilances every 1 hour but you can customize this interval (the value is in minutes) :

```<setting key="RefreshInterval" value="60" />```

By default the package use url from meteo france to get vigilances :

```xml
	<setting key="Url" value="http://vigilance.meteofrance.com/data/NXFR33_LFPW_.xml" />
```
----

Apache License
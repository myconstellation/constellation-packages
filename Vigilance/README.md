# Constellation Package for Meteo France Vigilance

This package get Vigilance informations of a departement from Meteo France.

Note : Departement must be two digit number (for example : 04)

### MessageCallbacks
  - GetVigilance(Departement) : get Vigilance informations for this departement. Number can be comma separated (for example : 04,77).

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="Vigilance">
	<settings>
		<setting key="RefreshInterval" value="60">
		<setting key="Departement" value="01,02">
	</settings>
</package>
```
License
----

Apache License
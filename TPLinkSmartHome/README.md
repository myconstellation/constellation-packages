---
Package: TPLinkSmartHome
Tags: TPLink, Plug
---

# TPLinkSmartHome : Prises et autres périphériques connectés

Le package TPLinkSmartHome vous permet de contrôler et monitorer les périphériques TPLink Smart Home (kasa smarthome).
**ATTENTION, pour le moment seules les prises HS100 et HS110 sont prises en charge par ce package.**

## Installation
Depuis le “Online Package Repository” de votre Console Constellation, installez et déployez le package sur la sentinelle de votre choix (compatible Windows et Linux).

Sur la page de Settings, vous devez obligatoirement définir un ou plusieurs périphériques.

Par exemple : 
```json
	[{
        "HostName": "192.168.0.123",
        "Type": "PlugWithEnergyMeter"
        }, {
        "HostName": "192.168.0.234",
        "Type": "Plug"
    }]
```

Vous pouvez également déployer ce package manuellement dans la configuration de votre Constellation :
```xml
<package name="TPLinkSmartHome">
	<settings>
		<setting key="poolingInterval" value="2000" />
        <setting key="devices">
            <content>[{
			"HostName": "192.168.0.123",
			"Type": "PlugWithEnergyMeter"
		}, {
			"HostName": "192.168.0.234",
			"Type": "PlugWithEnergyMeter"
		}]</content>
		</setting>
	</settings>
</package>
```

## Settings

| Nom | Type | Requis ? | Description du Setting |
| --- | ---- | -------- | ---------------------  |
| devices | JsonObject | OUI | liste de périphériques |
| poolingInterval | int | NON | interval en millisecondes entre deux récupération de l'état des périphériques (défaut : 2000ms) |

## StateObjects
Vous retrouverez 1 StateObject publié par périphérique par le package :

| Nom | Type | Description |
| --- | ---- | ----------- |
| TPLink-##Hostname## | JsonObject | les informations d'un périphérique |

## MessageCallbacks
Le package expose 3 MessageCallbacks :

| Signature | Réponse (saga) | Description |
| --------- | -------------- | ----------- |
| GetDailyStat(string hostname, int? year = null, int? month = null) | JsonObject | Les statistiques d'utilisation quotidiennes pour une année et un mois donné (Pour les prises HS110 uniquement) |
| GetMonthStat(string hostname, int? year = null) | JsonObject | Les statistiques d'utilisation mensuelles pour une année donnée (Pour les prises HS110 uniquement) |
| SetOutputState(string hostname, bool state) | void | Change l'état (ON/OFF) d'une prise (Pour les prises HS100 et HS110 uniquement) |
| SetLedOff(string hostname, bool isOff) | Object | Change l'état (ON/OFF) de la LED d'une prise (Pour les prises HS100 et HS110 uniquement) |
  
## Quelques exemples
* Surveiller la consommation d'une prise connectée
* Contrôler à distance une prise connectée, éteindre la nuit certaines prises etc.
  
License
----

Apache License

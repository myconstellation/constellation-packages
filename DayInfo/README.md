---
Package: DayInfo
Tags: Sun, Soleil, Fete
---
# DayInfo : heures de lever/coucher du soleil et fête du jour

Le package DayInfo vous permet de connaitre la fête du jour ainsi que les heures du lever et coucher du soleil pour une position GPS donnée.

## Installation
Depuis le “Online Package Repository” de votre Console Constellation, installez et déployez le package sur la sentinelle de votre choix (compatible Windows et Linux).

Sur la page de Settings, vous devez obligatoirement définir une position GPS (latitude et longitude) ainsi que le “timezone”.

Par exemple pour Paris:
* Timezone : 1
* Latitude : 48.866667
* Longitude : 2.333333

Pour avoir un calcul des horaires du soleil plus précis, entrez votre position GPS exacte. Pour connaitre la position GPS précise pour une adresse donnée : http://www.coordonnees-gps.fr/

Vous pouvez également déployer ce package manuellement dans la configuration de votre Constellation :
```xml
<package name="DayInfo>
  <settings>
    <setting key="TimeZone" value="1" />
    <setting key="Latitude" value="50,4130" />
    <setting key="Longitude" value="3,0852" />
  </settings>
</package>
```

## Settings

| Nom | Type | Requis ? | Description du Setting |
| --- | ---- | -------- | ---------------------  |
| TimeZone | Int32 | OUI | Timezone de votre position (ex: 1 pour la France) |
| Latitude | Double | OUI | Latitude GPS de votre position |
| Longitude | Double | OUI | Longitude GPS de votre position |

## StateObjects
Vous retrouverez 2 StateObjects publiés chaque jour par le package :

| Nom | Type | Description |
| --- | ---- | ----------- |
| NameDay | String | La fête du jour |
| SunInfo | Objet SunInfo | Information sur les horaires du soleil pour votre position GPS |

## MessageCallbacks
Le package expose 2 MessageCallbacks :

| Signature | Réponse (saga) | Description |
| --------- | -------------- | ----------- |
| GetNameDay(Date date) | String | La fête pour le jour spécifié |
| GetSunInfo(Date date, int timezone, double latitude, double longitude) | Objet SunInfo | Information sur les horaires du soleil pour le jour spécifié |
  
## Quelques exemples
* Exploiter les horaires du soleil dans vos algorithmes C# pour gérer les volets ou les éclairages
* Afficher la fête jour dans vos Dashboards HTML ou WPF
  
License
----

Apache License
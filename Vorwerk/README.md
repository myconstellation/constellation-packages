---
Package: Vorwerk
Tags: Vorwerk, Neato, Vacuum, Robot
---
# Vorwerk : connectez votre robot aspirateur Kobold ou Neato dans Constellation

Le package Vorwerk vous permet connecter votre robot aspirateur Kobold VR200 ou VR300 dans Constellation.

L'API utilisée par Vorwerk est la même que celle des Neato Botvac, vous pouvez donc également connecter vos robots Botvac.

## Installation
Depuis le "Online Package Repository" de votre Console Constellation, installez et déployez le package sur la sentinelle de votre choix (compatible Windows et Linux).

Sur la page de Settings, vous devez obligatoirement définir vos identifiants de connexion au Cloud Vorwerk ou Neato.

Vous pouvez également déployer ce package manuellement dans la configuration de votre Constellation :

```xml
<package name="Vorwerk">
  <settings>
    <setting key="Username" value="xxxx@yyyyy.com" />
    <setting key="Password" value="xxxxxxx" />
    <setting key="Vendor" value="Vorwerk" />
  </settings>
</package>
```

## Settings

| Nom | Type | Requis ? | Description du Setting |
| --- | ---- | -------- | ---------------------  |
| Username | String | OUI | Nom d'utilisateur Vorwerk/Neato |
| Password | String | OUI | Mot de passe Vorwerk/Neato |
| Vendor | String | NON | Marque du robot : "Vorwerk" ou "Neato" (par défaut Vorwerk) |
| RobotPollingInterval | Int | NON | Interval en seconde d'interrogation du robot (par défaut 10 sec) |
| DashboardPollingInterval | Int | NON | Interval en seconde d'interrogation du "dashboard" (par défaut 900 sec soit 15 min) |

## StateObjects
Vous retrouverez un StateObject "Dashboard" qui contient les informations sur le compte utilisé ainsi que la liste des robots attachés et un StateObject par robot (le nom du StateObject est le nom du robot) qui contient les informations sur l'état du robot.

| Nom | Type | Description |
| --- | ---- | ----------- |
| Dashboard | Vorwerk.Models.Dashboard | Information sur le compte et robots attachés |
| << nom du robot >> | 	Vorwerk.Models.RobotState | Information sur l'état du robot

## MessageCallbacks
Le package expose 2 MessageCallbacks :

| Signature | Réponse (saga) | Description |
| --------- | -------------- | ----------- |
| StartCleaning(string robotName, bool ecoMode = true) | RobotState | Démarre un néttoyage |
| StartSpotCleaning(string robotName, bool ecoMode = true, int height = 200, int width = 200, bool repeat = false) | RobotState | Démarre un néttoyage d'une zone |
| StopCleaning(string robotName) | RobotState | Arrete le néttoyage |
| PauseCleaning(string robotName) | RobotState | Pause le néttoyage |
| ResumeCleaning(string robotName) | RobotState | Résume le néttoyage |
| SendToBase(string robotName) | RobotState | Démarre un néttoyage |

## Quelques exemples
* Lancer le néttoyage en journée lorsque l'alarme est armée
* Contrôler le robot depuis vos Dashboards HTML ou WPF

## Ressources

Quelques ressources pour le developpement du package :

* A node module for Vorwerk Kobold VR200 and VR300 : https://github.com/nicoh88/node-kobold
* PHP library for Neato Botvac : https://github.com/tomrosenback/botvac
* Neato Javascript SDK : https://github.com/NeatoRobotics/neato-sdk-js
* Python module for interacting with Neato Botvac Connected vacuum robots : https://github.com/stianaske/pybotvac

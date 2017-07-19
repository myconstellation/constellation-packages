# Constellation Package to get horoscope

This package get the horoscope for the day.

### MessageCallbacks
  - GetHoroscope(Zodiacal_Sign) : Get the horoscope of zodiacal sign for the day.

### StateObjects

This package create one StateObject by sign in settings. Each StateObject have the sign as name.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
	<package name="Horoscope">
		<settings>
			<setting key="ZodiacalSigns">
				<content>
					<signs xmlns="http://schemas.myconstellation.io/Constellation/1.8/PackageManifest">
						<sign name="Scorpion" />
						<sign name="Gemeaux" />
					</signs>
				</content>
			</setting>
		</settings>
	</package>
```

By default the package queries the horoscope at 01:00 (Asiaflash refresh time) :

```xml
	<setting key="RefreshTime" value="01:00:00" />
```

By default the package use url from asiaflash to get horoscope :

```xml
	<setting key="Url" value="http://www.asiaflash.com/horoscope/rss_horojour_{0}.xml" />
```
----

Apache License
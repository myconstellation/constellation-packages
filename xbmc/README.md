# XBMC/Kodi Package for Constellation

This XBMC/Kodi Package allow you to connect your XBMC/Kodi hosts to Constellation

### StateObjects
  - Each XBMC/Kodi hosts registred in settings are pushed as StateObjects. Each StateObjects contains the current player state and player item.

### MessageCallbacks
  - OpenMovie(xbmcName, itemId) : Start playback of a movie with the given ID
  - OpenEpisode(xbmcName, itemId) : Start playback of an episode with the given ID
  - OpenSong(xbmcName, itemId) : Start playback of a song with the given ID
  - OpenItem(xbmcName, item) : Start playback of a playlist's item
  - PlayPause(xbmcName) : Pauses or unpause playback
  - Stop(xbmcName) : Stops playback
  - SendInputKey(xbmcName, inputKey) : Sends the input key to the XBMC host
  - ScanVideoLibrary(xbmcName) : Scans the video sources for new library items
  - ScanAudioLibrary(xbmcName) : Scans the audio sources for new library items
  - SetVolume(string xbmcName, int volume) : Sets the current volume
  - SetMute(xbmcName, mute) : Sets the mute mode
  - ShowNotification(xbmcName, request) : Shows the notification

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="Xbmc">
  <settings>
    <setting key="xbmcConfigurationSection">
      <content>
        <xbmcConfigurationSection xmlns="urn:Xbmc">
          <hosts>
            <xbmcHost name="XBMC Bedroom" host="192.168.0.X" port="80" login="xbmc" password="xbmc" />
            <xbmcHost name="XBMC Living room" host="192.168.0.Y" port="80" login="xbmc" password="xbmc" />
          </hosts>
        </xbmcConfigurationSection>
      </content>
    </setting>
  </settings>
</package>
```
License
----

Apache License
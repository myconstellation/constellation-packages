# Squeezebox Package for Constellation

Package to control Squeezebox (Logitech Media Server).

### MessageCallbacks SendToServer
 - Mute_Toggle : Toggle muting for all the players
 - Mute_Off : Disable muting for all the players
 - Mute_On : Mute all the players
 - Pause : Pause all the players
 - Play : Play all the players
 - Play_Toggle : Toggle play and pause state for all the players
 - Power_Off : Power off all the players
 - Power_On : Power On all the players
 - Power_Toggle : Toggle power for all the players
 - Scan_Cancel : Cancel the actual scan
 - Scan_Fast : Launch fast scan
 - Scan_Full : Launch full scan
 - Stop : Stop all the players
 - Sync_Off : Disable syncing for all the players
 - Sync_To : Sync all the players to a target player (require target player name)
 - Volume : Set the volume of all the players (require volume level)
 - Volume_Down : Decrease the volume of all the players
 - Volume_Up : Increase the volume of all the players

### MessageCallbacks SendToSqueezebox
 - Mute_Off : Disable muting
 - Mute_On : Enable muting
 - Mute_Toggle : Toggle muting
 - Next : Launch the next song
 - Pause : Pause music
 - Play : Play music
 - Play_Album : Launch an album (require album name)
 - Play_Album_Id : Launch an album by id (require album id)
 - Play_Artist : Launch an artist (require artist name)
 - Play_Artist_Id : Launch an artist by id (require artist id)
 - Play_Playlist : Launch a playlist (require playlist name)
 - Play_Playlist_Id : Launch a playist by id (require playlist id)
 - Play_Title : Launch a title (require title)
 - Play_Title_Id : Launch a title by id (require title id)
 - Play_Toggle : Toggle pause state
 - Playlist_Clear : Erase current playlist
 - Power_Off : Power Off
 - Power_On : Power On
 - Power_Toggle : Toggle power state
 - Previous : Launch the previous song
 - Random_Album : Launch a random play by album
 - Random_Artist : Launch a random play by artist
 - Random_Title : Launch a random play by title
 - Random_Year : Launch a random play by year
 - Repeat_Off : Disable repeat
 - Repeat_Playlist : Repeat playlist
 - Repeat_Title : Repeat title
 - Repeat_Toggle : Toggle repeat state
 - Shuffle_Album : Shuffle by album
 - Shuffle_Off : Shuffle Off
 - Shuffle_Title : Shuffle by title
 - Shuffle_Toggle : Toggle shuffle state
 - Stop : Stop music
 - Sync : Sync another player to this player (require target player)
 - Sync_Off : Disable syncing
 - Sync_To : Sync this player to another player (require target player)
 - Volume : Set volume (require volume level)
 - Volume_Down : Decrease volume
 - Volume_Up : Increase volume

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
	<package name="Squeezebox" enable="true">
		<settings>
			<setting key="ServerUrl" value="IP Adress:Port" />
		</settings>
	</package>
```

### Settings
 - ServerUrl (string - required) : IP adress of the Logitech Media Server with port (for exemple : 192.168.0.50:9000)

License
----

Apache License

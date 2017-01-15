# Panasonic TV Package for Constellation

Package to control Panasonic TV (Viera). Your TV must be compatible with Panasonic TV Remote 2 app.

### MessageCallbacks
 - Red : RED button
 - Green : GREEN button
 - Blue : BLUE button
 - Yellow : YELLOW button
 - Power : Toggle power
 - PowerOn : Power ON the TV
 - Button0 : 0 button
 - Button1 : 1 button
 - Button2 : 2 button
 - Button3 : 3 button
 - Button4 : 4 button
 - Button5 : 5 button
 - Button6 : 6 button
 - Button7 : 7 button
 - Button8 : 8 button
 - Button9 : 9 button
 - Back : Return button
 - VolumeUp : Increase volume
 - VolumeDown : Decrease volume
 - OK : Enter button
 - Up : Up button
 - Right : Right button
 - Down : Down button
 - Left : Left button
 - ProgramUp : Channel up
 - ProgramDown : Channel down
 - Mute : Toggle mute
 - TV : TV button
 - Input : AUX button
 - VieraTools : VieraTools button
 - Cancel : Cancel button
 - Option : Options button
 - ThreeDimensional : 3D button
 - SDCard : SD card button
 - DisplayMode : Change display mode
 - Menu : Menu button
 - VieraConnect : Display VIERA Connect
 - VieraLink : Display VIERA Link
 - EPG : Toggle EPG
 - Text : Toggle Text
 - Subtitle : Toggle subtitle
 - Info : Info button
 - Index : Index button
 - Hold : Hold button
 - LastView : Display last view
 - Rewind : Rewind button
 - Play : Play button
 - FastForward : Forward button
 - SkipPrevious : Skip Previous
 - Pause : Pause button
 - SkipNext : Skip Next
 - Stop : Stop button
 - Record : Record button
 - HDMI_1 : Toggle HDMI1
 - HDMI_2 : Toggle HDMI2
 - HDMI_3 : Toggle HDMI3
 - HDMI_4 : Toggle HDMI4

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
	<package name="FreeboxTV" enable="true">
		<settings>
			<setting key="Your_TV_name" value="IP Adress" />
		</settings>
	</package>
```

### Settings
 - Your_TV_name (string - optional - can be change) : IP adress of the Panasonic TV

License
----

Apache License

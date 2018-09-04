# S-Sound - Multi-room audio system powered by Constellation

S-Sound is a Constellation package  that deploys on any Windows Sentinel can broadcast sound.

You can play your audio files locally or on a network share and even HTTP streaming (web radio for example), listen to your audio inputs and even make voice synthesis to broadcast voice messages.

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="SSound" />
```

Without setting, the ouput device will be the Windows default device.

### StateObjects
  - InDevices : the list of all input devices
  - OutDevices : the list of all output devices
  - OutDevice : the current output device selected for the S-Sound instance
  - WaveInPlayers : list of loaded and available WaveIn players
  - CurrentPlayer : name of the current player (MediaPlayer, StreamingPlayer or WaveInPlayer)
  - CurrentPlayerInfo : informations about the current player (Arguments and Status)
  - State : state of the current player (Playing, Stopped or Paused)
  - Volume : the volume of the current output device (1.0 is full scale, 0.0 is silence)
  - Mute : mute or un-mute (boolean)
  - DlnaMediaRenderer : indicate if the DLNA Media Renderer is enabled (boolean)

### MessageCallbacks
  - PlayMediaRessource(uri) : Plays the media ressource.
  - PlayM3UList(uri) : Plays the M3U playlist.
  - PlayMediaRessourceList(uris) : Plays the media ressource list.
  - PlayMP3Streaming(uri) : Plays the MP3 stream using the StreamingPlayer.
  - PlayWaveIn(name) : Plays the device input device (specified the device name or friendly name).
  - Speech(text) : Speeches the specified text (/!\ the Cerevoice configuration must be set in setting).
  - Play() : Plays/Resumes the current player.
  - Pause() : Pauses the current player.
  - Stop() : Stops the current player.
  - SetVolume(volume) : Sets volume: 1.0 is full scale, 0.0 is silence
  - SetMute(mute) : Set mute or un-mute the output device.


### Configuration

You can customize the package by adding the `ssoundConfiguration` configuration section like this :

```xml
<package name="SSound">
  <settings>
      <setting key="ssoundConfiguration">
        <content>
          <ssoundConfiguration xmlns="urn:SSound">
          </ssoundConfiguration>
        </content>
    </setting>
  </settings>
</package>
```

The `ssoundConfiguration` section has 5 attributes :

- outputDeviceName (string) : the device ID or name of the output device use to renderer the audio for this S-Sound instance (if not set, the default audio output device will be selected as output device)
- endpointName (string) : the DLNA endpoint name (by defaut the MachineName)
- enableDlnaRenderer (boolean) : enable the DLNA media renderer
- initialVolume (float) : the default volume for the output device (leave empty to use the current Windows volume)
- speechVolume (float) : the default volume for the TTS (leave empty to use the current Windows volume)

You can explore the `OutDevices` state object to get the list of all available output device.

Keep the `initialVolume` and `speechVolume` attribute empty to use the current Windows volume.

To enable the TTS you need to create an account on https://www.cereproc.com/en/user/register then add the following section :

```xml
<cerevoice accountID="xxxxxxxxxx" password="xxxxxxxxxxxxx" voiceName="Suzanne" bitrate="48000" />
```

The voice names are availables here : https://www.cereproc.com/en/products/cloud

The `bitrate` is optional, 48000 by default.

Then you can also customize the audio inputs like this :
```xml
<inputs>
  <device name="jack" inputDeviceName="Microphone (4- USB Audio CODEC )" signalThreshold="3" autoPlay="true" />
  <device name="chromecast" inputDeviceName="Microphone (3- USB Audio CODEC )" signalThreshold="3" />
</inputs>
```

This allow you to define a friendlyName for your input device.

In this example the device "Microphone (4- USB Audio CODEC )" is also named "jack".

Now, when you invoke the MC `PlayWaveIn(name)` you can pass the device name or the friendly name !

Also on each input device you can add the following optionals attributes :

- autoPlay (boolean) : if signal is detected, the WaveInPlayer is automatically played (by default: false)
- bufferMilliseconds (int) : the audio buffer duration in ms (by default: 25)
- signalThreshold (in) : the threshold of the minimum volume to consider that there is an audio signal on the input (by default: 5)
- hasSignalDuration :  the minimum time (in ms) to consider that there is an audio signal(by default: 2000)
- noSignalDuration : the time  (in ms) beyond which in the absence of audio signal, we stop the playback (by default: 30000)

So for my "jack" input, the WaveIn is automatically plays when the signal when the sound exceeds 3% for 2 seconds.

Full example :
```xml
<package name="SSound">
  <settings>
      <setting key="ssoundConfiguration">
        <content>
          <ssoundConfiguration xmlns="urn:SSound" endpointName="Cuisine" outputDeviceName="Speakers (4- USB Audio CODEC )" initialVolume="0.8" speechVolume="0.7" enableDlnaRenderer="true">
            <inputs>
               <device name="jack" inputDeviceName="Microphone (4- USB Audio CODEC )" signalThreshold="3" autoPlay="true" />
               <device name="chromecast" inputDeviceName="Microphone (3- USB Audio CODEC )" signalThreshold="3" />
             </inputs>
             <cerevoice accountID="xxxxx" password="xxxxx" voiceName="Suzanne" bitrate="48000" />
           </ssoundConfiguration>
         </content>
    </setting>
  </settings>
</package>
```

### Multi instances

If you are multiple output audio devices, you can deploy multiple instances of S-Sound, once per output device.

For example, I have two audio USB output devices plug on my PC to broadcast sound in my kitchen and my bathroom.

- Speakers (4- USB Audio CODEC ) is the output device for my kitchen
- Speakers (3- USB Audio CODEC ) is the output device for my bathroom

So as you can see, I have deploy the SSound package twice on my sentinel. One instance is named "SSound-Cuisine" and the other is named "SSound-SalleDeBain" :
```xml
<package name="SSound-Cuisine" filename="SSound.zip">
  <settings>
    <setting key="ssoundConfiguration">
      <content>
        <ssoundConfiguration xmlns="urn:SSound" endpointName="Cuisine" outputDeviceName="Speakers (4- USB Audio CODEC )" initialVolume="0.8" speechVolume="0.7" enableDlnaRenderer="true">
          <inputs>
             <device name="jack" inputDeviceName="Microphone (4- USB Audio CODEC )" signalThreshold="3" autoPlay="true" />
             <device name="chromecast" inputDeviceName="Microphone (3- USB Audio CODEC )" signalThreshold="3" />
           </inputs>
           <cerevoice accountID="xxxxx" password="xxxxx" voiceName="Suzanne" bitrate="48000" />
        </ssoundConfiguration>
      </content>
    </setting>
  </settings>
</package>
<package name="SSound-SalleDeBain" filename="SSound.zip">
  <settings>
    <setting key="ssoundConfiguration">
      <content>
        <ssoundConfiguration xmlns="urn:SSound" endpointName="Salle de Bain" outputDeviceName="Speakers (3- USB Audio CODEC )" initialVolume="0.8" speechVolume="0.7" enableDlnaRenderer="true">
          <inputs>
            <device name="jack" inputDeviceName="Microphone (4- USB Audio CODEC )" signalThreshold="3" />
            <device name="chromecast" inputDeviceName="Microphone (3- USB Audio CODEC )" signalThreshold="3" autoPlay="true" />
          </inputs>
          <cerevoice accountID="xxxxx" password="xxxxx" voiceName="Suzanne" bitrate="48000" />
        </ssoundConfiguration>
      </content>
    </setting>
  </settings>
</package>
```

For each instance, the `outputDeviceName` is different.

The `autoPlay` device is also different for these two instance. If there is an audio signal on the input named "chromecast" the sound will be broadcast to the bathroom and the "jack" input to the kitchen !

For information (FR) about S-Sound : http://sebastien.warin.fr/2015/05/05/2476-s-sound-la-solution-audio-multi-room-connectee-dans-la-constellation/

### Changelog

- 1st dec. 2014 : release 1.0 (file and HTTP media support + DLNA renderer)
- 9th dec. 2014 : release 1.1 (WaveIn w/ noise detection & MP3 streaming & M3U playlist support, stability & quality improvement)
- 21th jan. 2015 : release 1.2 (Text to Speech based on Google)
- 24th aug. 2015 : release 1.3 (TTS moving to Cerevoice & upgrading to Constellation 1.7 & NAudio 1.7)
- 3th sep. 2015 : release 1.3.1 (adding more setting)
- 22th feb 2016 : release 1.4 (upgrading to Constellation 1.8)
- 17th nov 2016 : release 1.4.2 (pushing devices capabilities in StateObject)
- 18th apr 2018 : release 1.5 (updating to last libs & net462, rewriting from legacy WaveIn/Out API to Wasapi API, new audio volume management,  optional configuration, devices discovery & code refactoring)

License
----

Apache License
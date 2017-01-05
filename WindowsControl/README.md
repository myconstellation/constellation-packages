# WindowsControl Package for Constellation

Control your Windows from Constellation

### StateObjects
  - SessionLocked : indicate if the session is locked or not

### MessageCallbacks
  - LogoffSession() : Logoff the current session.
  - LockWorkStation() : Locks the work station.
  - Shutdown() : Shutdowns the work station.
  - Reboot() : Reboots the work station.
  - Sleep() : Sleeps the work station.
  - Hibernate() : Hibernates the work station.
  - Mute() : Mute/Unmute volume.
  - VolumeUp() : Increase volume.
  - VolumeDown() : Decrease volume.
  - SetBrightness(targetBrightness) : Sets the brightness.
  - BrightnessUp() : Increase Brightness.
  - BrightnessDown() : Decrease Brightness.

### Installation

Declare the package in a Sentinel with the following configuration :

```xml
<package name="WindowsControl" />
```
License
----

Apache License

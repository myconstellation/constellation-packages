# PushBullet Package for Constellation

PushBullet connector for Constellation to send notifications, links and files to your devices or friends and more.

API official Documentation : [http://docs.pushbullet.com/http/](http://docs.pushbullet.com/http/)

### MessageCallbacks
 - GetDevices : Get a list of devices belonging to the current user.
 - GetChats : Get a list of chats belonging to the current user.
 - GetPushes : Request push history.
 - GetCurrentUser :  Gets the currently logged in user.
 - SendSMS : Sends the SMS from your phone (for Android devices only).
 - CopyToClipboard : Copy a String to the Device's Clipboard (PushBullet premium feature).
 - PushNote : Push a note to a device or another person.
 - PushLink : Push a Link to a device or another person.
 - PushFile : Uploads and Push a file to a device or another person.

### StateObjects
 - CurrentUser : user currently logged in
 - Chats : list of chats belonging to the current user
 - Devices : list of devices belonging to the current user
 - RateLimit : service usage limit

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="PushBullet">
  <settings>
	<setting key="token" value="<< access token >>" />
  </settings>
</package>
```

To get an access token go to [Account Settings](https://www.pushbullet.com/#settings/account) page.

### Settings
 - token (string - required) : PushBullet access token
 - PushDevicesAsStateObjects (Boolean - optional) : Push PushBullet's devices as StateObjects (default: true)
 - PushChatsAsStateObjects (Boolean - optional) : Push PushBullet's chats as StateObjects (default: true)
 - PushCurrentUserAsStateObject (Boolean - optional) : Push PushBullet's current user as StateObject (default: true)
 - SendPushesReceivedToGroup (string - optional) : Send pushes received to Constellation group. Leave blank to disable! (default: 'PushBullet')
 - SendEphemeralsReceivedToGroup (string - optional) : Send ephemerals received to Constellation group. Leave blank to disable! (default: 'PushBullet')

License
----

Apache License
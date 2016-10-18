# Pushover Package for Constellation

Pushover connector for Constellation to send notifications to your devices or friends.

API official Documentation : [https://pushover.net/api](https://pushover.net/api/)

### MessageCallbacks
 - CheckUserOrGroup(userOrGroupId) : Checks user or group identifiers.
 - GetNotificationStatus(receipt) : Gets the status of your emergency notification.
 - PushNotification(message, title = null, url = null, user = null, devices = null, sound = Sound.Pushover, priority = Priority.Normal, timestamp = 0, emergencyOptions = null) : Pushes the specified notification

### StateObjects
 - RateLimit : service usage limit

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="Pushover">
  <settings>
	<setting key="Token" value="<< Pushover API token >>" />
	<setting key="UserId" value="<< Pushover User Id >>" />
  </settings>
</package>
```

To get an API token you need to [Create Application](https://pushover.net/apps/build) on Pushover.com.

### Settings
 - Token (string - required) : Your Pushover application's API token.
 - UserId (string - required) : Your user key (not e-mail address) viewable when logged into the Pushover dashboard.
 - DefaultEmergencyRetry (Int32 - optional - default: 60) : Specifies how often (in seconds) the Pushover servers will send the same notification to the user if the notification has not been acknowledged and is not expired. This parameter must have a value of at least 30 seconds between retries.
 - DefaultEmergencyExpiration (Int32 - optional - default: 3600) : Specifies how many seconds your notification will continue to be retried for (every retry seconds).  This parameter must have a maximum value of at most 86400 seconds (24 hours).

License
----

Apache License
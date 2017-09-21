# Microsoft Exchange connector for Constellation

This package pushes Exchange Calendars to Constellation and allow to send mail

### StateObjects
  - All calendars defined in "CalendarAccounts" setting are pushed as StateObjects

### MessageCallbacks
 - SendMail(SendMailRequest) : send email message

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="Exchange">
    <settings>
      <setting key="EWSServiceUrl" value="https://my.exchange-server.com/ews/exchange.asmx" />
      <setting key="AccountName" value="MyServiceAccount" />
      <setting key="AccountPassword" value="MyServiceAccountPassword" />
      <setting key="AccountDomain" value="MY-DOMAIN" />
      <setting key="CalendarAccounts" value="account1@mydomain.net;account2@mydomain.net" />
      <setting key="NumberOfDaysToInclude" value="5" />
    </settings>
</package>
```
License
----

Apache License
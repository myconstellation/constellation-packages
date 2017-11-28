# Windows Performance Counters Package for Constellation

This package injects Windows Performance Counters into Constellation.

### Installation

Declare the package in a Sentinel with the following configuration :
```xml
<package name="perfcounter">
  <settings>
	<setting key="PerfCounters">
        <content>
          <perfCounterSection xmlns="urn:PerfCounter">
            <perfCounters>     
              <perfCounter id="<< StateObject Name >>" categoryName="<<  Performance Counter Category Name >>" counterName="<< Performance Counter Counter Name >>"  instanceName="<< Optionnal Performance Counter Instance Name >>" />
			</perfCounters>
          </perfCounterSection>
        </content>
      </setting>
  </settings>
</package>
```

For example, to inject counters about your Processor, PhysicalDisk, Memory, Network interfaces, ASP.net, CLR and IIS, you can declare in the PerfCounters setting :

```xml
<package name="perfcounter">
  <settings>
    <setting key="PerfCounters">
        <content>
          <perfCounterSection xmlns="urn:PerfCounter">
            <perfCounters>     

              <!-- Processor -->
              <perfCounter id="ProcessorTime" categoryName="Processor" counterName="% Processor Time" instanceName="_Total" />
              <perfCounter id="ProcessorQueueLength" categoryName="System" counterName="Processor Queue Length" />

              <!-- Disk -->
              <perfCounter id="Disk0_IdleTime" categoryName="PhysicalDisk" counterName="% Idle Time" instanceName="0 C:" />
              <perfCounter id="Disk0_SecPerRead" categoryName="PhysicalDisk" counterName="Avg. Disk sec/read"   instanceName="0 C:"/>
              <perfCounter id="Disk0_SecPerWrite" categoryName="PhysicalDisk" counterName="Avg. Disk sec/write"  instanceName="0 C:" />
              <perfCounter id="Disk0_QueueLength" categoryName="PhysicalDisk" counterName="Current Disk Queue Length"  instanceName="0 C:" />
              
              <!-- Memory -->
              <perfCounter id="MemoryAvailableMBytes" categoryName="Memory" counterName="Available MBytes"  />
              <perfCounter id="MemoryPagesPerSec" categoryName="Memory" counterName="Pages/sec"  />
              <perfCounter id="MemoryCacheBytes" categoryName="Memory" counterName="Cache Bytes"  />
              
              <!-- Network Interface -->
              <perfCounter id="NetworkIntefaceBytesPerSec" categoryName="Network Interface" counterName="Bytes Total/sec" instanceName="Broadcom NetXtreme 57xx Gigabit Controller"  />
              <perfCounter id="NetworkIntefaceOutputQueueLength" categoryName="Network Interface" counterName="Output Queue Length" instanceName="Broadcom NetXtreme 57xx Gigabit Controller" />
              
              <!-- Paging File -->
              <perfCounter id="PagingFileUsage" categoryName="Paging File" counterName="% Usage" instanceName="_Total"  />

              <!-- .NET CLR -->
              <perfCounter id="DotNetExceptionThrownPerSec" categoryName=".NET CLR Exceptions" counterName="# of Exceps Thrown / Sec" instanceName="_Global_" />
              <perfCounter id="DotNetTotalCommittedBytes" categoryName=".NET CLR Memory" counterName="# Total Committed Bytes" instanceName="_Global_" />

              <!-- ASP.NET -->
              <perfCounter id="AspNetRequestPerSec" categoryName="ASP.NET Applications" counterName="Requests/Sec" instanceName="__Total__" />
              <perfCounter id="AspNetApplicationRestart" categoryName="ASP.NET" counterName="Application Restarts" />
              <perfCounter id="AspNetRequestWaitTime" categoryName="ASP.NET" counterName="Request Wait Time" />
              <perfCounter id="AspNetRequestQueued" categoryName="ASP.NET" counterName="Requests Queued" />

              <!-- IIS -->
              <perfCounter id="IISGetRequestPerSec" categoryName="Web Service" counterName="Get Requests/sec" instanceName="_Total"  />
              <perfCounter id="IISPostRequestPerSec" categoryName="Web Service" counterName="Post Requests/sec" instanceName="_Total"  />
              <perfCounter id="IISCurrentConnections" categoryName="Web Service" counterName="Current Connections" instanceName="_Total"  />

              <!-- SQL Server -->
              <perfCounter id="SqlBufferCacheHitRatio" categoryName="MSSQL$SQL2012:Buffer Manager" counterName="Buffer cache hit ratio" />
              <perfCounter id="SqlPageLifeExpectancy" categoryName="MSSQL$SQL2012:Buffer Manager" counterName="Page life expectancy" />
              <perfCounter id="SqlBatchRequestsPerSec" categoryName="MSSQL$SQL2012:SQL Statistics" counterName="Batch Requests/Sec" />
              <perfCounter id="SqlSQLCompilationsPerSec" categoryName="MSSQL$SQL2012:SQL Statistics" counterName="SQL Compilations/Sec" />
              <perfCounter id="SqlSQLReCompilationsPerSec" categoryName="MSSQL$SQL2012:SQL Statistics" counterName="SQL Re-Compilations/Sec" />
              <perfCounter id="SqlUserConnections" categoryName="MSSQL$SQL2012:General Statistics" counterName="User Connections" />
              <perfCounter id="SqlLockWaitsPerSec" categoryName="MSSQL$SQL2012:Locks" counterName="Lock Waits/sec" instanceName="_Total" />
              <perfCounter id="SqlPageSplitsPerSec" categoryName="MSSQL$SQL2012:Access Methods" counterName="Page Splits/sec" />
              <perfCounter id="SqlProcessesBlocked" categoryName="MSSQL$SQL2012:General Statistics" counterName="Processes Blocked" />
              <perfCounter id="SqlCheckpointPagesPerSec" categoryName="MSSQL$SQL2012:Buffer Manager" counterName="Checkpoint Pages/sec" />

              <!-- Constellation Server / HTTP requests -->
              <perfCounter id="ConstellationHttpRequest" categoryName="Constellation Server" counterName="# HTTP Requests / sec" instanceName="default" />
              <perfCounter id="ConstellationHttpRequestCount" categoryName="Constellation Server" counterName="Total HTTP Requests" instanceName="default" />

              <!-- Constellation Server / Authorization middleware -->
              <perfCounter id="ConstellationAccessGranted" categoryName="Constellation Server" counterName="# Access granted / sec" instanceName="default" />
              <perfCounter id="ConstellationAccessGrantedCount" categoryName="Constellation Server" counterName="Total Access granted" instanceName="default" />
              <perfCounter id="ConstellationAccessDenied" categoryName="Constellation Server" counterName="# Access denied / sec" instanceName="default" />
              <perfCounter id="ConstellationAccessDeniedCount" categoryName="Constellation Server" counterName="Total Access denied" instanceName="default" />
              <perfCounter id="ConstellationAccessChecked" categoryName="Constellation Server" counterName="# Check access / sec" instanceName="default" />
              <perfCounter id="ConstellationAccessCheckedCount" categoryName="Constellation Server" counterName="Total Check access" instanceName="default" />

              <!-- Constellation Server / StateObjects provider -->
              <perfCounter id="ConstellationPushStateObject" categoryName="Constellation Server" counterName="# PushStateObject / sec" instanceName="default" />
              <perfCounter id="ConstellationPushStateObjectCount" categoryName="Constellation Server" counterName="Total PushStateObject" instanceName="default" />
              <perfCounter id="ConstellationUpdateStateObject" categoryName="Constellation Server" counterName="# UpdateStateObject / sec" instanceName="default" />
              <perfCounter id="ConstellationUpdateStateObjectCount" categoryName="Constellation Server" counterName="Total UpdateStateObject" instanceName="default" />
              <perfCounter id="ConstellationRequestStateObjects" categoryName="Constellation Server" counterName="# RequestStateObjects /sec" instanceName="default" />
              <perfCounter id="ConstellationRequestStateObjectsCount" categoryName="Constellation Server" counterName="Total RequestStateObjects" instanceName="default" />
              <perfCounter id="ConstellationSubscribeStateObjects" categoryName="Constellation Server" counterName="# SubscribeStateObjects / sec" instanceName="default" />
              <perfCounter id="ConstellationSubscribeStateObjectsCount" categoryName="Constellation Server" counterName="Total SubscribeStateObjects" instanceName="default" />
              <perfCounter id="ConstellationStateObjectsCount" categoryName="Constellation Server" counterName="Current StateObjects Count" instanceName="default" />
              <perfCounter id="ConstellationStateObjectSubscriptionCount" categoryName="Constellation Server" counterName="Current StateObject Subscriptions" instanceName="default" />

              <!-- Constellation Server / Messaging -->
              <perfCounter id="ConstellationSendMessage" categoryName="Constellation Server" counterName="# SendMessage / sec" instanceName="default" />
              <perfCounter id="ConstellationSendMessageCount" categoryName="Constellation Server" counterName="Total SendMessage" instanceName="default" />
              <perfCounter id="ConstellationReceiveMessage" categoryName="Constellation Server" counterName="# ReceiveMessage / sec" instanceName="default" />
              <perfCounter id="ConstellationReceiveMessageCount" categoryName="Constellation Server" counterName="Total ReceiveMessage" instanceName="default" />

              <!-- Constellation Server / Logging -->
              <perfCounter id="ConstellationWriteLog" categoryName="Constellation Server" counterName="# WriteLog / sec" instanceName="default" />
              <perfCounter id="ConstellationWriteLogCount" categoryName="Constellation Server" counterName="Total WriteLog" instanceName="default" />

              <!-- Constellation Server / Packages -->
              <perfCounter id="ConstellationPackageConnections" categoryName="Constellation Server" counterName="Total Package connections" instanceName="default" />
              <perfCounter id="ConstellationPackageDisconnections" categoryName="Constellation Server" counterName="Total Package disconnections" instanceName="default" />
              <perfCounter id="ConstellationPackagesConnected" categoryName="Constellation Server" counterName="Current Packages connected" instanceName="default" />

              <!-- Constellation Server / Sentinels -->
              <perfCounter id="ConstellationSentinelConnections" categoryName="Constellation Server" counterName="Total Sentinel connections" instanceName="default" />
              <perfCounter id="ConstellationSentinelDisconnections" categoryName="Constellation Server" counterName="Total Sentinel disconnections" instanceName="default" />
              <perfCounter id="ConstellationSentinelsConnected" categoryName="Constellation Server" counterName="Current Sentinels connected" instanceName="default" />

              <!-- Constellation Server / Consumers -->
              <perfCounter id="ConstellationConsumerConnections" categoryName="Constellation Server" counterName="Total Consumers connections" instanceName="default" />
              <perfCounter id="ConstellationConsumerDisconnections" categoryName="Constellation Server" counterName="Total Consumers disconnections" instanceName="default" />
              <perfCounter id="ConstellationConsumerConnected" categoryName="Constellation Server" counterName="Current Consumers connected" instanceName="default" />

              <!-- Constellation Server / API REST -->
              <perfCounter id="ConstellationWebApiSubscriptionCount" categoryName="Constellation Server" counterName="Current subscriptions (HTTP REST API)" instanceName="default" />
              <perfCounter id="ConstellationWebApiRequestInProcess" categoryName="Constellation Server" counterName="Current long-polling requests (HTTP REST API)" instanceName="default" />
              
            </perfCounters>
          </perfCounterSection>
        </content>
      </setting>
  </settings>
</package>
```

License
----

Apache License
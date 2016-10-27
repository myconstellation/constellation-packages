<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="9b574bd2-c34a-417a-b263-7761128f9628" namespace="GraylogConnector" xmlSchemaNamespace="urn:GraylogConnector" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
    <enumeratedType name="GelfOutputProtocol" namespace="GraylogConnector">
      <literals>
        <enumerationLiteral name="Udp" />
        <enumerationLiteral name="Tcp" />
      </literals>
    </enumeratedType>
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="GraylogConfiguration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="graylogConfiguration">
      <attributeProperties>
        <attributeProperty name="SendPackageLogs" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="sendPackageLogs" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="SendPackageStates" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="sendPackageStates" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="SendSentinelUpdates" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="sendSentinelUpdates" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Subscriptions" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="subscriptions" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/SubscriptionCollection" />
          </type>
        </elementProperty>
        <elementProperty name="GelfOutputs" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="outputs" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/GelfOutputCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="SubscriptionCollection" xmlItemName="subscription" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Subscription" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="Subscription">
      <attributeProperties>
        <attributeProperty name="Sentinel" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="sentinel" isReadOnly="false" defaultValue="&quot;*&quot;">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Package" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="package" isReadOnly="false" defaultValue="&quot;*&quot;">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Name" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="name" isReadOnly="false" defaultValue="&quot;*&quot;">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Type" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="type" isReadOnly="false" defaultValue="&quot;*&quot;">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Enable" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="enable" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Aggregation" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="aggregation" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/AggregatePropertyCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Exclusions" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="exclusions" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/ExclusionCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="GelfOutputElement">
      <attributeProperties>
        <attributeProperty name="Host" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Port" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="port" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Protocol" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="protocol" isReadOnly="false" defaultValue="GelfOutputProtocol.Udp">
          <type>
            <enumeratedTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/GelfOutputProtocol" />
          </type>
        </attributeProperty>
        <attributeProperty name="Enable" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="enable" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="GelfOutputCollection" xmlItemName="gelfOutput" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/GelfOutputElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="AggregatePropertyElement">
      <attributeProperties>
        <attributeProperty name="PropertyName" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="propertyName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="IncludeAggregateInfo" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="includeAggregateInfo" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="AggregatePropertyCollection" xmlItemName="aggregateProperty" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Interval" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="interval" isReadOnly="false" defaultValue="30">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/AggregatePropertyElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ExclusionElement">
      <attributeProperties>
        <attributeProperty name="Sentinel" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="sentinel" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Package" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="package" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Name" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Type" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="type" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="ExclusionCollection" xmlItemName="exclusion" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/9b574bd2-c34a-417a-b263-7761128f9628/ExclusionElement" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>
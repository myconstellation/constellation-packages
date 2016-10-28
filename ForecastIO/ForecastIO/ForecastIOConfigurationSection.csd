<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="602c8513-00eb-4bd9-b70b-b408a9e6c61e" namespace="ForecastIO" xmlSchemaNamespace="urn:ForecastIO" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ForecastIOConfigurationSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="forecastIOConfigurationSection">
      <attributeProperties>
        <attributeProperty name="RefreshInterval" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="refreshInterval" isReadOnly="false" defaultValue="&quot;01:00:00&quot;">
          <type>
            <externalTypeMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/TimeSpan" />
          </type>
        </attributeProperty>
        <attributeProperty name="ApiKey" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="apiKey" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Stations" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="stations" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/StationsCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="StationElement">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Longitude" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="longitude" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/Double" />
          </type>
        </attributeProperty>
        <attributeProperty name="Latitude" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="latitude" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/Double" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="StationsCollection" xmlItemName="station" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/602c8513-00eb-4bd9-b70b-b408a9e6c61e/StationElement" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>
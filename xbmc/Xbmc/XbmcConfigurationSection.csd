<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="04931c4c-3e96-47d5-8e4a-b6e59c1354d6" namespace="Xbmc" xmlSchemaNamespace="urn:Xbmc" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="XbmcConfigurationSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="xbmcConfigurationSection">
      <elementProperties>
        <elementProperty name="Hosts" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="hosts" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/XbmcHosts" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="XbmcHost">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Host" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Port" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="port" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="Login" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="login" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Password" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Interval" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="interval" isReadOnly="false" defaultValue="1000">
          <type>
            <externalTypeMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="XbmcHosts" xmlItemName="xbmcHost" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/04931c4c-3e96-47d5-8e4a-b6e59c1354d6/XbmcHost" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>
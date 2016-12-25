<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="a1277f7f-fb30-4057-bf5c-864011fc8fca" namespace="Snmp" xmlSchemaNamespace="urn:Snmp" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="SnmpConfiguration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="snmpConfiguration">
      <attributeProperties>
        <attributeProperty name="QueryInterval" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="queryInterval" isReadOnly="false" defaultValue="&quot;00:00:05&quot;">
          <type>
            <externalTypeMoniker name="/a1277f7f-fb30-4057-bf5c-864011fc8fca/TimeSpan" />
          </type>
        </attributeProperty>
        <attributeProperty name="MultipleStateObjectsPerDevice" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="multipleStateObjectsPerDevice" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/a1277f7f-fb30-4057-bf5c-864011fc8fca/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Devices" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="devices" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/a1277f7f-fb30-4057-bf5c-864011fc8fca/DeviceCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="Device">
      <attributeProperties>
        <attributeProperty name="Host" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/a1277f7f-fb30-4057-bf5c-864011fc8fca/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Community" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="community" isReadOnly="false" defaultValue="&quot;public&quot;">
          <type>
            <externalTypeMoniker name="/a1277f7f-fb30-4057-bf5c-864011fc8fca/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="DeviceCollection" xmlItemName="device" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/a1277f7f-fb30-4057-bf5c-864011fc8fca/Device" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>
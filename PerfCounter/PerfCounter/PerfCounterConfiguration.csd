<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="525b5847-025a-4fa1-8b78-3043018a871e" namespace="PerfCounter" xmlSchemaNamespace="urn:PerfCounter" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="PerfCounterSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="perfCounterSection">
      <elementProperties>
        <elementProperty name="PerfCounters" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="perfCounters" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/PerfCounterCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="PerfCounterCollection" xmlItemName="perfCounter" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/PerfCounter" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="PerfCounter">
      <attributeProperties>
        <attributeProperty name="CategoryName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="categoryName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="CounterName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="counterName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="InstanceName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="instanceName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="MachineName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="machineName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="ID" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="id" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/525b5847-025a-4fa1-8b78-3043018a871e/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>
<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="1447c7e2-1cc7-4bab-b403-1d2e4ff1e247" namespace="SSound" xmlSchemaNamespace="urn:SSound" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="SSoundConfigurationSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="ssoundConfiguration">
      <attributeProperties>
        <attributeProperty name="EndpointName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="endpointName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="EnableDlnaRenderer" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="enableDlnaRenderer" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="InitialVolume" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="initialVolume" isReadOnly="false" defaultValue="-1.0">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Double" />
          </type>
        </attributeProperty>
        <attributeProperty name="OutputDeviceName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="outputDeviceName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="SpeechVolume" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="speechVolume" isReadOnly="false" defaultValue="-1.0">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Double" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Inputs" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="inputs" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/InputDeviceCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Cerevoice" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="cerevoice" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/CerevoiceElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="InputDeviceElement">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="InputDeviceName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="inputDeviceName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="HasSignalDuration" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="hasSignalDuration" isReadOnly="false" defaultValue="2000">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="NoSignalDuration" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="noSignalDuration" isReadOnly="false" defaultValue="30000">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="BufferMilliseconds" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="bufferMilliseconds" isReadOnly="false" defaultValue="25">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="SignalThreshold" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="signalThreshold" isReadOnly="false" defaultValue="5.0">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Double" />
          </type>
        </attributeProperty>
        <attributeProperty name="AutoPlay" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="autoPlay" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="InputDeviceCollection" xmlItemName="device" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/InputDeviceElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="CerevoiceElement">
      <attributeProperties>
        <attributeProperty name="AccountID" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="accountID" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Password" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="VoiceName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="voiceName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Bitrate" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="bitrate" isReadOnly="false" defaultValue="48000">
          <type>
            <externalTypeMoniker name="/1447c7e2-1cc7-4bab-b403-1d2e4ff1e247/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>
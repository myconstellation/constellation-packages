using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Paradox.HomeAssistant.DiscoveryConfig.Contracts
{
    /// <summary>
    /// The MQTT discovery base configuration.
    /// </summary>
    public abstract class MqttDiscoveryConfig
    {
        /// <summary>
        /// Gets the HA component type.
        /// </summary>
        [JsonIgnore]
        public abstract string Component { get; }

        /// <summary>
        /// Gets or sets the name of the MQTT sensor.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// An ID that uniquely identifies this sensor. If two sensors have the same unique ID, Home Assistant will raise an exception.
        /// </summary>
        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Information about the device this sensor is a part of to tie it into the device registry. Only works through MQTT discovery and when <see cref="UniqueId"/> is set. At least one of identifiers or connections must be present to identify the device.
        /// </summary>
        [JsonProperty("device")]
        public MqttDiscoveryDevice Device { get; set; }

        /// <summary>
        /// A list of MQTT topics subscribed to receive availability (online/offline) updates. Must not be used together with availability_topic
        /// </summary>
        [JsonProperty("availability")]
        public List<MqttDiscoveryAvailablilty> Availability { get; set; }

        ///<summary>
        /// Defines a template to extract device’s availability from the availability_topic. To determine the devices’s availability result of this template will be compared to payload_available and payload_not_available.
        ///</summary> 
        [JsonProperty("availability_template")]
        public string AvailabilityTemplate { get; set; }

        ///<summary>
        /// The MQTT topic subscribed to receive availability (online/offline) updates. Must not be used together with availability.
        ///</summary> 
        [JsonProperty("availability_topic")]
        public string AvailabilityTopic { get; set; }

        /// <summary>
        /// When <see cref="Availability"/> is configured, this controls the conditions needed to set the entity to available.
        /// <br/>
        /// If set to <see cref="MqttDiscoveryAvailabilityMode.All"/>, payload_available must be received on all configured availability topics before the entity is marked as online.
        /// <br/>
        /// If set to <see cref="MqttDiscoveryAvailabilityMode.Any"/>, payload_available must be received on at least one configured availability topic before the entity is marked as online.
        /// <br/>
        /// If set to <see cref="MqttDiscoveryAvailabilityMode.Latest"/>, the last payload_available or payload_not_available received on any configured availability topic controls the availability.
        /// </summary>
        [JsonProperty("availability_mode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MqttDiscoveryAvailabilityMode? AvailabilityMode { get; set; }

        /// <summary>
        /// <see href="https://www.home-assistant.io/docs/configuration/customizing-devices/#icon">Icon</see> for the entity.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    /// <summary>
    /// MQTT Discovery availability mode
    /// </summary>
    public enum MqttDiscoveryAvailabilityMode
    {
        /// <summary>
        /// The payload_available must be received on all configured availability topics before the entity is marked as online
        /// </summary>
        [EnumMember(Value = @"all")]
        All,
        /// <summary>
        /// The payload_available must be received on at least one configured availability topic before the entity is marked as online
        /// </summary>
        [EnumMember(Value = @"any")]
        Any,
        /// <summary>
        /// The last payload_available or payload_not_available received on any configured availability topic controls the availability.
        /// </summary>
        [EnumMember(Value = @"latest")]
        Latest,
    }

    /// <summary>
    /// A list of MQTT topics subscribed to receive availability (online/offline) updates. Must not be used together with AvailabilityTopic.
    /// </summary>
    public class MqttDiscoveryAvailablilty
    {
        /// <summary>
        /// The payload that represents the available state.
        /// </summary>
        [JsonProperty("payload_available")]
        public object PayloadAvailable { get; set; }

        /// <summary>
        /// The payload that represents the unavailable state.
        /// </summary>
        [JsonProperty("payload_not_available")]
        public object PayloadNotAvailable { get; set; }

        /// <summary>
        /// An MQTT topic subscribed to receive availability (online/offline) updates.
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// Defines a template to extract device’s availability from the topic. To determine the devices’s availability result of this template will be compared to payload_available and payload_not_available.
        /// </summary>
        [JsonProperty("value_template")]
        public string ValueTemplate { get; set; }
    }

    /// <summary>
    /// Information about the device this sensor is a part of to tie it into the device registry. 
    /// </summary>
    public class MqttDiscoveryDevice
    {
        /// <summary>
        /// The name of the device.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The manufacturer of the device.
        /// </summary>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model of the device.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Suggest an area if the device isn’t in one yet.
        /// </summary>
        [JsonProperty("suggested_area")]
        public string SuggestedArea { get; set; }

        /// <summary>
        /// The firmware version of the device.
        /// </summary>
        [JsonProperty("sw_version")]
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Identifier of a device that routes messages between this device and Home Assistant. Examples of such devices are hubs, or parent devices of a sub-device. This is used to show device topology in Home Assistant.
        /// </summary>
        [JsonProperty("via_device")]
        public string ViaDevice { get; set; }

        /// <summary>
        /// A link to the webpage that can manage the configuration of this device. Can be either an HTTP or HTTPS link.
        /// </summary>
        [JsonProperty("configuration_url")]
        public string ConfigurationUrl { get; set; }

        /// <summary>
        /// A list of connections of the device to the outside world as a list of tuples [connection_type, connection_identifier]. For example the MAC address of a network interface: "connections": [["mac", "02:5b:26:a8:dc:12"]].
        /// </summary>
        [JsonProperty("connections")]
        public List<List<string>> Connections { get; set; }

        /// <summary>
        /// A list of IDs that uniquely identify the device. For example a serial number.
        /// </summary>
        [JsonProperty("identifiers")]
        public List<string> Identifiers { get; set; }
    }
}

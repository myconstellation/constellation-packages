using Newtonsoft.Json;

namespace Paradox.HomeAssistant.DiscoveryConfig.Contracts
{
    /// <summary>
    /// The MQTT binary sensor platform uses an MQTT message received to set the binary sensor’s state to on or off.
    /// Original: https://github.com/JonasMH/ToMqttNet/blob/main/src/ToMqttNet/DeviceTypes/MqttBinarySensorDiscoveryConfig.cs
    /// </summary>
    public class MqttBinarySensorDiscoveryConfig : MqttDiscoveryConfig
    {
        /// <summary>
        /// Gets the HA component type.
        /// </summary>
        public override string Component => "binary_sensor";

        ///<summary>
        /// Sets the class of the device, changing the device state and icon that is displayed on the frontend.
        /// https://www.home-assistant.io/integrations/binary_sensor/#device-class
        ///</summary> 
        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }

        ///<summary>
        /// Flag which defines if the entity should be enabled when first added.
        /// , default: true
        ///</summary> 
        [JsonProperty("enabled_by_default")]
        public bool? EnabledByDefault { get; set; }

        ///<summary>
        /// The encoding of the payloads received. Set to "" to disable decoding of incoming payload.
        /// , default: utf-8
        ///</summary> 
        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        ///<summary>
        /// The category of the entity.
        /// , default: None
        ///</summary> 
        [JsonProperty("entity_category")]
        public string EntityCategory { get; set; }

        ///<summary>
        /// Defines the number of seconds after the sensor’s state expires, if it’s not updated. After expiry, the sensor’s state becomes unavailable.
        ///</summary> 
        [JsonProperty("expire_after")]
        public long? ExpireAfter { get; set; }

        ///<summary>
        /// Sends update events (which results in update of state object’s last_changed) even if the sensor’s state hasn’t changed. Useful if you want to have meaningful value graphs in history or want to create an automation that triggers on every incoming state message (not only when the sensor’s new state is different to the current one).
        /// , default: false
        ///</summary> 
        [JsonProperty("force_update")]
        public bool? ForceUpdate { get; set; }

        ///<summary>
        /// Defines a template to extract the JSON dictionary from messages received on the json_attributes_topic. Usage example can be found in MQTT sensor documentation.
        ///</summary> 
        [JsonProperty("json_attributes_template")]
        public string JsonAttributesTemplate { get; set; }

        ///<summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes. Usage example can be found in MQTT sensor documentation.
        ///</summary> 
        [JsonProperty("json_attributes_topic")]
        public string JsonAttributesTopic { get; set; }

        ///<summary>
        /// Used instead of name for automatic generation of entity_id
        ///</summary> 
        [JsonProperty("object_id")]
        public string ObjectId { get; set; }

        ///<summary>
        /// For sensors that only send on state updates (like PIRs), this variable sets a delay in seconds after which the sensor’s state will be updated back to off.
        ///</summary> 
        [JsonProperty("off_delay")]
        public long? OffDelay { get; set; }

        ///<summary>
        /// The string that represents the online state.
        /// , default: online
        ///</summary> 
        [JsonProperty("payload_available")]
        public string PayloadAvailable { get; set; }

        ///<summary>
        /// The string that represents the offline state.
        /// , default: offline
        ///</summary> 
        [JsonProperty("payload_not_available")]
        public string PayloadNotAvailable { get; set; }

        ///<summary>
        /// The string that represents the off state. It will be compared to the message in the state_topic (see value_template for details)
        /// , default: OFF
        ///</summary> 
        [JsonProperty("payload_off")]
        public string PayloadOff { get; set; }

        ///<summary>
        /// The string that represents the on state. It will be compared to the message in the state_topic (see value_template for details)
        /// , default: ON
        ///</summary> 
        [JsonProperty("payload_on")]
        public string PayloadOn { get; set; }

        ///<summary>
        /// The maximum QoS level to be used when receiving messages.
        /// , default: 0
        ///</summary> 
        [JsonProperty("qos")]
        public long? Qos { get; set; }

        ///<summary>
        /// The MQTT topic subscribed to receive sensor’s state.
        ///</summary> 
        [JsonProperty("state_topic")]
        public string StateTopic { get; set; }

        ///<summary>
        /// Defines a template that returns a string to be compared to payload_on/payload_off or an empty string, in which case the MQTT message will be removed. Available variables: entity_id. Remove this option when ‘payload_on’ and ‘payload_off’ are sufficient to match your payloads (i.e no pre-processing of original message is required).
        ///</summary> 
        [JsonProperty("value_template")]
        public string ValueTemplate { get; set; }
    }
}

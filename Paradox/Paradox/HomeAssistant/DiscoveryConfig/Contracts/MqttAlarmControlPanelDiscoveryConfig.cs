using Newtonsoft.Json;

namespace Paradox.HomeAssistant.DiscoveryConfig.Contracts
{

    /// <summary>
    /// The MQTT alarm panel platform enables the possibility to control MQTT capable alarm panels. The Alarm icon will change state after receiving a new state from state_topic. If these messages are published with RETAIN flag, the MQTT alarm panel will receive an instant state update after subscription and will start with the correct state. Otherwise, the initial state will be unknown.
    /// Original: https://github.com/JonasMH/ToMqttNet/blob/main/src/ToMqttNet/DeviceTypes/MqttAlarmControlPanelDiscoveryConfig.cs
    /// </summary>
    public class MqttAlarmControlPanelDiscoveryConfig : MqttDiscoveryConfig
    {
        /// <summary>
        /// Gets the HA component type.
        /// </summary>
        public override string Component => "alarm_control_panel";

        ///<summary>
        /// If defined, specifies a code to enable or disable the alarm in the frontend. Note that the code is validated locally and blocks sending MQTT messages to the remote device. For remote code validation, the code can be configured to either of the special values REMOTE_CODE (numeric code) or REMOTE_CODE_TEXT (text code). In this case, local code validation is bypassed but the frontend will still show a numeric or text code dialog. Use command_template to send the code to the remote device. Example configurations for remote code validation can be found here.
        ///</summary> 
        [JsonProperty("code")]
        public string Code { get; set; }

        ///<summary>
        /// If true the code is required to arm the alarm. If false the code is not validated.
        /// , default: true
        ///</summary> 
        [JsonProperty("code_arm_required")]
        public bool? CodeArmRequired { get; set; }

        ///<summary>
        /// If true the code is required to disarm the alarm. If false the code is not validated.
        /// , default: true
        ///</summary> 
        [JsonProperty("code_disarm_required")]
        public bool? CodeDisarmRequired { get; set; }

        ///<summary>
        /// If true the code is required to trigger the alarm. If false the code is not validated.
        /// , default: true
        ///</summary> 
        [JsonProperty("code_trigger_required")]
        public bool? CodeTriggerRequired { get; set; }

        ///<summary>
        /// The template used for the command payload. Available variables: action and code.
        /// , default: action
        ///</summary> 
        [JsonProperty("command_template")]
        public string CommandTemplate { get; set; }

        ///<summary>
        /// The MQTT topic to publish commands to change the alarm state.
        ///</summary> 
        [JsonProperty("command_topic")]
        public string CommandTopic { get; set; }

        ///<summary>
        /// Flag which defines if the entity should be enabled when first added.
        /// , default: true
        ///</summary> 
        [JsonProperty("enabled_by_default")]
        public bool? EnabledByDefault { get; set; }

        ///<summary>
        /// The encoding of the payloads received and published messages. Set to "" to disable decoding of incoming payload.
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
        /// The payload to set armed-away mode on your Alarm Panel.
        /// , default: ARM_AWAY
        ///</summary> 
        [JsonProperty("payload_arm_away")]
        public string PayloadArmAway { get; set; }

        ///<summary>
        /// The payload to set armed-home mode on your Alarm Panel.
        /// , default: ARM_HOME
        ///</summary> 
        [JsonProperty("payload_arm_home")]
        public string PayloadArmHome { get; set; }

        ///<summary>
        /// The payload to set armed-night mode on your Alarm Panel.
        /// , default: ARM_NIGHT
        ///</summary> 
        [JsonProperty("payload_arm_night")]
        public string PayloadArmNight { get; set; }

        ///<summary>
        /// The payload to set armed-vacation mode on your Alarm Panel.
        /// , default: ARM_VACATION
        ///</summary> 
        [JsonProperty("payload_arm_vacation")]
        public string PayloadArmVacation { get; set; }

        ///<summary>
        /// The payload to set armed-custom-bypass mode on your Alarm Panel.
        /// , default: ARM_CUSTOM_BYPASS
        ///</summary> 
        [JsonProperty("payload_arm_custom_bypass")]
        public string PayloadArmCustomBypass { get; set; }

        ///<summary>
        /// The payload that represents the available state.
        /// , default: online
        ///</summary> 
        [JsonProperty("payload_available")]
        public string PayloadAvailable { get; set; }

        ///<summary>
        /// The payload to disarm your Alarm Panel.
        /// , default: DISARM
        ///</summary> 
        [JsonProperty("payload_disarm")]
        public string PayloadDisarm { get; set; }

        ///<summary>
        /// The payload that represents the unavailable state.
        /// , default: offline
        ///</summary> 
        [JsonProperty("payload_not_available")]
        public string PayloadNotAvailable { get; set; }

        ///<summary>
        /// The payload to trigger the alarm on your Alarm Panel.
        /// , default: TRIGGER
        ///</summary> 
        [JsonProperty("payload_trigger")]
        public string PayloadTrigger { get; set; }

        ///<summary>
        /// The maximum QoS level of the state topic.
        /// , default: 0
        ///</summary> 
        [JsonProperty("qos")]
        public long? Qos { get; set; }

        ///<summary>
        /// If the published message should have the retain flag on or not.
        /// , default: false
        ///</summary> 
        [JsonProperty("retain")]
        public bool? Retain { get; set; }

        ///<summary>
        /// The MQTT topic subscribed to receive state updates.
        ///</summary> 
        [JsonProperty("state_topic")]
        public string StateTopic { get; set; }

        ///<summary>
        /// Defines a template to extract the value.
        ///</summary> 
        [JsonProperty("value_template")]
        public string ValueTemplate { get; set; }
    }
}

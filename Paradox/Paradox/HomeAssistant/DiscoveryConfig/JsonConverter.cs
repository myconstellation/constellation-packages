using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Paradox.HomeAssistant.DiscoveryConfig.Contracts;

namespace Paradox.HomeAssistant.DiscoveryConfig
{
    /// <summary>
    /// JSON converter for HA MQTT Discovery
    /// </summary>
    public static class JsonConverter
    {
        /// <summary>
        /// Gets the JSON settings for HA MQTT Discovery.
        /// </summary>
        public static JsonSerializerSettings DiscoveryJsonSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            },
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
        };

        /// <summary>
        /// Converts MqttDiscoveryConfig to JSON.
        /// </summary>
        /// <typeparam name="T">MqttDiscoveryConfig type</typeparam>
        /// <param name="config">The MQTT Discovery configuration.</param>
        /// <returns></returns>
        public static string ToJson<T>(this T config) where T : MqttDiscoveryConfig
        {
            return JsonConvert.SerializeObject(config, DiscoveryJsonSettings);
        }
    }
}

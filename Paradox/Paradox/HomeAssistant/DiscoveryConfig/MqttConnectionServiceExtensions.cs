using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Paradox.HomeAssistant.DiscoveryConfig.Contracts;
using System.Threading.Tasks;

namespace Paradox.HomeAssistant.DiscoveryConfig
{
    /// <summary>
    /// MQTT connection extensions
    /// </summary>
    public static class MqttConnectionServiceExtensions
    {
        /// <summary>
        /// Publishes the HA MQTT discovery document.
        /// </summary>
        /// <typeparam name="T">MqttDiscoveryConfig type</typeparam>
        /// <param name="client">The MQTT client.</param>
        /// <param name="config">The MQTT discovery configuration.</param>
        public static Task PublishDiscoveryDocument<T>(this IManagedMqttClient client, T config) where T : MqttDiscoveryConfig
        {
            return client.EnqueueAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic($"homeassistant/{config.Component}/{config.UniqueId}/config")
                    .WithRetainFlag()
                    .WithPayload(config.ToJson())
                    .Build());
        }
    }
}

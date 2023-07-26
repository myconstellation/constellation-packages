using System.Collections.Generic;

namespace Paradox.HomeAssistant
{
    /// <summary>
    /// HomeAssistant package configuration
    /// </summary>
    public class HomeAssistantConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the HomeAssistant MQTT integration is enable (optional, default: false).
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the name of the alarm on Home Assistant (optional, default: 'Paradox Security System').
        /// </summary>
        public string Label { get; set; } = "Paradox Security System";

        /// <summary>
        /// Gets or sets the base identifier of the alarm on Home Assistant (optional, default: 'Paradox').
        /// </summary>
        public string Identifier { get; set; } = nameof(Paradox);

        /// <summary>
        /// Gets or sets the PIN code to enable or disable the alarm without code on the frontend. If not defined, the PIN code will be requested on the frontend and it will be sent on MQTT to the package (optional, default: None).
        /// </summary>
        public string PIN { get; set; }

        /// <summary>
        /// Gets or sets the MQTT configuration (required if enable).
        /// </summary>
        public MqttConfiguration Mqtt { get; set; }

        /// <summary>
        /// Gets or sets the Paradox zone configuration (optional)
        /// </summary>
        public List<ZoneConfiguration> Zones { get; set; }

        /// <summary>
        /// Defines the MQTT configuration
        /// </summary>
        public class MqttConfiguration
        {
            /// <summary>
            /// Gets or sets the MQTT server (required).
            /// </summary>
            public string Server { get; set; }

            /// <summary>
            /// Gets or sets the MQTT port (optional, default: 1883).
            /// </summary>
            public int Port { get; set; } = 1883;

            /// <summary>
            /// Gets or sets the MQTT username (optional).
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the MQTT password (optional).
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Gets or sets the MQTT client identifier (optional, default: Identifier).
            /// </summary>
            public string ClientId { get; set; }
        }

        /// <summary>
        /// Defines the sensor configurations between Home Assitant and Paradox
        /// </summary>
        public class ZoneConfiguration
        {
            /// <summary>
            /// Gets or sets the Paradox zone number (required).
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the name of the sensor (optional, default: the Paradox zone label).
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets the manufacturer of the sensor (optional, default: Paradox).
            /// </summary>
            public string Manufacturer { get; set; }

            /// <summary>
            /// Gets or sets the model of the sensor (optional).
            /// </summary>
            public string Model { get; set; }

            /// <summary>
            /// Gets or sets the suggested area (optional).
            /// </summary>
            public string Area { get; set; }

            /// <summary>
            /// Gets or sets the HomeAssistant sensor type (optional, default: None).
            /// See: https://www.home-assistant.io/integrations/binary_sensor/#device-class
            /// Eg: door, garage_door, motion, tamper, smoke, vibration, window
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ZoneConfiguration"/> is battery-based (optional, default: False).
            /// </summary>
            public bool Battery { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ZoneConfiguration"/> is a fire zone (optional, default: False).
            /// </summary>
            public bool Fire { get; set; }
        }
    }
}

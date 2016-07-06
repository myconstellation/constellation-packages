
namespace NetworkTools
{
    using Constellation.Package;

    [StateObject]
    public class MonitoringResult
    {
        /// <summary>
        /// Gets or sets the response time in millisecond.
        /// </summary>
        /// <value>
        /// The response time in millisecond..
        /// </value>
        public long ResponseTime { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        ///   <c>true</c> if OK; otherwise, <c>false</c>.
        /// </value>
        public bool State { get; set; }

        /// <summary>
        /// Monitoring type
        /// </summary>
        public enum MonitoringType
        {

            /// <summary>
            /// ICMP Echo
            /// </summary>
            Ping,
            /// <summary>
            /// Check TCP port
            /// </summary>
            Tcp,
            /// <summary>
            /// Check HTTP response
            /// </summary>
            Http
        }
    }
}

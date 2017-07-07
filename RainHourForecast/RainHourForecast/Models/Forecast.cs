using Constellation;
using Constellation.Package;
using System.Collections.Generic;

namespace RainHourForecast.Models
{
    /// <summary>
    /// Represent the result for 5 minutes
    /// </summary>
    [StateObject]
    public class DataCadran
    {
        /// <summary>
        /// Rain level in text
        /// </summary>
        public string NiveauPluieText { get; set; }
        /// <summary>
        /// Rain level
        /// </summary>
        public int NiveauPluie { get; set; }
        /// <summary>
        /// Rain color based on level
        /// </summary>
        public string Color { get; set; }
    }

    /// <summary>
    /// Represent the response from the service
    /// </summary>
    [StateObject]
    public class Forecast
    {
        /// <summary>
        /// Town's id
        /// </summary>
        public string IdLieu { get; set; }
        /// <summary>
        /// Starting date and hour of the forecast
        /// </summary>
        public string Echeance { get; set; }
        /// <summary>
        /// Forecast last update time
        /// </summary>
        public string LastUpdate { get; set; }
        /// <summary>
        /// True if avaible
        /// </summary>
        public bool IsAvailable { get; set; }
        /// <summary>
        /// True if there is data
        /// </summary>
        public bool HasData { get; set; }
        /// <summary>
        /// Rain forecast summary for the next hour in text
        /// </summary>
        public List<string> NiveauPluieText { get; set; }
        /// <summary>
        /// Rain forecast for the next hour
        /// </summary>
        public List<DataCadran> DataCadran { get; set; }
    }
}

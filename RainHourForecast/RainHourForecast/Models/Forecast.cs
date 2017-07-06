using System.Collections.Generic;

namespace RainHourForecast.Models
{
    /// <summary>
    /// Represent the result for 5 minutes
    /// </summary>
    public class DataCadran
    {
        /// <summary>
        /// Rain level in text
        /// </summary>
        public string niveauPluieText { get; set; }
        /// <summary>
        /// Rain level
        /// </summary>
        public int niveauPluie { get; set; }
        /// <summary>
        /// Rain color based on level
        /// </summary>
        public string color { get; set; }
    }

    /// <summary>
    /// Represent the response from the service
    /// </summary>
    public class Forecast
    {
        /// <summary>
        /// Town's id
        /// </summary>
        public string idLieu { get; set; }
        /// <summary>
        /// Starting date and hour of the forecast
        /// </summary>
        public string echeance { get; set; }
        /// <summary>
        /// Forecast last update time
        /// </summary>
        public string lastUpdate { get; set; }
        /// <summary>
        /// True if avaible
        /// </summary>
        public bool isAvailable { get; set; }
        /// <summary>
        /// True if there is data
        /// </summary>
        public bool hasData { get; set; }
        /// <summary>
        /// Rain forecast summary for the next hour in text
        /// </summary>
        public List<string> niveauPluieText { get; set; }
        /// <summary>
        /// Rain forecast for the next hour
        /// </summary>
        public List<DataCadran> dataCadran { get; set; }
    }
}

namespace RainHourForecast.Models
{
    /// <summary>
    /// Represent the response from the service when searching for a town
    /// </summary>
    public class Town
    {
        /// <summary>
        /// Town's id to use to get forecast
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Town's name
        /// </summary>
        public string nomAffiche { get; set; }

        /// <summary>
        /// Town's postal code
        /// </summary>
        public int codePostal { get; set; }
    }
}

namespace OpenWeatherAPI
{
	public class API
	{
		private string openWeatherAPIKey;
		private Language language;

		public API(string apiKey, Language? lang )
		{
			openWeatherAPIKey = apiKey;
			language = lang.HasValue ? lang.Value : Language.en;
		}

		public void UpdateAPIKey(string apiKey)
		{
			openWeatherAPIKey = apiKey;
		}
		public WeatherInfo QueryWeather(float longitude, float latitude)
		{
			WeatherInfo newQuery = new WeatherInfo(openWeatherAPIKey, longitude, latitude, language);
			return newQuery;
		}
	}
}

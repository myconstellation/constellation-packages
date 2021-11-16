using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpenWeatherAPI
{
	public class CurrentWeather : WeatherBase
	{
		public string Base { get; internal set; }
		public double Visibility { get; internal set; }

		public CurrentWeather(JToken weatherData) : base(weatherData)
		{
			if (weatherData is null)
				throw new System.ArgumentNullException(nameof(weatherData));

			if (weatherData.SelectToken("base") != null)
				this.Base = weatherData.SelectToken("base").ToString();
			if (weatherData.SelectToken("visibility") != null)
				this.Visibility = double.Parse(weatherData.SelectToken("visibility").ToString(), CultureInfo.CurrentCulture);

		}
	}
}

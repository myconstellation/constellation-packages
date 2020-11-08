using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpenWeatherAPI
{
	public class WeatherBase
	{
		public DateTime Dt { get; internal set; }
		public List<Weather> Weathers { get; internal set; } = new List<Weather>();
		public MainInfo MainInfo { get; internal set; }
		public Clouds Clouds { get; internal set; }
		public Wind Wind { get; internal set; }
		public Rain Rain { get; internal set; }
		public Snow Snow { get; internal set; }

		public WeatherBase(JToken weatherData)
		{
			if (weatherData is null)
				throw new System.ArgumentNullException(nameof(weatherData));

			foreach (JToken weather in weatherData.SelectToken("weather"))
				this.Weathers.Add(new Weather(weather));
			this.MainInfo = new MainInfo(weatherData.SelectToken("main"));
			this.Wind = new Wind(weatherData.SelectToken("wind"));
			if (weatherData.SelectToken("rain") != null)
				this.Rain = new Rain(weatherData.SelectToken("rain"));
			if (weatherData.SelectToken("snow") != null)
				this.Snow = new Snow(weatherData.SelectToken("snow"));
			this.Clouds = new Clouds(weatherData.SelectToken("clouds"));
			this.Dt = Helper.convertUnixToDateTime(long.Parse(weatherData.SelectToken("dt").ToString(), CultureInfo.CurrentCulture));
		}
	}
}

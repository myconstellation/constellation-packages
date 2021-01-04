using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherAPI
{
    public class WeatherInfo
    {
        static readonly HttpClient client = new HttpClient();

        public int Cod { get; }
        public int CityID { get; }
        public string Name { get; }
        public string Country { get; }
        public DateTime Sunrise { get; }
        public DateTime Sunset { get; }
        public int Timezone { get; }
        public Coord Coord { get; }
        public double Message { get; }

        public CurrentWeather CurrentWeather { get; }

        public List<WeatherBase> ForecastWeather { get; } = new List<WeatherBase>();


        public bool ValidRequestWeather { get; }
        public bool ValidRequestForecast { get; }

        public Exception LastEx { get; }


        public WeatherInfo(string apiKey, float longitude, float latitude, OpenWeatherAPI.Language language)
        {
            string baseUri = "https://api.openweathermap.org/data/2.5";
            string currentWeatherRoute = "/weather";
            string weatherForecastRoute = "/forecast";
            string queryString = $"?appid={apiKey}&lat={latitude}&lon={longitude}&lang={language}";

            JObject jsonData = null;
            try
            {
                jsonData = JObject.Parse(client.GetStringAsync(new Uri($"{baseUri}{currentWeatherRoute}{queryString}")).Result);
            }
            catch (AggregateException ex)
            {
                this.LastEx = ex;
            }

            if (jsonData != null && jsonData.SelectToken("cod").ToString() == "200")
            {
                ValidRequestWeather = true;
                CurrentWeather = new CurrentWeather(jsonData);

                Coord = new Coord(jsonData.SelectToken("coord"));
                CityID = int.Parse(jsonData.SelectToken("id").ToString(), CultureInfo.CurrentCulture);
                Name = jsonData.SelectToken("name").ToString();
                Cod = int.Parse(jsonData.SelectToken("cod").ToString(), CultureInfo.CurrentCulture);
                Timezone = int.Parse(jsonData.SelectToken("timezone").ToString(), CultureInfo.CurrentCulture);

                if (jsonData.SelectToken("sys") != null)
                {
                    JToken sysData = jsonData.SelectToken("sys");
                    if (sysData.SelectToken("country") != null)
                        Country = sysData.SelectToken("country").ToString();
                    if (sysData.SelectToken("sunrise") != null)
                        Sunrise = Helper.convertUnixToDateTime(double.Parse(sysData.SelectToken("sunrise").ToString(), CultureInfo.CurrentCulture));
                    if (sysData.SelectToken("sunset") != null)
                        Sunset = Helper.convertUnixToDateTime(double.Parse(sysData.SelectToken("sunset").ToString(), CultureInfo.CurrentCulture));
                }
            }
            else
            {
                ValidRequestWeather = false;
            }

            jsonData = null;
            try
            {
                jsonData = JObject.Parse(client.GetStringAsync(new Uri($"{baseUri}{weatherForecastRoute}{queryString}")).Result);
            }
            catch (AggregateException ex)
            {
                this.LastEx = ex;
            }

            if (jsonData != null && jsonData.SelectToken("cod").ToString() == "200")
            {
                ValidRequestForecast = true;
                if (jsonData.SelectToken("message") != null)
                    Message = double.Parse(jsonData.SelectToken("message").ToString(), CultureInfo.CurrentCulture);

                if (jsonData.SelectToken("city") != null)
                {
                    JToken cityData = jsonData.SelectToken("city");
                    if (cityData.SelectToken("id") != null)
                        CityID = int.Parse(cityData.SelectToken("id").ToString(), CultureInfo.CurrentCulture);
                    Name = cityData.SelectToken("name").ToString();

                    Coord = new Coord(cityData.SelectToken("coord"));
                    if (cityData.SelectToken("country") != null)
                        Country = cityData.SelectToken("country").ToString();

                    Timezone = int.Parse(cityData.SelectToken("timezone").ToString(), CultureInfo.CurrentCulture);
                    if (cityData.SelectToken("sunrise") != null)
                        Sunrise = Helper.convertUnixToDateTime(double.Parse(cityData.SelectToken("sunrise").ToString(), CultureInfo.CurrentCulture));
                    if (cityData.SelectToken("sunset") != null)
                        Sunset = Helper.convertUnixToDateTime(double.Parse(cityData.SelectToken("sunset").ToString(), CultureInfo.CurrentCulture));
                }

                foreach (JToken listItem in jsonData.SelectToken("list"))
                {
                    this.ForecastWeather.Add(new WeatherBase(listItem));
                }
            }
            else
            {
                ValidRequestForecast = false;
            }
        }
    }
}

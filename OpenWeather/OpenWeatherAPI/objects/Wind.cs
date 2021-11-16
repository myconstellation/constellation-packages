using Newtonsoft.Json.Linq;
using System.Globalization;

namespace OpenWeatherAPI
{
	public class Wind
	{
		public enum DirectionEnum
		{
			North,
			NorthNorthEast,
			NorthEast,
			EastNorthEast,
			East,
			EastSouthEast,
			SouthEast,
			SouthSouthEast,
			South,
			SouthSouthWest,
			SouthWest,
			WestSouthWest,
			West,
			WestNorthWest,
			NorthWest,
			NorthNorthWest,
			Unknown
		}

		public double SpeedMetersPerSecond { get; }

		public double SpeedFeetPerSecond { get; }

		public DirectionEnum Direction { get; }

		public double Degree { get; }

		public double Gust { get; }

		public Wind(JToken windData)
		{
			if (windData is null)
				throw new System.ArgumentNullException(nameof(windData));


			SpeedMetersPerSecond = double.Parse(windData.SelectToken("speed").ToString(), CultureInfo.CurrentCulture);
			SpeedFeetPerSecond = SpeedMetersPerSecond * 3.28084;
			if (windData.SelectToken("deg") != null)
				Degree = double.Parse(windData.SelectToken("deg").ToString(), CultureInfo.CurrentCulture);
			Direction = assignDirection(Degree);
			if (windData.SelectToken("gust") != null)
				Gust = double.Parse(windData.SelectToken("gust").ToString(), CultureInfo.CurrentCulture);
		}

		public static string DirectionEnumToString(DirectionEnum dir)
		{
			switch (dir)
			{
				case DirectionEnum.East:
					return "East";
				case DirectionEnum.EastNorthEast:
					return "East North-East";
				case DirectionEnum.EastSouthEast:
					return "East South-East";
				case DirectionEnum.North:
					return "North";
				case DirectionEnum.NorthEast:
					return "North East";
				case DirectionEnum.NorthNorthEast:
					return "North North-East";
				case DirectionEnum.NorthNorthWest:
					return "North North-West";
				case DirectionEnum.NorthWest:
					return "North West";
				case DirectionEnum.South:
					return "South";
				case DirectionEnum.SouthEast:
					return "South East";
				case DirectionEnum.SouthSouthEast:
					return "South South-East";
				case DirectionEnum.SouthSouthWest:
					return "South South-West";
				case DirectionEnum.SouthWest:
					return "South West";
				case DirectionEnum.West:
					return "West";
				case DirectionEnum.WestNorthWest:
					return "West North-West";
				case DirectionEnum.WestSouthWest:
					return "West South-West";
				case DirectionEnum.Unknown:
					return "Unknown";
				default:
					return "Unknown";
			}
		}

		private DirectionEnum assignDirection(double degree)
		{
			if (fB(degree, 348.75, 360))
				return DirectionEnum.North;
			if (fB(degree, 0, 11.25))
				return DirectionEnum.North;
			if (fB(degree, 11.25, 33.75))
				return DirectionEnum.NorthNorthEast;
			if (fB(degree, 33.75, 56.25))
				return DirectionEnum.NorthEast;
			if (fB(degree, 56.25, 78.75))
				return DirectionEnum.EastNorthEast;
			if (fB(degree, 78.75, 101.25))
				return DirectionEnum.East;
			if (fB(degree, 101.25, 123.75))
				return DirectionEnum.EastSouthEast;
			if (fB(degree, 123.75, 146.25))
				return DirectionEnum.SouthEast;
			if (fB(degree, 168.75, 191.25))
				return DirectionEnum.South;
			if (fB(degree, 191.25, 213.75))
				return DirectionEnum.SouthSouthWest;
			if (fB(degree, 213.75, 236.25))
				return DirectionEnum.SouthWest;
			if (fB(degree, 236.25, 258.75))
				return DirectionEnum.WestSouthWest;
			if (fB(degree, 258.75, 281.25))
				return DirectionEnum.West;
			if (fB(degree, 281.25, 303.75))
				return DirectionEnum.WestNorthWest;
			if (fB(degree, 303.75, 326.25))
				return DirectionEnum.NorthWest;
			if (fB(degree, 326.25, 348.75))
				return DirectionEnum.NorthNorthWest;
			return DirectionEnum.Unknown;
		}

		//fB = fallsBetween
		private static bool fB(double val, double min, double max)
		{
			if ((min <= val) && (val <= max))
				return true;
			return false;
		}
	}
}

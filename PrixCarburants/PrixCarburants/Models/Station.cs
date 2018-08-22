using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{
    [XmlRoot(ElementName = "pdv")]
    public class Station
    {
        [XmlElement(ElementName = "adresse")]
        public string Adress { get; set; }

        [XmlElement(ElementName = "ville")]
        public string City { get; set; }

        [XmlElement(ElementName = "horaires")]
        public SchedulesList Schedule { get; set; }

        [XmlElement(ElementName = "services")]
        public Services Services { get; set; }

        [XmlElement(ElementName = "prix")]
        public List<Price> Prices { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "latitude")]
        public double LatitudeRaw { get; set; }
        public double Latitude { get { return this.LatitudeRaw / 100000; } }

        [XmlAttribute(AttributeName = "longitude"), JsonIgnore]
        public double LongitudeRaw { get; set; }

        public double Longitude { get { return this.LongitudeRaw / 100000; } }

        [XmlAttribute(AttributeName = "cp")]
        public int PostalCode { get; set; }

        [XmlAttribute(AttributeName = "pop")]
        public string Pop { get; set; }
    }
}

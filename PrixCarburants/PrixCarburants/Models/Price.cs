using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{
    [XmlRoot(ElementName = "prix")]
    public class Price
    {
        [XmlAttribute(AttributeName = "nom")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public Fuel Id { get; set; }
        [XmlAttribute(AttributeName = "maj")]
        public string Maj { get; set; }
        [XmlAttribute(AttributeName = "valeur")]
        public double Value { get; set; }
    }
}

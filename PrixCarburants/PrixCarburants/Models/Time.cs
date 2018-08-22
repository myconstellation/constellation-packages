using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{
    [XmlRoot(ElementName = "horaire")]
    public class Time
    {
        [XmlAttribute(AttributeName = "ouverture")]
        public string Opening { get; set; }
        [XmlAttribute(AttributeName = "fermeture")]
        public string Closing { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{
    [XmlRoot(ElementName = "jour")]
    public class Schedule
    {
        [XmlElement(ElementName = "horaire")]
        public Time Times { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "nom")]
        public string Nom { get; set; }
        [XmlAttribute(AttributeName = "ferme"), JsonIgnore]
        public string ClosedRaw { get; set; }
        public bool Closed { get { return this.ClosedRaw == "1"; } }
    }
}

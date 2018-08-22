using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{
    [XmlRoot(ElementName = "pdv_liste")]
    public class StationsList
    {
        [XmlElement(ElementName = "pdv")]
        public List<Station> Stations { get; set; }
    }
}

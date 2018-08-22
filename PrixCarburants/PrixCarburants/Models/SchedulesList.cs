using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{

    [XmlRoot(ElementName = "horaires")]
    public class SchedulesList
    {
        [XmlElement(ElementName = "jour")]
        public List<Schedule> Schedules { get; set; }
        [XmlAttribute(AttributeName = "automate-24-24"), JsonIgnore]
        public string Automate2424Raw { get; set; }
        public bool Automate2424 { get { return this.Automate2424Raw == "1"; } }
    }
}

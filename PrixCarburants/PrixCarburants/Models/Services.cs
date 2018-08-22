using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{

    [XmlRoot(ElementName = "services")]
    public class Services
    {
        [XmlElement(ElementName = "service")]
        public List<string> Service { get; set; }
    }
}

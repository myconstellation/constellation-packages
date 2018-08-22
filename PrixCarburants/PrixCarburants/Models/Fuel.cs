using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrixCarburants.Models
{
    /// <summary>
    /// Fuel types
    /// </summary>
    public enum Fuel
    {
        /// <summary>
        /// Gazole
        /// </summary>
        [XmlEnum(Name ="1")]
        Gazole,

        /// <summary>
        /// Sans Plomb 95
        /// </summary>
        [XmlEnum(Name = "2")]
        SP95 = 2,

        /// <summary>
        /// Superéthanol E85
        /// </summary>
        [XmlEnum(Name = "3")]
        E85 = 3,

        /// <summary>
        /// Gaz de Pétrole Liquéfié carburant
        /// </summary>
        [XmlEnum(Name = "4")]
        GPLc = 4,

        /// <summary>
        /// E10
        /// </summary>
        [XmlEnum(Name = "5")]
        E10 = 5,

        /// <summary>
        /// Sans Plomb 98
        /// </summary>
        [XmlEnum(Name = "6")]
        SP98 = 6,

    }
}

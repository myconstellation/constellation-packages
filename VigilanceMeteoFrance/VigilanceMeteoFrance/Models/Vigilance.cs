using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VigilanceMeteoFrance.Models
{
    /// <summary>
    /// Vigilance.
    /// </summary>
    [StateObject]
    public class Vigilance
    {
        /// <summary>
        /// Level of the vigilance for the departement.
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// List of the vigilance.
        /// </summary>
        public List<Type> Type { get; set; }
    }

    /// <summary>
    /// Vigilance list.
    /// </summary>
    [StateObject]
    public class Type
    {
        /// <summary>
        /// Name of the vigilance
        /// </summary>
        public string Name { get; set; }
    }
}

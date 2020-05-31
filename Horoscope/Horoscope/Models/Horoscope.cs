using Constellation;
using Constellation.Package;
using System.Collections.Generic;

namespace Horoscope.Models
{
    /// <summary>
    /// Horoscopes.
    /// </summary>
    [StateObject]
    public class Horoscopes
    {
        /// <summary>
        /// Horoscope date.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Horoscope part text.
        /// </summary>
        public List<Section> Section { get; set; }
    }
    
    /// <summary>
    /// Horoscopes.
    /// </summary>
    [StateObject]
    public class Section
    {
        /// <summary>
        /// Horoscope part title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Horoscope part text.
        /// </summary>
        public string Horoscope { get; set; }
    }
}

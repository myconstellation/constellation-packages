namespace PanasonicTV.Remote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class EnumerationExtensions
    {
        /// <summary>
        /// Extract the description from the enum value
        /// </summary>
        /// <param name="value"> Enum value </param>
        /// <returns> Description value </returns>
        public static string To<T>(this Enum value) where T : DescriptionAttribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            T[] attributes =
                (T[])fi.GetCustomAttributes(
                typeof(T),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}

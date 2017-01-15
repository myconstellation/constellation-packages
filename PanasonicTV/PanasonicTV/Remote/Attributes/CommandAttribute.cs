namespace PanasonicTV.Remote.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CommandAttribute : DescriptionAttribute
    {
        public CommandAttribute(string key) : base(key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
        }
    }
}

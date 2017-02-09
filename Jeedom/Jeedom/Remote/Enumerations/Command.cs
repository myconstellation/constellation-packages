namespace Jeedom.Remote.Enumerations
{

    using Jeedom.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum Command
    {
        //// Switch command (without value)
        [Command("&id={0}")]
        Switch,

        //// Slider command (with value)
        [Command("&id={0}&slider={1}")]
        Slider,

        //// Message command (with value and value2)
        [Command("&id={0}&title={1}&message={2}")]
        Message,

        //// Color command (with value)
        [Command("&id={0}&color={1}")]
        Color,

    }
}

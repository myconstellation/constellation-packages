namespace Squeezebox.Remote.Enumerations
{

    using Squeezebox.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum ServerCommand
    {

        //// Cancel the actual scan
        [Command("\"abortscan\"")]
        Scan_Cancel,

        //// Launch fast scan
        [Command("\"wipecache\"")]
        Scan_Fast,

        //// Launch full scan
        [Command("\"rescan\",\"full\"")]
        Scan_Full,

    }
}

namespace FreeboxTV.Remote.Enumerations
{
    using FreeboxTV.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum FreeCommandType
    {
        [Command("false")]
        Short = 0,
        [Command("true")]
        Long = 1
    }
}

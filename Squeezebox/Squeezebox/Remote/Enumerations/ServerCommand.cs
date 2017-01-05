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

        [Command("\"mixer\",\"muting\"")]
        Mute_Toggle,

        [Command("\"mixer\",\"muting\",\"1\"")]
        Mute_Off,

        [Command("\"mixer\",\"muting\",\"0\"")]
        Mute_On,

        [Command("\"pause\",\"1\"")]
        Pause,

        [Command("\"pause\",\"0\"")]
        Play,

        [Command("\"pause\"")]
        Play_Toggle,

        [Command("\"power\",\"0\"")]
        Power_Off,

        [Command("\"power\",\"1\"")]
        Power_On,

        [Command("\"power\"")]
        Power_Toggle,

        [Command("\"abortscan\"")]
        Scan_Cancel,

        [Command("\"wipecache\"")]
        Scan_Fast,

        [Command("\"rescan\",\"full\"")]
        Scan_Full,

        [Command("\"stop\"")]
        Stop,

        [Command("\"sync\",\"-\"")]
        Sync_Off,

        [Command("\"sync\",\"{0}\"")]
        Sync_To,

        [Command("\"mixer\",\"volume\",{0}")]
        Volume,

        [Command("\"mixer\",\"volume\",\"-2\"")]
        Volume_Down,

        [Command("\"mixer\",\"volume\",\"+2\"")]
        Volume_Up,

    }
}

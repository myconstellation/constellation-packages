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
        //// Toggle muting for all the players
        [Command("\"mixer\",\"muting\"")]
        Mute_Toggle,

        //// Disable muting for all the players
        [Command("\"mixer\",\"muting\",\"1\"")]
        Mute_Off,

        //// Mute all the players
        [Command("\"mixer\",\"muting\",\"0\"")]
        Mute_On,

        //// Pause all the players
        [Command("\"pause\",\"1\"")]
        Pause,

        //// Play all the players
        [Command("\"pause\",\"0\"")]
        Play,

        //// Toggle play and pause state for all the players
        [Command("\"pause\"")]
        Play_Toggle,

        //// Power off all the players
        [Command("\"power\",\"0\"")]
        Power_Off,

        //// Power On all the players
        [Command("\"power\",\"1\"")]
        Power_On,

        //// Toggle power for all the players
        [Command("\"power\"")]
        Power_Toggle,

        //// Cancel the actual scan
        [Command("\"abortscan\"")]
        Scan_Cancel,

        //// Launch fast scan
        [Command("\"wipecache\"")]
        Scan_Fast,

        //// Launch full scan
        [Command("\"rescan\",\"full\"")]
        Scan_Full,

        //// Stop all the players
        [Command("\"stop\"")]
        Stop,

        //// Disable syncing for all the players
        [Command("\"sync\",\"-\"")]
        Sync_Off,

        //// Sync all the players to a target player (require target player name)
        [Command("\"sync\",\"{0}\"")]
        Sync_To,

        //// Set the volume of all the players (require volume level)
        [Command("\"mixer\",\"volume\",{0}")]
        Volume,

        //// Decrease the volume of all the players
        [Command("\"mixer\",\"volume\",\"-2\"")]
        Volume_Down,

        //// Increase the volume of all the players
        [Command("\"mixer\",\"volume\",\"+2\"")]
        Volume_Up,

    }
}

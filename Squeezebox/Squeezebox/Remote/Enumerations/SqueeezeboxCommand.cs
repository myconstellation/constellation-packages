namespace Squeezebox.Remote.Enumerations
{

    using Squeezebox.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum SqueezeboxCommand
    {

        [Command("\"mixer\",\"muting\",\"0\"")]
        Mute_Off,

        [Command("\"mixer\",\"muting\",\"1\"")]
        Mute_On,

        [Command("\"mixer\",\"muting\"")]
        Mute_Toggle,

        [Command("\"button\",\"jump_fwd\"")]
        Next,

        [Command("\"pause\",\"1\"")]
        Pause,

        [Command("\"pause\",\"0\"")]
        Play,

        [Command("\"playlist\",\"loadtracks\",\"album.titlesearch={0}\"")]
        Play_Album,

        [Command("\"playlist\",\"loadtracks\",\"contributor.namesearch={0}\"")]
        Play_Artist,

        [Command("\"playlist\",\"play\",\"{0}\"")]
        Play_Playlist,

        [Command("\"playlist\",\"loadtracks\",\"track.titlesearch={0}\"")]
        Play_Title,

        [Command("\"pause\"")]
        Play_Toggle,

        [Command("\"playlist\",\"clear\"")]
        Playlist_Clear,

        [Command("\"power\",\"0\"")]
        Power_Off,

        [Command("\"power\",\"1\"")]
        Power_On,

        [Command("\"power\"")]
        Power_Toggle,

        [Command("\"button\",\"jump_rew\"")]
        Previous,

        [Command("\"randomplay\",\"album\"")]
        Random_Album,

        [Command("\"randomplay\",\"contributor\"")]
        Random_Artist,

        [Command("\"randomplay\",\"track\"")]
        Random_Title,

        [Command("\"randomplay\",\"year\"")]
        Random_Year,

        [Command("\"playlist\",\"repeat\",\"0\"")]
        Repeat_Off,

        [Command("\"playlist\",\"repeat\",\"2\"")]
        Repeat_Playlist,

        [Command("\"playlist\",\"repeat\",\"1\"")]
        Repeat_Title,

        [Command("\"playlist\",\"repeat\"")]
        Repeat_Toggle,

        [Command("\"playlist\",\"repeat\",\"2\"")]
        Shuffle_Album,

        [Command("\"playlist\",\"repeat\",\"0\"")]
        Shuffle_Off,

        [Command("\"playlist\",\"repeat\",\"1\"")]
        Shuffle_Title,

        [Command("\"playlist\",\"shuffle\"")]
        Shuffle_Toggle,

        [Command("\"stop\"")]
        Stop,

        [Command("\"sync\",\"{0}\"")]
        Sync,

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

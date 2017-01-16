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
        //// Disable muting
        [Command("\"mixer\",\"muting\",\"0\"")]
        Mute_Off,

        //// Enable muting
        [Command("\"mixer\",\"muting\",\"1\"")]
        Mute_On,

        //// Toggle muting
        [Command("\"mixer\",\"muting\"")]
        Mute_Toggle,

        //// Launch the next song
        [Command("\"button\",\"jump_fwd\"")]
        Next,

        //// Pause music
        [Command("\"pause\",\"1\"")]
        Pause,

        //// Play music
        [Command("\"pause\",\"0\"")]
        Play,

        //// Launch an album (require album name)
        [Command("\"playlist\",\"loadtracks\",\"album.titlesearch={0}\"")]
        Play_Album,

        //// Launch an artist (require artist name)
        [Command("\"playlist\",\"loadtracks\",\"contributor.namesearch={0}\"")]
        Play_Artist,

        //// Launch a playlist (require playlist name)
        [Command("\"playlist\",\"play\",\"{0}\"")]
        Play_Playlist,

        //// Launch a title (require title)
        [Command("\"playlist\",\"loadtracks\",\"track.titlesearch={0}\"")]
        Play_Title,

        //// Toggle pause state
        [Command("\"pause\"")]
        Play_Toggle,

        //// Erase current playlist
        [Command("\"playlist\",\"clear\"")]
        Playlist_Clear,

        //// Power Off
        [Command("\"power\",\"0\"")]
        Power_Off,

        //// Power On
        [Command("\"power\",\"1\"")]
        Power_On,

        //// Toggle power state
        [Command("\"power\"")]
        Power_Toggle,

        //// Launch the previous song
        [Command("\"button\",\"jump_rew\"")]
        Previous,

        //// Launch a random play by album
        [Command("\"randomplay\",\"album\"")]
        Random_Album,

        //// Launch a random play by artist
        [Command("\"randomplay\",\"contributor\"")]
        Random_Artist,

        //// Launch a random play by title
        [Command("\"randomplay\",\"track\"")]
        Random_Title,

        //// Launch a random play by year
        [Command("\"randomplay\",\"year\"")]
        Random_Year,

        //// Disable repeat
        [Command("\"playlist\",\"repeat\",\"0\"")]
        Repeat_Off,

        //// Repeat playlist
        [Command("\"playlist\",\"repeat\",\"2\"")]
        Repeat_Playlist,

        //// Repeat title
        [Command("\"playlist\",\"repeat\",\"1\"")]
        Repeat_Title,

        //// Toggle repeat state
        [Command("\"playlist\",\"repeat\"")]
        Repeat_Toggle,

        //// Shuffle by album
        [Command("\"playlist\",\"repeat\",\"2\"")]
        Shuffle_Album,

        //// Shuffle Off
        [Command("\"playlist\",\"repeat\",\"0\"")]
        Shuffle_Off,

        //// Shuffle by title
        [Command("\"playlist\",\"repeat\",\"1\"")]
        Shuffle_Title,

        //// Toggle shuffle state
        [Command("\"playlist\",\"shuffle\"")]
        Shuffle_Toggle,

        //// Stop music
        [Command("\"stop\"")]
        Stop,

        //// Sync another player to this player (require target player)
        [Command("\"sync\",\"{0}\"")]
        Sync,

        //// Disable syncing
        [Command("\"sync\",\"-\"")]
        Sync_Off,

        //// Sync this player to another player (require target player)
        [Command("\"sync\",\"{0}\"")]
        Sync_To,

        //// Set volume (require volume level)
        [Command("\"mixer\",\"volume\",{0}")]
        Volume,

        //// Decrease volume
        [Command("\"mixer\",\"volume\",\"-2\"")]
        Volume_Down,

        //// Increase volume
        [Command("\"mixer\",\"volume\",\"+2\"")]
        Volume_Up,


    }
}

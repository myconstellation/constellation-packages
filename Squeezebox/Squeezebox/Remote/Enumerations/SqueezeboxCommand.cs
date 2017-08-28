namespace Squeezebox.Remote.Enumerations
{

    using Squeezebox.Remote.Attributes;

    public enum SqueezeboxCommand
    {

        //// Add an album by id at the end of current playlist (require album id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:add\",\"album_id:{1}\"]]}}")]
        Add_Album_Id,

        //// Add an artist by id at the end of current playlist (require artist id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:add\",\"artist_id:{1}\"]]}}")]
        Add_Artist_Id,

        //// Add a title by id at the end of current playlist (require title id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:add\",\"track_id:{1}\"]]}}")]
        Add_Title_Id,

        //// Delete an album by id from the playlist (require album id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:delete\",\"album_id:{1}\"]]}}")]
        Delete_Album_Id,

        //// Delete an artist by id from the playlist (require artist id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:delete\",\"artist_id:{1}\"]]}}")]
        Delete_Artist_Id,

        //// Delete a title by id from the playlist (require title id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:delete\",\"track_id:{1}\"]]}}")]
        Delete_Title_Id,

        //// Connect player on "LMS bis" to this LMS (require "LMS bis" Squeebox's name and IP adress)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"\",[\"disconnect\",\"{0}\",\"{1}\"]]}}")]
        Connect,

        //// Connect player to "LMS bis" (require "LMS bis" IP adress)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"connect\",\"{1}\"]]}}")]
        Connect_To,

        //// Disable muting
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"mixer\",\"muting\",\"0\"]]}}")]
        Mute_Off,

        //// Enable muting
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"mixer\",\"muting\",\"1\"]]}}")]
        Mute_On,

        //// Toggle muting
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"mixer\",\"muting\"]]}}")]
        Mute_Toggle,

        //// Launch the next song
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"button\",\"jump_fwd\"]]}}")]
        Next,

        //// Pause music
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"pause\",\"1\"]]}}")]
        Pause,

        //// Play music
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"pause\",\"0\"]]}}")]
        Play,
        
        //// Launch an album (require album name)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"loadtracks\",\"album.titlesearch={1}\"]]}}")]
        Play_Album,

        //// Launch an album by id (require album id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:load\",\"album_id:{1}\"]]}}")]
        Play_Album_Id,

        //// Launch an artist (require artist name)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"loadtracks\",\"contributor.namesearch={1}\"]]}}")]
        Play_Artist,

        //// Launch an artist by id (require artist id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:load\",\"artist_id:{1}\"]]}}")]
        Play_Artist_Id,

        //// Launch a title in the current playlist by his index (require title index)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"index\",\"{1}\"]]}}")]
        Play_Index,

        //// Launch a playlist (require playlist name)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"play\",\"{1}\"]]}}")]
        Play_Playlist,

        //// Launch a playist by id (require playlist id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:load\",\"playlist_id:{1}\"]]}}")]
        Play_Playlist_Id,

        //// Launch a title (require title)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"loadtracks\",\"track.titlesearch={1}\"]]}}")]
        Play_Title,

        //// Launch a title by id (require title id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:load\",\"track_id:{1}\"]]}}")]
        Play_Title_Id,

        //// Add music by id on next position (require title id)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlistcontrol\",\"cmd:insert\",\"track_id:{1}\"]]}}")]
        Play_Title_Id_Next,

        //// Toggle pause state
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"pause\"]]}}")]
        Play_Toggle,

        //// Erase current playlist
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"clear\"]]}}")]
        Playlist_Clear,

        //// Move song from playlist by index
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"move\",\"{1}\",\"{2}\"]]}}")]
        Playlist_Move,

        //// Power Off
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"power\",\"0\"]]}}")]
        Power_Off,

        //// Power On
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"power\",\"1\"]]}}")]
        Power_On,

        //// Toggle power state
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"power\"]]}}")]
        Power_Toggle,

        //// Launch the previous song
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"button\",\"jump_rew\"]]}}")]
        Previous,

        //// Launch a random play by album
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"randomplay\",\"album\"]]}}")]
        Random_Album,

        //// Launch a random play by artist
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"randomplay\",\"contributor\"]]}}")]
        Random_Artist,

        //// Launch a random play by title
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"randomplay\",\"track\"]]}}")]
        Random_Title,

        //// Launch a random play by year
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"randomplay\",\"year\"]]}}")]
        Random_Year,

        //// Disable repeat
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"repeat\",\"0\"]]}}")]
        Repeat_Off,

        //// Repeat playlist
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"repeat\",\"2\"]]}}")]
        Repeat_Playlist,

        //// Repeat title
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"repeat\",\"1\"]]}}")]
        Repeat_Title,

        //// Toggle repeat state
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"repeat\"]]}}")]
        Repeat_Toggle,

        //// Shuffle by album
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"shuffle\",\"2\"]]}}")]
        Shuffle_Album,

        //// Shuffle Off
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"shuffle\",\"0\"]]}}")]
        Shuffle_Off,

        //// Shuffle by title
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"shuffle\",\"1\"]]}}")]
        Shuffle_Title,

        //// Toggle shuffle state
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"playlist\",\"shuffle\"]]}}")]
        Shuffle_Toggle,

        //// Stop music
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"stop\"]]}}")]
        Stop,

        //// Sync another player to this player (require target player)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"sync\",\"{1}\"]]}}")]
        Sync,

        //// Disable syncing
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"sync\",\"-\"]]}}")]
        Sync_Off,

        //// Sync this player to another player (require target player)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{1}\",[\"sync\",\"{0}\"]]}}")]
        Sync_To,

        //// Set volume (require volume level)
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"mixer\",\"volume\",{1}]]}}")]
        Volume,

        //// Decrease volume
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"mixer\",\"volume\",\"-2\"]]}}")]
        Volume_Down,

        //// Increase volume
        [Command("{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[\"mixer\",\"volume\",\"+2\"]]}}")]
        Volume_Up,


    }
}

namespace FreeboxTV.Remote.Enumerations
{

    using FreeboxTV.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum FreeCommandKey
    {
        [Command("red")]
        Red,
        [Command("green")]
        Green,
        [Command("blue")]
        Blue,
        [Command("yellow")]
        Yellow,

        [Command("power")]
        Power,
        [Command("list")]
        List,
        [Command("tv")]
        TV,

        [Command("0")]
        Button0,
        [Command("1")]
        Button1,
        [Command("2")]
        Button2,
        [Command("3")]
        Button3,
        [Command("4")]
        Button4,
        [Command("5")]
        Button5,
        [Command("6")]
        Button6,
        [Command("7")]
        Button7,
        [Command("8")]
        Button8,
        [Command("9")]
        Button9,

        [Command("back")]
        Back,
        [Command("swap")]
        Swap,

        [Command("info")]
        Info,
        [Command("epg")]
        EPG,
        [Command("mail")]
        Mail,
        [Command("media")]
        Media,
        [Command("help")]
        Help,
        [Command("options")]
        Options,
        [Command("pip")]
        PIP,

        [Command("vol_inc")]
        VolumeUp,
        [Command("vol_dec")]
        VolumeDown,

        [Command("ok")]
        OK,
        [Command("up")]
        Up,
        [Command("right")]
        Right,
        [Command("down")]
        Down,
        [Command("left")]
        Left,

        [Command("prgm_inc")]
        ProgramUp,
        [Command("prgm_dec")]
        ProgramDown,

        [Command("mute")]
        Mute,
        [Command("home")]
        Home,
        [Command("rec")]
        Record,

        [Command("bwd")]
        BackPlay,
        [Command("prev")]
        PreviousPlay,
        [Command("play")]
        Play,
        [Command("fwd")]
        ForwardPlay,
        [Command("next")]
        NextPlay
    }
}

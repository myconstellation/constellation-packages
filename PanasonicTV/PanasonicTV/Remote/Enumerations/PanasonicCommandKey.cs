namespace PanasonicTV.Remote.Enumerations
{

    using PanasonicTV.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum PanasonicCommandKey
    {
        //// RED button
        [Command("NRC_RED-ONOFF")]
        Red,

        //// GREEN button
        [Command("NRC_GREEN-ONOFF")]
        Green,

        //// BLUE button
        [Command("NRC_BLUE-ONOFF")]
        Blue,

        //// YELLOW button
        [Command("NRC_YELLOW-ONOFF")]
        Yellow,

        //// Toggle power
        [Command("NRC_POWER-ONOFF")]
        Power,

        //// Power ON the TV
        [Command("NRC_POWER-ON")]
        PowerOn,

        //// 0 button
        [Command("NRC_D0-ONOFF")]
        Button0,

        //// 1 button
        [Command("NRC_D1-ONOFF")]
        Button1,

        //// 2 button
        [Command("NRC_D2-ONOFF")]
        Button2,

        //// 3 button
        [Command("NRC_D3-ONOFF")]
        Button3,

        //// 4 button
        [Command("NRC_D4-ONOFF")]
        Button4,

        //// 5 button
        [Command("NRC_D5-ONOFF")]
        Button5,

        //// 6 button
        [Command("NRC_D6-ONOFF")]
        Button6,

        //// 7 button
        [Command("NRC_D7-ONOFF")]
        Button7,

        //// 8 button
        [Command("NRC_D8-ONOFF")]
        Button8,

        //// 9 button
        [Command("NRC_D9-ONOFF")]
        Button9,

        //// Return button
        [Command("NRC_RETURN-ONOFF")]
        Back,

        //// Increase volume
        [Command("NRC_VOLUP-ONOFF")]
        VolumeUp,

        //// Decrease volume
        [Command("NRC_VOLDOWN-ONOFF")]
        VolumeDown,

        //// Enter button
        [Command("NRC_ENTER-ONOFF")]
        OK,

        //// Up button
        [Command("NRC_UP-ONOFF")]
        Up,

        //// Right button
        [Command("NRC_RIGHT-ONOFF")]
        Right,

        //// Down button
        [Command("NRC_DOWN-ONOFF")]
        Down,

        //// Left button
        [Command("NRC_LEFT-ONOFF")]
        Left,

        //// Channel up
        [Command("NRC_CH_UP-ONOFF")]
        ProgramUp,

        //// Channel down
        [Command("NRC_CH_DOWN-ONOFF")]
        ProgramDown,

        //// Toggle mute
        [Command("NRC_MUTE-ONOFF")]
        Mute,

        //// TV button
        [Command("NRC_TV-ONOFF")]
        TV,

        //// AUX button
        [Command("NRC_CHG_INPUT-ONOFF")]
        Input,

        //// VieraTools button
        [Command("NRC_VTOOLS-ONOFF")]
        VieraTools,

        //// Cancel button
        [Command("NRC_CANCEL-ONOFF")]
        Cancel,

        //// Options button
        [Command("NRC_SUBMENU-ONOFF")]
        Option,

        //// 3D button
        [Command("NRC_3D-ONOFF")]
        ThreeDimensional,

        //// SD card button
        [Command("NRC_SD_CARD-ONOFF")]
        SDCard,

        //// Change display mode
        [Command("NRC_DISP_MODE-ONOFF")]
        DisplayMode,

        //// Menu button
        [Command("NRC_MENU-ONOFF")]
        Menu,

        //// Display VIERA Connect
        [Command("NRC_INTERNET-ONOFF")]
        VieraConnect,

        //// Display VIERA Link
        [Command("NRC_VIERA_LINK-ONOFF")]
        VieraLink,

        //// Toggle EPG
        [Command("NRC_EPG-ONOFF")]
        EPG,

        //// Toggle Text
        [Command("NRC_TEXT-ONOFF")]
        Text,

        //// Toggle subtitle
        [Command("NRC_STTL-ONOFF")]
        Subtitle,

        //// Info button
        [Command("NRC_INFO-ONOFF")]
        Info,

        //// Index button
        [Command("NRC_INDEX-ONOFF")]
        Index,

        //// Hold button
        [Command("NRC_HOLD-ONOFF")]
        Hold,

        //// Display last view
        [Command("NRC_R_TUNE-ONOFF")]
        LastView,

        //// Rewind button
        [Command("NRC_REW-ONOFF")]
        Rewind,

        //// Play button
        [Command("NRC_PLAY-ONOFF")]
        Play,

        //// Forward button
        [Command("NRC_FF-ONOFF")]
        FastForward,

        //// Skip Previous
        [Command("NRC_SKIP_PREV-ONOFF")]
        SkipPrevious,

        //// Pause button
        [Command("NRC_PAUSE-ONOFF")]
        Pause,

        //// Skip Next
        [Command("NRC_SKIP_NEXT-ONOFF")]
        SkipNext,

        //// Stop button
        [Command("NRC_STOP-ONOFF")]
        Stop,

        //// Record button
        [Command("NRC_REC-ONOFF")]
        Record,

        //// Toggle HDMI1
        [Command("NRC_HDMI1-ONOFF")]
        HDMI_1,

        //// Toggle HDMI2
        [Command("NRC_HDMI2-ONOFF")]
        HDMI_2,

        //// Toggle HDMI3
        [Command("NRC_HDMI3-ONOFF")]
        HDMI_3,

        //// Toggle HDMI4
        [Command("NRC_HDMI4-ONOFF")]
        HDMI_4,
    }
}

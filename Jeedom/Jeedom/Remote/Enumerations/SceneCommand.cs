namespace Jeedom.Remote.Enumerations
{

    using Jeedom.Remote.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum SceneCommand
    {
        //// Start the scene
        [Command("start")]
        Start,

        //// Stop the scene
        [Command("stop")]
        Stop,

        //// Activate the scene
        [Command("activate")]
        Activer,

        //// Desactivate the scene
        [Command("deactivate")]
        Desactiver,

    }
}

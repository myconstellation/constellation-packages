namespace Jeedom.Remote.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface 
    /// Show a basic remote controller struct
    /// </summary>
    public interface ISceneController<Int, T>
    {
        /// <summary>
        /// Send a key to the api
        /// </summary>
        /// <param name="id"> Command explanation </param>
        /// <param name="command"> Command explanation </param>
        void SendKey(int id, T command);

    }
}

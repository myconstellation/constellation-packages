namespace FreeboxTV.Remote.Interfaces
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
    public interface IRemoteController<T, P>
    {
        /// <summary>
        /// Send a key to the api
        /// </summary>
        /// <param name="command"> Command explanation </param>
        /// <param name="commandType"> Type of the command </param>
        void SendKey(T command, P commandType);
    }
}

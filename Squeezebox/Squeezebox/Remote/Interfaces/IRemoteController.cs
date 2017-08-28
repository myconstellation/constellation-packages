namespace Squeezebox.Remote.Interfaces
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
    public interface IRemoteController<T1, T2, T3, T4>
    {
        /// <summary>
        /// Send a key to the api
        /// </summary>
        /// <param name="command"> Command explanation </param>
        /// <param name="value"> Command explanation </param>
        /// <param name="value2"> Command explanation </param>
        /// <param name="squeezebox"> Command explanation </param>
        void SendKey(T1 command, string value, string value2, string squeezebox);

    }
}

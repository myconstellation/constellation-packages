namespace FreeboxTV.Remote
{
    using FreeboxTV.Remote.Attributes;
    using FreeboxTV.Remote.Enumerations;
    using FreeboxTV.Remote.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpFreeRemoteController : IRemoteController<FreeCommandKey, FreeCommandType>, IDisposable
    {
        public string RemoteKey { get; set; }
        public WebClient HttpClient { get; set; }

        /// <summary>
        /// Configuration
        /// URL about Free HTTP API template
        /// </summary>
        const string HttpApiUrlTemplate = "http://hd1.freebox.fr/pub/remote_control?code={0}&key={1}&long={2}";

        public HttpFreeRemoteController(string remoteKey)
        {
            if (string.IsNullOrEmpty(remoteKey))
                throw new InvalidOperationException("Remote Key must be provided");

            this.RemoteKey = remoteKey;
            this.HttpClient = new WebClient();
        }


        /// <summary>
        ///  Send a command to the freebox api
        /// </summary>
        /// <param name="command"> Command enum </param>
        /// <param name="commandType"> Command Type </param>
        public void SendKey(FreeCommandKey command, FreeCommandType commandType)
        {
            //// Volontary use not async action
            string result = this.HttpClient.DownloadString(this.GenerateCommandFromUrl(command, commandType));

            //// TODO : Parse result
        }

        /// <summary>
        ///  Generate a valid api http url with associated parameters
        /// </summary>
        /// <param name="command"> Command enum </param>
        /// <param name="commandType"> Command type enum </param>
        /// <returns></returns>
        private string GenerateCommandFromUrl(FreeCommandKey command, FreeCommandType commandType)
        {
            return string.Format(HttpApiUrlTemplate, this.RemoteKey, command.To<CommandAttribute>(), commandType.To<CommandAttribute>());
        }

        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}

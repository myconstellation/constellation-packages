namespace Squeezebox.Remote
{
    using Squeezebox.Remote.Attributes;
    using Squeezebox.Remote.Enumerations;
    using Squeezebox.Remote.Interfaces;
    using Constellation;
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.IO;

    public class HttpServerController : IServerController<ServerCommand, string>, IDisposable
    {
        public string RemoteKey { get; set; }
        public WebClient HttpClient { get; set; }

        /// <summary>
        /// Configuration
        /// URL about Free HTTP API template
        /// </summary>
        const string HttpApiUrlTemplateServer = "{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"\",[{0}]]}}";
        const string HttpApiUrlTemplateSqueezebox = "{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[{1}]]}}";

        public HttpServerController(string remoteKey)
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
        /// <param name="value"> Command enum </param>
        public void SendKey(ServerCommand command, string value)
        {
            PackageHost.WriteInfo("test");
            string url = "http://" + this.RemoteKey + "/jsonrpc.js";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            string test = command.ToString();
            PackageHost.WriteInfo("{0}", test);
            if (test != "Scan_Fast")
            {
                PackageHost.StateObjectUpdated += (s, e) =>
                {
                    foreach (dynamic hw in e.StateObject.DynamicValue)
                    {
                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        Byte[] byteArray = encoding.GetBytes(this.GenerateCommandFromUrlToSqueezebox(command, hw.name));
                        request.ContentLength = byteArray.Length;
                        request.ContentType = @"application/json";
                        PackageHost.WriteInfo("{0}", this.GenerateCommandFromUrlToSqueezebox(command, hw.name));
                        using (Stream dataStream = request.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                        }
                        long length = 0;
                        try
                        {
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                length = response.ContentLength;

                            }

                        }
                        catch (WebException ex)
                        {

                        }
                    }
                };
                PackageHost.RequestStateObjects(sentinel: "Squeezebox", package: "Info", name: "Players");
            }
            else
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(this.GenerateCommandFromUrlToServer(command));

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                long length = 0;
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        length = response.ContentLength;

                    }

                }
                catch (WebException ex)
                {

                }
            }


            //// TODO : Parse result
        }


        /// <summary>
        ///  Generate a valid api http url with associated parameters
        /// </summary>
        /// <param name="command"> Command enum </param>
        /// <returns></returns>
        private string GenerateCommandFromUrlToServer(ServerCommand command)
        {
            return string.Format(HttpApiUrlTemplateServer, command.To<CommandAttribute>());
        }

        /// <summary>
        ///  Generate a valid api http url with associated parameters
        /// </summary>
        /// <param name="command"> Command enum </param>
        /// <param name="squeezebox"> string </param>
        /// <returns></returns>
        private string GenerateCommandFromUrlToSqueezebox(ServerCommand command, dynamic squeezebox)
        {
            return string.Format(HttpApiUrlTemplateSqueezebox, squeezebox, command.To<CommandAttribute>());
        }


        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}

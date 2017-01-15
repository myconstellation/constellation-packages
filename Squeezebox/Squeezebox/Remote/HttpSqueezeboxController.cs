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

    public class HttpSqueezeboxController : IRemoteController<SqueezeboxCommand, string, string>, IDisposable
    {
        public string RemoteKey { get; set; }
        public WebClient HttpClient { get; set; }
        const string HttpApiUrlTemplateSqueezebox = "{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"{0}\",[{1}]]}}";

        public HttpSqueezeboxController(string remoteKey)
        {
            if (string.IsNullOrEmpty(remoteKey))
                throw new InvalidOperationException("Remote Key must be provided");
            this.RemoteKey = remoteKey;
            this.HttpClient = new WebClient();
        }

        public void SendKey(SqueezeboxCommand command, string value, string squeezebox)
        {
            string url = "http://" + this.RemoteKey + "/jsonrpc.js";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(this.GenerateCommandFromUrlToSqueezebox(command, squeezebox, value));
            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";
            PackageHost.WriteInfo("{0}", this.GenerateCommandFromUrlToSqueezebox(command, squeezebox, value));
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
                PackageHost.WriteError(ex);
            }
        }

        public string GenerateCommandFromUrlToSqueezebox(SqueezeboxCommand command, string squeezebox, string value)
        {
            string command_name = string.Format("{0}", command);
            string provider = squeezebox;
            string data = value;
            if (command_name == "Sync_To")
            {
                provider = value;
                data = squeezebox;
            }
            if (string.IsNullOrEmpty(value))
            {
                return string.Format(HttpApiUrlTemplateSqueezebox, provider, command.To<CommandAttribute>());

            }
            else
            {
                string result = string.Format(command.To<CommandAttribute>(), data);
                return string.Format(HttpApiUrlTemplateSqueezebox, provider, result);
            }
        }

        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}

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
    using System.Net.Cache;
    using System.Web.Script.Serialization;

    public class HttpServerController : IServerController<ServerCommand>, IDisposable
    {
        public string RemoteKey { get; set; }
        public WebClient HttpClient { get; set; }
        const string HttpApiUrlTemplateServer = "{{\"id\":1,\"method\":\"slim.request\",\"params\":[\"\",[{0}]]}}";

        public HttpServerController(string remoteKey)
        {
            if (string.IsNullOrEmpty(remoteKey))
                throw new InvalidOperationException("Remote Key must be provided");
            this.RemoteKey = remoteKey;
            this.HttpClient = new WebClient();
        }

        public void SendKey(ServerCommand command)
        {
            
            string testcommand = this.GenerateCommandFromUrlToServer(command);
            PackageHost.WriteInfo("Send {0} to Logitech Media Server", command);
            string raiponce = this.Requete(testcommand);

        }

        public string Requete(string command)
        {
            string url = "http://" + this.RemoteKey + "/jsonrpc.js";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = @"application/json";
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(command);
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                        using( var reader = new StreamReader(response.GetResponseStream()))
                        {
                            return reader.ReadToEnd();
                        }
                }
            }
            catch (WebException ex)
            {

                    using (var errorResponse = (HttpWebResponse)ex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            return reader.ReadToEnd();
                        }
                    }
            }
        }

        private string GenerateCommandFromUrlToServer(ServerCommand command)
        {
            return string.Format(HttpApiUrlTemplateServer, command.To<CommandAttribute>());
        }

        public void Dispose()
        {
            this.HttpClient.Dispose();
        }

        public class PlayersLoop
        {
            public int seq_no { get; set; }
            public string playerid { get; set; }
            public string displaytype { get; set; }
            public int connected { get; set; }
            public string ip { get; set; }
            public string model { get; set; }
            public string name { get; set; }
            public object firmware { get; set; }
            public object uuid { get; set; }
            public int isplayer { get; set; }
            public int canpoweroff { get; set; }
            public int isplaying { get; set; }
            public object playerindex { get; set; }
            public int power { get; set; }
            public string modelname { get; set; }
        }

        public class Result
        {
            public int count { get; set; }
            public List<PlayersLoop> players_loop { get; set; }
        }

        public class RootObject
        {
            public List<object> @params { get; set; }
            public string method { get; set; }
            public int id { get; set; }
            public Result result { get; set; }
        }

    }
}

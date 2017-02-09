namespace Jeedom.Remote
{
    using Jeedom.Remote.Attributes;
    using Jeedom.Remote.Enumerations;
    using Jeedom.Remote.Interfaces;
    using Constellation;
    using Constellation.Package;
    using System;
    using System.Net;
    using System.IO;

    public class HttpSceneController : ISceneController<int, SceneCommand>, IDisposable
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }
        public WebClient HttpClient { get; set; }
        const string HttpApiJeedom = "http://{0}/core/api/jeeApi.php?apikey={1}&type=scenario&id={2}&action={3}";

        public HttpSceneController(string JeedomUrl, string JeedomApiKey)
        {
            if (string.IsNullOrEmpty(JeedomUrl))
                throw new InvalidOperationException("Jeedom Url must be provided");
            if (string.IsNullOrEmpty(JeedomApiKey))
                throw new InvalidOperationException("Jeedom Api Key must be provided");
            this.Url = JeedomUrl;
            this.ApiKey = JeedomApiKey;
            this.HttpClient = new WebClient();
        }

        public void SendKey(int id, SceneCommand command)
        {
            string url = this.GenerateUrl(id, command);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(url);
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
                    PackageHost.WriteInfo("Command send to Jeedom");
                }
            }
            catch (WebException ex)
            {
                PackageHost.WriteError(ex);
            }
        }

        public string GenerateUrl(int id, SceneCommand command)
        {
            string commandhttp = command.To<CommandAttribute>();
            return string.Format(HttpApiJeedom, Url, ApiKey, id, commandhttp);

        }

        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}

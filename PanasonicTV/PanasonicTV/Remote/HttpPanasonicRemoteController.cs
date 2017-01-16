namespace PanasonicTV.Remote
{
    using Constellation.Package;
    using PanasonicTV.Remote.Attributes;
    using PanasonicTV.Remote.Enumerations;
    using PanasonicTV.Remote.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Sockets;
    using System.IO;
    using System.Diagnostics;

    public class HttpPanasonicRemoteController : IRemoteController<PanasonicCommandKey, String>, IDisposable
    {
        public WebClient HttpClient { get; set; }

        public HttpPanasonicRemoteController()
        {
            this.HttpClient = new WebClient();
        }

        /// <summary>
        ///  Send a command to the panasonic api
        /// </summary>
        /// <param name="command"> Command enum </param>
        /// <param name="target"> Target name </param>
        public void SendKey(PanasonicCommandKey command, string target)
        {
            string url = PackageHost.GetSettingValue<string>(target);
            string data = this.GenerateCommandFromUrl(command);
            WebRequest req = WebRequest.Create("http://" + url + ":55000/nrc/control_0");
            HttpWebRequest httpReq = (HttpWebRequest)req;
            httpReq.Method = "POST";
            httpReq.ContentType = "text/xml; charset=utf-8";
            httpReq.ProtocolVersion = HttpVersion.Version11;
            httpReq.Proxy = null;
            httpReq.Credentials = CredentialCache.DefaultCredentials;
            httpReq.Headers.Add("SOAPAction: \"urn:panasonic-com:service:p00NetworkControl:1#X_SendKey\"");
            httpReq.ContentLength = data.Length;
            Stream requestStream = httpReq.GetRequestStream();
            StreamWriter writer = new StreamWriter(requestStream, Encoding.ASCII);
            writer.Write(data);
            writer.Close();
            WebResponse response = httpReq.GetResponse();
            response.Close();
            PackageHost.WriteInfo("Send command to TV");

        }

        /// <summary>
        ///  Generate a valid api http url with associated parameters
        /// </summary>
        /// <param name="command"> Command enum </param>
        /// <returns></returns>
        private string GenerateCommandFromUrl(PanasonicCommandKey command)
        {
            StringBuilder request = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            string test = (command.To<CommandAttribute>()).ToString();
            request.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append("<s:Body>");
            request.Append("<u:X_SendKey xmlns:u=\"urn:panasonic-com:service:p00NetworkControl:1\">");
            request.Append("<X_KeyEvent>");
            request.Append(test);
            request.Append("</X_KeyEvent>");
            request.Append("</u:X_SendKey>");
            request.Append("</s:Body>");
            request.Append("</s:Envelope>");

            return request.ToString();
        }

        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}

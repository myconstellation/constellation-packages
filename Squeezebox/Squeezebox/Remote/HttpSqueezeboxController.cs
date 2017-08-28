namespace Squeezebox.Remote
{
    using Squeezebox.Remote.Attributes;
    using Squeezebox.Remote.Enumerations;
    using Squeezebox.Remote.Interfaces;
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.IO;
    using System.Web.Script.Serialization;
    using System.Text;

    public class HttpSqueezeboxController : IRemoteController<SqueezeboxCommand, string, string, string>, IDisposable
    {
        public WebClient HttpClient { get; set; }

        public HttpSqueezeboxController()
        {
            this.HttpClient = new WebClient();
        }

        public void SendKey(SqueezeboxCommand command, string squeezebox = "", string value = "none", string value2 = "none")
        {
            if (string.IsNullOrEmpty(squeezebox))
            {
                string players = "{\"id\":1,\"method\":\"slim.request\",\"params\":[\"\",[\"players\",\"0\",\"100\"]]}";
                var players_result = this.Requete(players);
                JavaScriptSerializer js = new JavaScriptSerializer();
                RootObject root = (RootObject)js.Deserialize(players_result, typeof(RootObject));
                foreach (var player in root.result.players_loop)
                {
                    string command_player = this.GenerateCommandFromUrlToSqueezebox(command, player.playerid, value, value2);
                    PackageHost.WriteInfo("Send {0} to {1}", command, player.name);
                    string request_result = this.Requete(command_player);
                }
            }
            else
            {
                var players = squeezebox.Split(new[]{ ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string player in players)
                {
                    string command_player = this.GenerateCommandFromUrlToSqueezebox(command, player, value, value2);
                    PackageHost.WriteInfo("Send {0} to {1}", command, player);
                    string request_result = this.Requete(command_player);
                }
            }

        }

        public string Requete(string command, string value = "", string value2 = "")
        {
            string url = string.Format("http://{0}/jsonrpc.js", PackageHost.GetSettingValue("ServerUrl"));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (PackageHost.TryGetSettingValue<string>("ServerUser", out string user))
            {
                if (PackageHost.TryGetSettingValue<string>("ServerPassword", out string password))
                {
                    string credentials = string.Format("{0}:{1}", PackageHost.GetSettingValue("ServerUser"), PackageHost.GetSettingValue("ServerPassword"));
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
                    request.PreAuthenticate = true;
                }
                else
                {
                    PackageHost.WriteError("Impossible de récupérer le setting 'Password' en string");
                }
            }
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
                    using (var reader = new StreamReader(response.GetResponseStream()))
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

        public string GenerateCommandFromUrlToSqueezebox(SqueezeboxCommand command, string squeezebox, string value, string value2)
        {
            return string.Format(command.To<CommandAttribute>(), squeezebox, value, value2);                   
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

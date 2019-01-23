/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
 *	
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *	
 *	 Unless required by applicable law or agreed to in writing,
 *	 software distributed under the License is distributed on an
 *	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *	 KIND, either express or implied. See the License for the
 *	 specific language governing permissions and limitations
 *	 under the License.
 */

namespace ZoneMinder
{
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    public static class Utils
    {
        public static string FormatUri(string rootUri, string path)
        {
            return rootUri.EndsWith("/") && path.StartsWith("/") ? rootUri + path.Substring(1) : !rootUri.EndsWith("/") && !path.StartsWith("/") ? rootUri + "/" + path : rootUri + path;
        }

        public static string DoRequest(string url, string method = "", string postdata = "", CookieContainer cookieContainer = null)
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.CookieContainer = cookieContainer;

            if (!string.IsNullOrEmpty(method))
            {
                myRequest.Method = method;
            }

            if (!string.IsNullOrEmpty(postdata))
            {
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.Method = "POST";
                byte[] data = Encoding.ASCII.GetBytes(postdata);
                myRequest.ContentLength = data.Length;
                using (var newStream = myRequest.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
            }

            using (var response = myRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream))
            {
                return responseReader.ReadToEnd();
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}

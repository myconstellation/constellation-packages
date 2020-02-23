
namespace Vorwerk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// HTTP utility
    /// </summary>
    internal static class HttpUtils
    {
        /// <summary>
        /// Gets the web response asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="throwException">if set to <c>true</c> to throw exception.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="postData">The post data body.</param>
        /// <param name="headers">The HTTP headers.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <returns></returns>
        internal static async Task<string> GetWebResponseAsync(Uri uri, bool throwException = false, string method = "", string postData = "", Dictionary<string, string> headers = null, CookieContainer cookieContainer = null, DateTime? requestDate = null)
        {
            try
            {
                // Create the request
                HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
                request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }
                // Add headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                // Add date
                if (requestDate != null)
                {
                    request.Date = requestDate.Value;
                }
                // Set the HTTP method
                if (!string.IsNullOrEmpty(method))
                {
                    request.Method = method;
                    request.ContentLength = 0;
                }
                // Set the POST body
                if (!string.IsNullOrEmpty(postData))
                {
                    var data = Encoding.ASCII.GetBytes(postData);
                    request.Method = "POST";
                    request.Accept = "application/vnd.neato.nucleo.v1";
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                // Get the response
                Debug.WriteLine(request.RequestUri);
                WebResponse response = await request.GetResponseAsync();
                // Read and return the content
                string content = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                Debug.WriteLine(content);
                return content;
            }
            catch (Exception ex)
            {
                if (ex is WebException wex)
                {
                    Debug.WriteLine(new StreamReader(wex.Response.GetResponseStream()).ReadToEnd());
                }
                if (throwException && (ex is WebException && ((WebException)ex).Status == WebExceptionStatus.Timeout) == false)
                {
                    throw ex;
                }
                return string.Empty;
            }
        }
    }
}

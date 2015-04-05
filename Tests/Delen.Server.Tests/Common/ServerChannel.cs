using System;
using System.IO;
using System.Net;
using Delen.Agent;
using Delen.Common;
using Delen.Common.Serialization;


namespace Delen.Server.Tests.Common

{
    public class ServerChannel
    {
        public static T SendRequest<T>(Uri endpoint, object payload = null) where T : class
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(endpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add(ServerConfiguration.Headers.Version,typeof (Program).Assembly.GetName().Version.ToString());
 
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonNetHelper.Serialize(payload));
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonNetHelper.Deserialize<T>(result);
                }
            }
        }
        public static T SendAgentRequest<T>(Uri endpoint, Guid token, object payload = null) where T : class
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add(ServerConfiguration.Headers.Version, typeof(Program).Assembly.GetName().Version.ToString());
            httpWebRequest.Headers.Add(ServerConfiguration.Headers.WorkerRegistrationToken, token.ToString());

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonNetHelper.Serialize(payload));
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return JsonNetHelper.Deserialize<T>(result);
                }
            }
        }
    }
}
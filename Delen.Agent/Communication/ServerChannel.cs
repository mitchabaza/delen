using System;
using System.IO;
using System.Net;
using Delen.Common;
using Delen.Common.Serialization;
using Delen.Core.Communication;
using Delen.Core.Tasks;
using log4net;

namespace Delen.Agent.Communication
{
    public class ServerChannel : IServerChannel
    {
        private readonly IUriFactory _uriFactory;
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ServerChannel));

        public ServerChannel(IUriFactory uriFactory)
        {
            _uriFactory = uriFactory;
        }

        public Response Register(RegisterWorkerRequest request)
        {
            Logger.Info("Registering worker {Worker}".FormatWith(new {Worker = request.IPAddress}));

            return SendCommand <Response>(_uriFactory.Create(EndPoint.RegisterAgent), request);
        }

        private T SendCommand<T>(Uri endpoint, object payload = null) where T : class
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(endpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add(ServerConfiguration.Headers.Version,(typeof (ServerChannel).Assembly.GetName().Version.ToString()));
            using (var writer = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                writer.Write(JsonNetHelper.Serialize(payload));
            }
            using (var reader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
            {
                var body = reader.ReadToEnd();
                try
                {
                    return JsonNetHelper.Deserialize<T>(body);
                }
                catch (JsonException ex)
                {
                    Logger.Error(ex);
                    Logger.Error(body);
                    throw;
                }
            }
        }

        public Response UnRegister(UnregisterWorkerRequest request)
        {
            Logger.Info("Unregistering worker {Worker}".FormatWith(new {Worker = request.IPAddress}));

            return SendCommand<Response>(_uriFactory.Create(EndPoint.UnregisterAgent), request);
        }

        public Response WorkComplete(TaskExecutionResult taskResult)
        {
            return SendCommand<Response>(_uriFactory.Create(EndPoint.WorkComplete) ,taskResult);
        }
 
        public Response<TaskRequest> RequestWork()
        {
            return SendCommand<Response<TaskRequest>>(_uriFactory.Create(EndPoint.RequestWork));
     
        }

        public void SendProgress(string output)
        {
            SendCommand<Response>(_uriFactory.Create(EndPoint.SendProgress), new TaskProgress{});
        }
    }
}
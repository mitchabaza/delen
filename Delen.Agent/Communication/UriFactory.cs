using System;
using StringFormat;

namespace Delen.Agent.Communication
{
    
    public class UriFactory : IUriFactory
    {
        protected readonly Settings Settings;

        protected virtual Uri BaseUri
        {
            get
            {
                string url = TokenStringFormat.Format("http://{Server}:{Port}/{Application}/",
                    new {Settings.Server, Settings.Port, Settings.Application});
                var uri = new Uri(url, UriKind.Absolute);

                return uri;
            }
        }

        private Uri RegisterWorker
        {
            get { return new Uri(BaseUri, "Registration/Add"); }
        }

        private Uri UnregisterWorker
        {
            get { return new Uri(BaseUri, "Registration/Remove"); }
        }

        private Uri AddJob
        {
            get { return new Uri(BaseUri, "Job/Queue"); }
        }

        private Uri CancelJob
        {
            get { return new Uri(BaseUri, "Job/Cancel"); }
        }

        public UriFactory(Settings settings)
        {
            Settings = settings;
        }

        public Uri RequestWork
        {
            get { return new Uri(BaseUri, "Task/Request"); }
        }

        public Uri WorkComplete
        {
            get { return new Uri(BaseUri, "Task/Complete"); }
        }

        public Uri Create(EndPoint endPoint)
        {
            switch (endPoint)
            {
                case EndPoint.RegisterAgent:
                    return RegisterWorker;
                case EndPoint.UnregisterAgent:
                    return UnregisterWorker;
                case EndPoint.QueueJob:
                    return AddJob;
                case EndPoint.CancelJob:
                    return CancelJob;
                case EndPoint.RequestWork:
                    return RequestWork;
                case EndPoint.WorkComplete:
                    return WorkComplete;
                case EndPoint.SendProgress:
                    return SendProgress;
                default:
                    throw new ArgumentOutOfRangeException("endPoint");
            }
        }

        public Uri SendProgress
        {
            get { return new Uri(BaseUri, "Task/Progess"); }
        }
    }
}
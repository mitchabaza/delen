using System;

namespace Delen.Agent.Communication
{
    public enum EndPoint

    {
        RequestWork,
        RegisterAgent,
        UnregisterAgent,
        QueueJob,
        CancelJob,
        WorkComplete,
        SendProgress
    }

    public interface IUriFactory
    {
        Uri Create(EndPoint endPoint);
       
    }
}
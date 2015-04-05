using System.Net;
using System.Net.Sockets;
using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
     public   class RegisterWorkerRequest
    {
        //private static T Create<T>() where T : WorkerRegistrationRequestBase, new()
        //{
        //    return new T
        //    {
        //        Name = Dns.GetHostName(),IPAddress= LocalIPAddress
        //    };
        //}

        public string Name { get; set; }
        public string IPAddress { get; set; }
    }
}
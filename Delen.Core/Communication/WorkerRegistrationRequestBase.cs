using System.Net;
using System.Net.Sockets;
using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
     public abstract  class WorkerRegistrationRequestBase 
    {
        public static T Create<T>() where T : WorkerRegistrationRequestBase, new()
        {
            return new T
            {
                Name = Dns.GetHostName(),IPAddress= LocalIPAddress
            };
        }

         public static string LocalIPAddress
         {
             get
             {
                 string localIP = "";
                 IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                 foreach (IPAddress ip in host.AddressList)
                 {
                     if (ip.AddressFamily == AddressFamily.InterNetwork)
                     {
                         localIP = ip.ToString();
                         break;
                     }
                 }
                 return localIP;
             }
         }
        public string Name { get; set; }
        public string IPAddress { get; set; }
    }
}
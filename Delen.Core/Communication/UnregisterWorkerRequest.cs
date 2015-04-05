using System;
using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
    

    public class UnregisterWorkerRequest 
    {
        public Guid Token { get; set; }

        public UnregisterWorkerRequest(Guid token)
        {
            Token = token;
        }

        public UnregisterWorkerRequest()
        {
             
        }

        public override string ToString()
        {
            return string.Format("Token: {0}", Token);
        }
    }
}
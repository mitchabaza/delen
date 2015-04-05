using System;

namespace Delen.Core.Tasks
{
    public class WorkerRequest
    {
        public WorkerRequest(Guid? token)
        {
            if (!token.HasValue)
            {
                throw new InvalidOperationException("Request is missing 'WorkerRegistrationToken' header");
            }
            Token = token.Value;
            
        }

        public Guid Token { get; private set; }
    }
}
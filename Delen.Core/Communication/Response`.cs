using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
         

    public class Response<T> : Response 
    {
        public Response(bool succeeded, string message, T payloadIdentifier)
            : base(succeeded, message)
        {
            Payload = payloadIdentifier;
        }

        public T Payload { get; set; }
    }
}
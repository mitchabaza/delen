using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
    

    public class CreateEntityResponse<T> : Response 
    {
        public CreateEntityResponse(bool succeeded, string message, T entityIdentifier)
            : base(succeeded, message)
        {
            EntityIdentifier = entityIdentifier;
        }

        public T EntityIdentifier { get; set; }

    }
}
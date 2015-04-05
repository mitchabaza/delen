using System.Linq.Expressions;
using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
        

    public class Response
    {
        public static Response Successful()
        {
            return new Response(true,"");
        }
           
        private Response(){}
        public Response(bool succeeded, string message )
        {
            Succeeded = succeeded;
            Message = message;
             
        }
        public Response(bool succeeded):this(true, string.Empty)
        {

        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public static Response Failure(string message="")
        {
            return new Response(false, message);
        }
    }
   
}
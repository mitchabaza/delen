using Delen.Common.Serialization;

namespace Delen.Core.Communication
{
        

    public class Response
    {
        public static Response Successful()
        {
            return new Response(true,"");
        }
           
        public Response(bool succeeded, string message )
        {
            Succeeded = succeeded;
            Message = message;
             
        }
       
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public static Response Failure(string message="")
        {
            return new Response(false, message);
        }
    }
   
}
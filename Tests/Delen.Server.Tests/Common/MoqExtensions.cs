using System.Reflection;
using Moq;

namespace Delen.Server.Tests.Common
{
   public static class MoqExtensions
    {
       public static void Setup<T>(this Mock<T> mock) where T : class
       {
           var type = typeof (T);
          var methods= type.GetMethods(BindingFlags.Public);

           foreach (var methodInfo in methods)
           {
               
           }
       }
    }
}

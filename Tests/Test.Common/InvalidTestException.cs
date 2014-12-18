using System;

namespace Delen.Test.Common
{
    public class InvalidTestException : Exception
    {
        public InvalidTestException(string message)
            : base(message)
        {
             
        }
    }
}
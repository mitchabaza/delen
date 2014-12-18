using log4net;

namespace Delen.Agent
{
   

    public interface ILogger : ILog
    {
        void WriteToConsole(string output);
    }
}
using System;

namespace Delen.Agent.Abstractions
{
    public interface IProcess
    {
        void Start();
        event EventHandler<string> Exited;
        event EventHandler<string> OutputWritten;

    }

}

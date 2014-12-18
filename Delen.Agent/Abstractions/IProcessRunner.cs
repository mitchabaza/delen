using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Delen.Agent.Tasks;
using Delen.Core.Tasks;

namespace Delen.Agent.Abstractions
{
    public interface IProcessRunner 
    {

        void Start(ProcessStartInfo startInfo, EventHandler<string> onExit, EventHandler<string> onOutput );
        IList<FileInfo> CollectArtifacts(Task task, string root);
    }
}
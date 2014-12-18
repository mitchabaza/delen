using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Delen.Agent.Tasks;
using Delen.Core.Tasks;

namespace Delen.Agent.Abstractions
{

    
    public class ProcessRunner : IProcessRunner
    {
        
        public void Start(ProcessStartInfo startInfo, EventHandler<string> onExit, EventHandler<string> onOutput )
        {
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.UseShellExecute = false;
            var process = CreateProcess(startInfo);
            process.Exited += onExit;
            process.OutputWritten += onOutput;
            process.Start();
        }

       

        //hook for testing
        protected virtual IProcess CreateProcess(ProcessStartInfo info)
        {
            var process = new Process {StartInfo = info, EnableRaisingEvents = true};
            return new ProcessWrapper(process);
        }

        public IList<FileInfo> CollectArtifacts(Task task, string root)
        {
            var artifactCollectionStrategy = new ArtifactCollector(new[] { root },
               new[] { task.ArtifactSearchFilter });
            return artifactCollectionStrategy.CollectArtifacts();
        }
    }
}
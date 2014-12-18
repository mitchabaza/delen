using Delen.Agent.Abstractions;
using Delen.Agent.Communication;

namespace Delen.Agent.Tasks
{
    public class TaskExecutionOptions
    {
        public TaskExecutionOptions(IServerChannel serverChannel, 
            IFileSystem fileSystem, 
            IProcessRunner processRunner, 
            string workingDirectory)
        {
            ServerChannel = serverChannel;
            FileSystem = fileSystem;
            ProcessRunner = processRunner;
            WorkingDirectory = workingDirectory;
        }

        public IServerChannel ServerChannel { get; private set; }
        public IFileSystem FileSystem { get; private set; }
        public IProcessRunner ProcessRunner { get; private set; }
        public string WorkingDirectory { get; private set; }

    }
}
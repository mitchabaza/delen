using Delen.Core.Tasks;

namespace Delen.Agent.Tasks
{
    public interface ITaskExecutor
    {
        void Execute(Task task);
    }
}
namespace Delen.Agent.Tasks
{
    public interface ITaskListener
    {
        void Stop();
        void Start();
    }
}
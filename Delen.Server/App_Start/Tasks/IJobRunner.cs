namespace Delen.Server.Tasks
{
    public interface IJobRunner
    {
        void  RunPendingJobs();
    }
}
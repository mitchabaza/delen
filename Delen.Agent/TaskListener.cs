using System.Threading;
using Delen.Agent.Communication;
using Delen.Agent.Tasks;
using Delen.Common;
using log4net;

namespace Delen.Agent
{
    public class TaskListener : ITaskListener
    {
        private readonly int _pollingInterval;
        private readonly IServerChannel _serverChannel;
        private readonly ITaskExecutor _executor;
        private volatile bool _shouldStop;
        private static readonly ILog Log = LogManager.GetLogger(typeof (TaskListener));

        public TaskListener(IServerChannel serverChannel, ITaskExecutor executor, int pollingInterval = 10000)
        {
            _pollingInterval = pollingInterval;
            _serverChannel = serverChannel;
            _executor = executor;
        }

        public void Stop()
        {
            _shouldStop = true;
        }

        public void Start()
        {
            while (!_shouldStop)
            {
                PollForWork();
            }
        }

        private void PollForWork()
        {
            Log.Info("Requesting work");

            var response = _serverChannel.RequestWork();

            if (!response.Payload.NoWorkAvailable)
            {
                Log.Info("Work Available {Task}".FormatWith(new {Task=response.Payload.Task.ToString()}));
                _executor.Execute(response.Payload.Task);
            }
            else
            {
                Log.Info("No work available");
                Log.Info("Sleeping for {_pollingInterval}".FormatWith(new { _pollingInterval }));
                Thread.Sleep(_pollingInterval);

            }

        }
    }
}
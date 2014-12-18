using System;
using System.Threading;
using log4net;
using Microsoft.WindowsAzure.Storage;
using Raven.Client.Extensions;
using Validation;

namespace Delen.Server.Tasks
{
    public class BackgroundTaskScheduler : IStartable
    {
        private readonly IJobRunner _jobRunner;
        private readonly TimeSpan _triggerTimespan;
        private static   Timer _timer;
        private readonly BackgroundTaskRunner _backgroundTaskRunner = new BackgroundTaskRunner();

        private static readonly ILog Log = LogManager.GetLogger(typeof (BackgroundTaskScheduler));
    
        public BackgroundTaskScheduler(IJobRunner jobRunner, TimeSpan triggerTimeSpan):this(jobRunner,Convert.ToInt32((triggerTimeSpan.Ticks * TimeSpan.TicksPerMillisecond)))
        {

         
        }

        public BackgroundTaskScheduler(IJobRunner jobRunner, int triggerMilliseconds)
        {
            Requires.Range(triggerMilliseconds > 0, "triggerMilliseconds");
            Log.Debug("Ctor called");
            _jobRunner = jobRunner;
            _triggerTimespan = TimeSpan.FromMilliseconds(triggerMilliseconds);
        }
        
        private  void OnTimerElapsed(object sender)
        {
            _backgroundTaskRunner.DoWork(() =>   Log.Debug("OnTimerElapsed called"));
        }

        public void Start()
        {
            _timer = new Timer(OnTimerElapsed);
            _timer.Change(TimeSpan.Zero, _triggerTimespan);
    
        }
    }
}

using System;
using System.Timers;
using Delen.Agent.Abstractions;

namespace Delen.Agent.Tests.Unit
{
    public class FakeProcess : IProcess
    {
         private readonly Timer _timer;

        public FakeProcess(int millisecondsBeforeExit)
        {
             if (millisecondsBeforeExit == 0)
            {
                millisecondsBeforeExit = 1;
            }
            _timer = new Timer {Interval = millisecondsBeforeExit};
            _timer.Elapsed += _timer_Elapsed;
        }


        private void StartTimer()
        {
            if (_timer.Enabled)
            {
                throw new InvalidOperationException("Cannot start Timer. Already started");
            }
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            OnExited();
        }

        public void Start()
        {
            StartTimer();
        }

        public event EventHandler<string> Exited;
        public event EventHandler<string> OutputWritten;

        protected  void OnExited()
        {
            EventHandler<string> handler = Exited;
            if (handler != null) handler(this, "");
        }
    }
}
using System;
using System.Diagnostics;
using Delen.Agent.Abstractions;

namespace Delen.Agent.Tasks
{
    public class ProcessWrapper : IProcess
    {
        private readonly Process _process;

        private string _output;


        public ProcessWrapper(Process process)
        {
            _process = process;
         }

        public void Start()
        {
            _process.Exited += Process_Exited;
            _process.Start();
            _process.ErrorDataReceived += process_OutputDataReceived;
            _process.BeginErrorReadLine();
            _process.OutputDataReceived += process_OutputDataReceived;
            _process.BeginOutputReadLine();
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnOutput(e.Data);
            _output += e.Data;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            OnExited();
        }

        protected virtual void OnExited()
        {
            EventHandler<string> handler = Exited;
            if (handler != null) handler(this, _output);
        }


        public event EventHandler<string> Exited;
        public event EventHandler<string> OutputWritten;

        protected virtual void OnOutput(string e)
        {
            EventHandler<string> handler = OutputWritten;
            if (handler != null) handler(this, e);
        }
    }
}
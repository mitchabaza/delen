using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Delen.AgentWrapper
{
    public partial class Agent : ServiceBase
    {
        public Agent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Delen.Agent.Program.Start(false);
        }

        protected override void OnStop()
        {
            Delen.Agent.Program.Stop();
        }
    }
}

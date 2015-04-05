using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Test.Common;
using Delen.Test.Common.RavenDB;

namespace Delen.Agent.Tests.Functional
{
    public abstract class AgentFixtureBase : DocumentStoreFixtureBase
    {
        protected Process Process;


       

        protected string AgentName { get; private set; }

        public override void SetupTest()
        {
            base.SetupTest();
            var processes = Process.GetProcessesByName("delen.agent.exe");
            foreach (var process in processes)
            {
                KillSilently(process);
            }
        }

        public override void TearDownTest()
        {
            KillSilently(Process);
            base.TearDownTest();
        }

        private void KillSilently(Process process)
        {
            if (process != null)

                try
                {
                    process.Kill();
                }
                catch
                {
                    //Console.WriteLine(e);
                }
        }

        protected void StartAgent()
        {
            AgentStartedAt = DateTime.Now;
            var info =
                new ProcessStartInfo(CurrentDirectory + @"..\..\..\..\..\Delen.Agent\bin\Debug\Delen.Agent.exe")
                {
                    Arguments = "TestMode",
                    ErrorDialog = false,
                    CreateNoWindow = false,
                    UseShellExecute = true
                };

            Process = Process.Start(info);
            Process.EnableRaisingEvents = true;

            Process.Exited += p_Exited;
        }

        protected DateTime AgentStartedAt { get; private set; }

        protected TimeSpan AgentHasBeenRunningFor
        {
            get
            {
                if (AgentStoppedAt.HasValue)
                {
                    return AgentStoppedAt.Value.Subtract(AgentStartedAt);
                }
                return DateTime.Now.Subtract(AgentStartedAt);
            }
        }

        protected DateTime? AgentStoppedAt { get; private set; }

        protected TimeSpan? AgentStoppedFor
        {
            get { return DateTime.Now.Subtract(AgentStoppedAt.Value); }
        }

        protected void StopAgent()
        {
            Process.CloseMainWindow();
            AgentStoppedAt = DateTime.Now;
        }

        private void p_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(@"exited");
        }
    }
}
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
            foreach (var process in Process.GetProcessesByName("delen.agent.exe"))
            {
                process.Kill();
            }
        }

        public override void TearDownTest()
        {
            if (Process != null)

                try
                {
                    Process.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            base.TearDownTest();
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
            Console.WriteLine("exited");
        }
    }
}
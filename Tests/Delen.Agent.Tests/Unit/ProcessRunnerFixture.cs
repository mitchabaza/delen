using System;
using System.Diagnostics;
using System.Threading;
using Delen.Agent.Abstractions;
using FluentAssertions;
using NUnit.Framework;

namespace Delen.Agent.Tests.Unit
{

    [TestFixture]
    public class ProcessRunnerFixture : ProcessRunner
    {
        protected override IProcess CreateProcess(ProcessStartInfo info)
        {
            return new FakeProcess(500);
        }

        [Test]
        public void Start_ShouldConnectToOnExitedEvent()
        {
            bool eventWasRaised = false;
             EventHandler<string> exitEvent = (sender, args) =>
            {
                eventWasRaised = true;
            };
             this.Start(new ProcessStartInfo(""), exitEvent, null);
            Thread.Sleep(1000);
            eventWasRaised.Should().BeTrue();
        }
    }
}

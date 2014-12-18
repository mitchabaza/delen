using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Delen.Agent.Abstractions;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Delen.Agent.Tests.Functional
{
    public class ProcessRunnerFixture : FixtureBase
    {
        private const string ConsoleExeLocation = @"..\..\..\Delen.TestConsoleApp\bin\Debug\Delen.TestConsoleApp.exe";

        [Test]
        public void StandardOutput_ShouldBeRedirectedUponExit()
        {
            var processRunner = new ProcessRunner();
             var actualString = string.Empty;
            bool waiting = true;
            EventHandler<string> enHandler = delegate(object sender, string s)
            {
                actualString += s;
                waiting = false;
            };
            Debug.Print(ExePath);
            string consoleValue = Guid.NewGuid().ToString();
            processRunner.Start(new ProcessStartInfo(ExePath, "echoAndExit " + consoleValue), enHandler, null);
// ReSharper disable LoopVariableIsNeverChangedInsideLoop
            while (waiting)
// ReSharper restore LoopVariableIsNeverChangedInsideLoop
            {
            }
            actualString.Should().Be(consoleValue);
        }

        private static string ExePath
        {
            get { return Path.Combine(CurrentDirectory ,ConsoleExeLocation); }
        }

        [Test]
        public void StandardOutput_ShouldBeRedirectedImmediately()
        {
            var processRunner = new ProcessRunner();
             var dictionaryLogEntries = new List<Tuple<TimeSpan, string>>();
            bool waitingToExit = true;
            EventHandler<string> exitHandler = delegate { waitingToExit = false; };
            EventHandler<string> outputHandler =
                (sender, output) =>
                    dictionaryLogEntries.Add(new Tuple<TimeSpan, string>(DateTime.Now.TimeOfDay, output));

            string consoleValue = Guid.NewGuid().ToString();
            processRunner.Start(new ProcessStartInfo(ExePath, "echoWithRepeat " + consoleValue + " 300"), exitHandler,
                outputHandler);
            do
            {
// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            } while (waitingToExit);


            dictionaryLogEntries.Count.Should().BeGreaterThan(10);

            for (int i = 1; i < 10; i++)

            {
                var priorTimespan = dictionaryLogEntries[i - 1].Item1;
                dictionaryLogEntries[i].Item1.Subtract(priorTimespan)
                    .Should()
                    .BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(280))
                    .And.BeLessOrEqualTo(TimeSpan.FromMilliseconds(400));
            }
        }

        [Test]
        [Ignore]
        public void StandardOutput_ShouldBeRedirected_WhenProcessSpawnsChildProcess()
        {
            var processRunner = new ProcessRunner();
             var actualString = string.Empty;
            bool waiting = true;
            //   var path2 = @"C:\Code\delen\trunk\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe";
            processRunner.Start(new ProcessStartInfo(ExePath, "echoAndExit " + ExePath), (sender, output) =>
            {
                actualString += output;
                waiting = false;
            },null);
            // ReSharper disable LoopVariableIsNeverChangedInsideLoop
            while (waiting)
                // ReSharper restore LoopVariableIsNeverChangedInsideLoop
            {
            }
            actualString.Should().Be(ExePath);
        }
    }
}
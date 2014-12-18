﻿using System.Security.Principal;
using Delen.Core.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading;
using Raven.Abstractions.Data;

namespace Delen.Agent.Tests.Functional
{
    public class RegistrationFixture : AgentFixtureBase
    {
        [Test]
        public void Agent_ShouldRegisterOnStartUp()
        {
            //arrange
            var startDate = DateTime.Now;

            WorkerRegistration workerRegistration = null;
            DocumentStore.Changes().ForDocumentsStartingWith("workerregistration").Subscribe(
                new DocumentObserver(a =>
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        workerRegistration = session.Load<WorkerRegistration>(a.Id);
                    }
                }));
            DocumentStore.Changes().WaitForAllPendingSubscriptions();
            //act
            StartAgent();

            do
            {
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop          
            } while (workerRegistration == null && AgentHasBeenRunningFor < TimeSpan.FromSeconds(15));

            StopAgent();
            //assert
            workerRegistration.Host.Should().Be(Dns.GetHostName());
            workerRegistration.CreationDate.Should().BeAfter(startDate);
        }

        [Test]
        [Ignore]
        public void Agent_ShouldUnRegisterOnShutdown()
        {
            //arrange
            var startDate = DateTime.Now;

           
            //act
            StartAgent();
            Thread.Sleep(2000);

            WorkerRegistration workerRegistration = null;
            DocumentStore.Changes().ForDocumentsStartingWith("workerregistration").Subscribe(
                new DocumentObserver(a =>
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        workerRegistration = session.Load<WorkerRegistration>(a.Id);
                        if (workerRegistration.Active )
                        {
                            workerRegistration = null;
                        }
                    }
                }));
               DocumentStore.Changes().WaitForAllPendingSubscriptions();
         
            StopAgent();
           
            do
            {
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop          
            } while (workerRegistration == null && AgentStoppedFor < TimeSpan.FromSeconds(5));

           
            //assert
            workerRegistration.Host.Should().Be(Dns.GetHostName());
            workerRegistration.CreationDate.Should().BeAfter(startDate);
            workerRegistration.Active.Should().BeFalse();
        }
    }
}
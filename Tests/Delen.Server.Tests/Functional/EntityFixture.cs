using System;
using System.Threading;
using Delen.Core.Entities;
using Delen.Test.Common;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Raven.Client.Document;

namespace Delen.Server.Tests.Functional
{
    [TestFixture]
    public class EntityFixture : DocumentStoreFixtureBase
    {
        
        [Test]
        public void LoadEntity_ShouldNotAlterActiveValue()
        {
            var registration = AutoFixture.Create<WorkerRegistration>();
            registration.Deactivate();
            using (var session= DocumentStore.OpenSession())
            {
                session.Store(registration);
                session.SaveChanges();
            }
            registration = DocumentStore.OpenSession().Load<WorkerRegistration>(registration.Id);
            registration.Active.Should().BeFalse();
        }

        [Test]
        public void LoadEntity_ShouldNotAlterCreationDateValue()
        {
            var registration = AutoFixture.Create<WorkerRegistration>();
            var timeCreation = DateTime.Now;
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(registration);
                session.SaveChanges();
            }
             
            DocumentStore.OpenSession().Load<WorkerRegistration>(registration.Id).CreationDate.Should().BeCloseTo(timeCreation);

        }
    }
}
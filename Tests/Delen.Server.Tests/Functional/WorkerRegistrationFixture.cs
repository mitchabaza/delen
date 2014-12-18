using System;
using System.Linq;
using System.Threading;
using Delen.Agent.Communication;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using Delen.Test.Common.RavenDB;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Raven.Client.Extensions;
using Raven.Database.Server.Responders;
using ServerChannel = Delen.Server.Tests.Common.ServerChannel;

namespace Delen.Server.Tests.Functional
{
    [TestFixture]
    public class WorkerRegistrationFixture : DocumentStoreFixtureBase
    {
        [Test]
        public void Add_ShouldCreateRegistration_WhenOneDoesntExist()
        {
            var request = AutoFixture.Create<RegisterWorkerRequest>();
            var dateCommandSent = DateTime.Now;
            var response = ServerChannel.SendRequest<Response>(UriFactory.Create(EndPoint.RegisterAgent), request);

            response.Succeeded.Should().Be(true);

            var registration =
                DocumentStore.OpenSession()
                    .Query<WorkerRegistration>()
                    .Single(s => s.IPAddress.Equals(request.IPAddress));
            registration.AsOf.Should().BeAfter(dateCommandSent);
            registration.Active.Should().BeTrue();
        }

        [Test]
        public void Add_ShouldUpdateRegistration_WhenOneAlreadyExists()
        {
            var request = new RegisterWorkerRequest() {IPAddress = Guid.NewGuid().ToString()};
            var response = ServerChannel.SendRequest<Response>(UriFactory.Create(EndPoint.RegisterAgent), request);
            response.Succeeded.Should().Be(true);


            DateTime originalDate;
            using (var session = DocumentStore.OpenSession())
            {
                originalDate =
                    session.Query<WorkerRegistration>().Single(s => s.IPAddress.Equals(request.IPAddress)).AsOf;
            }

            Thread.Sleep(50);
            response = ServerChannel.SendRequest<Response>(UriFactory.Create(EndPoint.RegisterAgent), request);
            response.Succeeded.Should().Be(true);

            var entity =
                DocumentStore.OpenSession()
                    .Query<WorkerRegistration>()
                    .Single(s => s.IPAddress.Equals(request.IPAddress));
            entity.AsOf.Should().BeAfter(originalDate);
        }


        [Test]
        public void Remove_ShouldInactivateRegistration()
        {
            var registration = AutoFixture.Create<WorkerRegistration>();
            RegisterAgent(registration);
            var command = new UnregisterWorkerRequest() {IPAddress = registration.IPAddress};

            var response = ServerChannel.SendRequest<Response>(UriFactory.Create(EndPoint.UnregisterAgent), command);
            response.Succeeded.Should().Be(true);

            //must open a new session to see results
            using (var session = DocumentStore.OpenSession())
            {
                var entity = session.Load<WorkerRegistration>(registration.Id);
                entity.Active.Should().BeFalse();
            }
        }

        private void RegisterAgent(WorkerRegistration registration)
        {
            DocumentStore.Save(registration,(r)=>r.IPAddress.Equals(registration.IPAddress));
         }
    }
}
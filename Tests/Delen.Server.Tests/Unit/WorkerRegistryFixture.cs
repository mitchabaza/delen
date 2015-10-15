using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Delen.Core;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Delen.Test.Common;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Delen.Server.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]

    public class WorkerRegistryFixture : FixtureBase
    {
        private Mock<IMappingEngine> _mapper;
        private Mock<IRepository> _repository;
        private WorkerRegistry _sut;

        public override void SetupTest()
        {
            base.SetupTest();
            _mapper = AutoFixture.Freeze<Mock<IMappingEngine>>();
            _mapper.DefaultValue = DefaultValue.Mock;
            _repository = AutoFixture.Freeze<Mock<IRepository>>();
            _sut = AutoFixture.Create<WorkerRegistry>();
            _mapper.Setup(s => s.Map<RegisterWorkerRequest, WorkerRegistration>(It.IsAny<RegisterWorkerRequest>())).Returns(AutoFixture.Create<WorkerRegistration>());

        }

        [Test]
        public void UnRegister_ShouldUnregisterAgent()
        {
            var list = AutoFixture.Create<List<WorkerRegistration>>();
            _repository.Setup(r => r.Query<WorkerRegistration>()).Returns(list.AsQueryable());

            _sut.UnRegister(new UnregisterWorkerRequest(list.First().Token));
            _repository.Verify(r => r.Delete(list.First()), Times.Once());
        }
        [Test]
        public void Register_ShouldRegisterAgent_WhenNameIsUnique()
        {
         
            var list = AutoFixture.Create<List<WorkerRegistration>>();
            _repository.Setup(r => r.Query<WorkerRegistration>()).Returns(list.AsQueryable());
          
         
            _repository.Setup(r => r.Put(It.IsAny<WorkerRegistration>())).Verifiable();
            var request = AutoFixture.Create<RegisterWorkerRequest>();
            //act
            var response = _sut.Register(request);
            
            //assert
            response.Succeeded.Should().BeTrue();
            response.Payload.Should().NotBeEmpty();
            _repository.Verify(r => r.Delete(list.First()),Times.Never());
            _repository.VerifyAll();

        }
        [Test]
        public void Register_ShouldDeletePriorRegistrations_WhenAgentNameNotUnique()
        {


            var list = AutoFixture.Create<List<WorkerRegistration>>().ToList().AsQueryable();
            _repository.Setup(r => r.Query<WorkerRegistration>()).Returns(list);
      
            var request = AutoFixture.Create<RegisterWorkerRequest>();
            request.Name = list.First().Name;
          
            //act
            var response = _sut.Register(request);

          
            //assert
            response.Succeeded.Should().BeTrue();
            response.Payload.Should().NotBeEmpty();
            _repository.Verify(r => r.Delete(list.First()));
            _repository.VerifyAll();
        }
    }
}

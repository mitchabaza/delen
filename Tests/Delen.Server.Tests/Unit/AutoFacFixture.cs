using System.Collections.Specialized;
using Autofac;
using Delen.Common;
using Delen.Core;
using Delen.Core.Services;
using Delen.Server.Configuration;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Delen.Server.Tests.Unit
{
    [TestFixture]
    public class AutoFacFixture
    {
        private static IDocumentStore _documentStore;

        private static IContainer BuildContainer()
        {
            var mockConfigurationProvider = new Mock<IAppConfigurationProvider>();
            var nvCol = new NameValueCollection {{ServerConfiguration.WebApplication.AppSettingKeys.TaskScheduleTrigger, "1"}};
            mockConfigurationProvider.SetupGet(cp => cp.Settings).Returns(nvCol);
            var autofac = new AutoFac(mockConfigurationProvider.Object) {SkipRavenDbInitialization = true};
            autofac.Configure();
            //capture DocumentStore for manual disposal during test tear down.  Otherwise, subsequent tests will fail with concurrency exceptions
            _documentStore = autofac.Container.Resolve<IDocumentStore>();
            return autofac.Container;
        }

        [TearDown]
        public void TearDownTest()
        {
            _documentStore.Dispose();
        }
        [Test]
        [Ignore]
        public void JobScheduler_ShouldBeASingleton()
        {
            IContainer container = BuildContainer();
            var scheduler1 = container.Resolve<IJobQueue>();
            var scheduler2 = container.Resolve<IJobQueue>();
            scheduler1.Should().BeSameAs(scheduler2);
        }


        [Test]
        [Ignore]
        public void TaskService_ShouldBeASingleton()
        {
            IContainer container = BuildContainer();
            var task1 = container.Resolve<ITaskService>();
            var task2 = container.Resolve<ITaskService>();
            task1.Should().BeSameAs(task2);
        }
    }
}
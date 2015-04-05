using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using AutoMapper.Mappers;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Core.Services;
using Delen.Server.Controllers;
using Delen.Server.Tasks;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Extensions;
using Raven.Database.Server;
using IStartable = Delen.Server.Tasks.IStartable;

namespace Delen.Server.Configuration
{
    public class AutoFac : IAppConfigurable
    {
        private readonly IAppConfigurationProvider _configurationProvider;

        public AutoFac()
        {
            _configurationProvider = new AppConfigurationProvider();
        }

        public AutoFac(IAppConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public bool SkipRavenDbInitialization { set; get; }

        /// <summary>
        /// internal for testing only
        /// </summary>
        internal IContainer Container { get; private set; }

        public void Configure()
        {
            var builder = new ContainerBuilder();

            RegisterCore(builder);

            RegisterRaven(builder);

            RegisterAutoMapper(builder);

            RegisterWebServer(builder);

            Container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

            return;
            var startables = Container.Resolve<IEnumerable<IStartable>>();

            foreach (var startable in startables)
            {
                startable.Start();
            }
        }

        private void RegisterWebServer(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof (RegistrationController).Assembly)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.RegisterAssemblyTypes(typeof (RegistrationController).Assembly)
                .Except<Controller>()
                .Except<BackgroundTaskScheduler>()
                .Except<MultiTenantDocumentStore>()
              //  .Except<WorkerRequestContext>()
                .AsImplementedInterfaces()
                .AsSelf();
            builder.Register(
                c =>
                    new BackgroundTaskScheduler(c.Resolve<IJobRunner>(),
                        int.Parse(
                            _configurationProvider.Settings[ServerConfiguration.WebApplication.AppSettingKeys.TaskScheduleTrigger]
                            ))).AsImplementedInterfaces().AsSelf().SingleInstance();

            builder.RegisterFilterProvider();

        }

        private static void RegisterCore(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof (RegisterWorkerRequest).Assembly)
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            var tempPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Temporary");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
                
            }
            builder.Register(
                c => new JobChunkingStrategyFactory(3, tempPath)).AsImplementedInterfaces();
        }

        private static void RegisterAutoMapper(ContainerBuilder builder)
        {
            builder.Register(ctx => new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => Mapper.Engine)
                .As<IMappingEngine>();
        }

        private void RegisterRaven(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var documentStore = CreateServerDocumentStore();
                documentStore.Conventions.TransformTypeTagNameToDocumentKeyPrefix = (key) => key.ToLower();
              //  documentStore.Conventions.JsonContractResolver = new DefaultRavenContractResolver();
                if (!SkipRavenDbInitialization)
                {
                    documentStore.Initialize();
                    documentStore.Conventions.DefaultQueryingConsistency=ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
                    documentStore.DatabaseCommands.EnsureDatabaseExists(ServerConfiguration.Database.Name);
                    documentStore.DatabaseCommands.EnsureDatabaseExists(ServerConfiguration.Database.TestName);
                    
                }
                return documentStore;
            }).As<IDocumentStore>().SingleInstance();
        }

        private static IDocumentStore CreateServerDocumentStore()
        {
            var documentStore = new DocumentStore
            {
               
                Url = "http://localhost:{Port}".FormatWith(new {Port = ServerConfiguration.Database.ServerMode.RavenPort})
                
            };

            return new MultiTenantDocumentStore(documentStore);
              
        }

        private static EmbeddableDocumentStore CreateEmbeddedDocumentStore()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(ServerConfiguration.Database.EmbeddedMode.RavenPort);
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "Test_Data",
                UseEmbeddedHttpServer = true,
            };
            documentStore.Configuration.Port = ServerConfiguration.Database.EmbeddedMode.RavenPort;

            return documentStore;
        }
    }
}
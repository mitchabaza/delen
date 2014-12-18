using System;
using System.IO;
using System.Threading;
using Autofac;
using Delen.Agent.Abstractions;
using Delen.Agent.Communication;
using Delen.Agent.Tasks;

namespace Delen.Agent
{
    public static class Program
    {
        private static Application _application;
        private static Thread _thread;

        public static void Start(bool testMode)
        {
            log4net.Config.XmlConfigurator.Configure();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            _thread = Run(testMode);
        }

        private static void Main(string[] args)
        {
            Start(args.Length > 0 && args[0].ToLower() == "testmode");
            Console.WriteLine(@"Agent is now running");
            Console.WriteLine(@"Hit any key to terminate....");
            Console.ReadKey();
            Stop();
        }


        public static void Stop()
        {
            _application.Stop();
            _thread.Abort();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            _application.Stop();
        }

        private static Thread Run(bool testMode = false)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(typeof (Program).Assembly).AsSelf().AsImplementedInterfaces();
            if (testMode)
            {
                builder.RegisterType<UriFactoryForTesting>().AsImplementedInterfaces().SingleInstance();
            }
            else
            {
                builder.RegisterType<UriFactory>().AsImplementedInterfaces().SingleInstance();
            }
            builder.Register(c => Settings.FromFile(
                Path.Combine(CurrentDirectory, "agent.properties")));
            builder.Register(
                c =>
                    new TaskExecutionOptions(c.Resolve<IServerChannel>(), c.Resolve<IFileSystem>(),
                        c.Resolve<IProcessRunner>(), c.Resolve<Settings>().WorkingDirectory)).SingleInstance();

            builder.RegisterModule(new LogInjectionModule());

            IContainer container = builder.Build();
            _application = container.Resolve<Application>();

            var thread = new Thread(_application.Start);
            thread.Start();
            return thread;
        }

        private static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}
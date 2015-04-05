using System.Net;

namespace Delen.Common
{
    public static class ServerConfiguration
    {
        public static class Database
        {
            public const string Name = "Delen";
            public const string TestName = "DelenTest";

            public static class ServerMode
            {
                public const int RavenPort = 8080;
            }

            public static class EmbeddedMode
            {
                public const int RavenPort = 1111;
            }
        }

        public class WebApplication
        {
            public const string Name = "Delen";
            public const int Port = 80;
            public const string TestModeUrlPrefix = "TestMode";

            public static class AppSettingKeys
            {
                public const string TaskScheduleTrigger = "TaskScheduleTriggerMilliseconds";
            }
        }

        public static class Headers
        {
            public const string Version = "ClientVersion";
            public const string WorkerRegistrationToken = "RegistrationToken";
        }
    }
}
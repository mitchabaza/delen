using System.Net;
using Delen.Agent.Communication;
using Delen.Common;
using Delen.Test.Common.RavenDB;
using NUnit.Framework;
using Raven.Client.Document;
using Raven.Client.Extensions;
using StringFormat;

namespace Delen.Test.Common
{
    /// <summary>
    /// Test Fixture that provides access to a RavenDB DocumentStore for backdoor manipulation http://xunitpatterns.com/Back%20Door%20Manipulation.html
    /// </summary>
    public class DocumentStoreFixtureBase : FixtureBase
    {
        private const string ApplicationUrlTemplate = "http://localhost:{Port}/{ApplicationName}";
        private const string RavenUrlTemplate = "http://localhost:{Port}";
        protected DocumentStore DocumentStore;
        private TestDataCleanUpListener _cleanupListener;
        protected static readonly IUriFactory UriFactory = new UriFactoryForTesting(Settings.Default);
        private bool _dontCleanUp;

        [TestFixtureSetUp]
        public void InitializeFixture()
        {
       
            InitializeDocumentStore();
        }

        private void InitializeDocumentStore()
        {
            DocumentStore = new TestDocumentStore()
            {

                Url =ServerUrl,
                DefaultDatabase = ServerConfiguration.Database.TestName
            };
            _cleanupListener = new TestDataCleanUpListener();
            DocumentStore.RegisterListener(_cleanupListener);
            DocumentStore.RegisterListener(new NoStaleQueriesListener());
            DocumentStore.Conventions.TransformTypeTagNameToDocumentKeyPrefix = (key) => key.ToLower();
            DocumentStore.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            DocumentStore.Initialize();
            DocumentStore.DatabaseCommands.EnsureDatabaseExists(ServerConfiguration.Database.TestName);
            DocumentStore.DatabaseCommands.DisableAllCaching();
       
        }

        public string EmbeddedUrl
        {
            get
            {
                return TokenStringFormat.Format(RavenUrlTemplate,
                    new
                    {
                        Port = ServerConfiguration.Database.EmbeddedMode.RavenPort,
                        Database = ServerConfiguration.Database.TestName
                    });
            }
        }
        public string ServerUrl
        {
            get
            {
                return TokenStringFormat.Format(RavenUrlTemplate,
                    new
                    {
                        Port = ServerConfiguration.Database.ServerMode.RavenPort,
                        Database = ServerConfiguration.Database.Name
                    });
            }
        }
        /// <summary>
        /// When RavenDb is running in "embedded" mode, the hosting web application acts as the container.  This means that until the web app is loaded, 
        /// a connection can't be established to RavenDB.  This method hits the web application to ensure that it, and by extension RavenDB, are loaded 
        /// </summary>
        private void LoadWebApp()
        {
            using (var client = new WebClient())
            {
                client.DownloadData((string) TokenStringFormat.Format(ApplicationUrlTemplate,
                    new
                    {
                        ServerConfiguration.WebApplication.Port,
                        ApplicationName = ServerConfiguration.WebApplication.Name
                    }));
            }
        }


        public override void SetupTest()
        {
            base.SetupTest();
            DocumentStore.DeleteAllDocuments();
        }

        public override void TearDownTest()
        {
            base.TearDownTest();
            if (!_dontCleanUp)
            DocumentStore.DeleteAllDocuments();
        }

        protected void DisableDatabaseCleanUp(bool flag)
        {
            _dontCleanUp = flag;
        }

        private void CleanupEntities()
        {
            if (!_dontCleanUp)
            {
                using (var session = DocumentStore.OpenSession())
                {
                    _cleanupListener.CleanUp(session);
                }
            }
            DocumentStore.Dispose();
        }

       
    }
}
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Delen.Test.Common
{
    public abstract class FixtureBase
    {
        protected Fixture AutoFixture;
        /// <summary>
        /// set to true when debugging AutoFixture object instantiation issues
        /// </summary>
        protected bool EnableAutoFixtureLogging { get; private set; }
        protected static string CurrentDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }
        protected IList<Mock<object>> Mocks;
        [SetUp]
        public virtual void  SetupTest()
        {
            InitAutoFixture();
        }
        [TearDown]
        public virtual void TearDownTest()
        {
            InitAutoFixture();
        }
        protected void InitAutoFixture()
        {
            AutoFixture = new Fixture();
            AutoFixture.Customize(new DelenAutoFixtureCustomization());
        }
    }
}
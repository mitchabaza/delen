using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delen.Core.Entities;
using Delen.Core.Services;
using Delen.Test.NUnitSample;
using NUnit.Core;
using NUnit.Framework;
using Delen.Test.Common;
using Ploeh.AutoFixture;

namespace Delen.Server.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class NUnitJobChunkingStrategyFixture : FixtureBase
    {
        private NUnitJobChunkingStrategy _strategy;
        private Job _job;

        public override void SetupTest()
        {
            base.SetupTest();
            _strategy = new NUnitJobChunkingStrategy(1, Path.GetTempPath());
            _job = AutoFixture.Create<Job>();
        }

        [Test]
        public void Shizer()
        {
           var pathToTestLibrary = CurrentDirectory + @"..\..\..\..\Delen.Test.NUnitSample\bin\Debug\" +
                                    BaseFixture.AssemblyName + ".dll"; 

            //  strategy.Chunk()
        }
    }
}
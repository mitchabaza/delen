using System.Collections.Generic;
using System.Linq;
using Delen.Core.Services;
using Delen.Test.Common;
using Delen.Test.NUnitSample;
using FluentAssertions;
using NUnit.Framework;

namespace Delen.Server.Tests.Unit
{
    public class NUnitAssemblyInspectorFixture:FixtureBase
    {
        [Test]
        public void ListTests_ShouldRetrieveAllTests()
        {
            var pathToTestLibrary = CurrentDirectory + @"..\..\..\..\Delen.Test.NUnitSample\bin\Debug\" +
                                   BaseFixture.AssemblyName + ".dll";
            var sut = new NUnitAssemblyInspector();
            var actualTests=sut.ListAll(pathToTestLibrary);
                       actualTests.Count().Should().Be(18);
       
        }
        
    }
}
using System.IO;
using System.Linq;
using NUnit.Core;

namespace Delen.Core.Services
{
    using System.Collections.Generic;

    public class NUnitAssemblyInspector
    {
        public IEnumerable<string> ListAll(string pathToTestLibrary)
        {
            CoreExtensions.Host.InitializeService();

            var testPackage = new TestPackage(pathToTestLibrary)
            {
                BasePath = Path.GetDirectoryName(pathToTestLibrary)
            };
            var builder = new TestSuiteBuilder();
            TestSuite suite = builder.Build(testPackage);
            var tests = new List<string>();

            return InternalList(tests, suite);
        }

        private IEnumerable<string> InternalList(ICollection<string> tests, ITest rootTest)
        {
            foreach (var test in rootTest.Tests.Cast<ITest>())
            {
                if (test.Tests != null && test.Tests.Count > 0)
                {
                    InternalList(tests, test);
                }
                else
                {
                    tests.Add(test.TestName.FullName);
                }
            }
            return tests;
        }
    }
} 
using System.Collections.Generic;
using NUnit.Framework;

namespace Delen.Test.NUnitSample
{
    public class BaseFixture
    {
        public const string AssemblyName = "Delen.Test.NUnitSample";
   
        [TestCase(1, 1, 2)]
        [TestCase(5, 3, 8)]
        [TestCase(1, 3, 4)]
        [TestCase(1, 5, 6)]
        public void Addition_ShouldWork(int operand1, int operand2, int sum)
        {
            Assert.AreEqual(operand1 + operand2, sum);
        }

        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                return new List<TestCaseData>()
                {
                    new TestCaseData(10, 5, 2),
                    new TestCaseData(20, 5, 4),
                };
            }
        }

        [TestCaseSource(typeof (BaseFixture), "TestData")]
        public void Division_ShouldWork(int operand1, int operand2, int result)
        {
            Assert.AreEqual(operand1/operand2,result);
            
        }
        [Test]
        public void OnePlusOne_ShouldEqualTwo()
        {
            Assert.AreEqual(1 + 1, 2);
        }

        [Test]
        public void OnePlusTwo_ShouldEqualThree()
        {
            Assert.AreEqual(1 + 2, 3);
        }

        [Test]
        public void TenDividedByTwo_ShouldEqualFive()
        {
            Assert.AreEqual(10/2, 5);
        }
    }
}
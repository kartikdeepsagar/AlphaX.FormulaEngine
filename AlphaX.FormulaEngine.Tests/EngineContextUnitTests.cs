using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class EngineContextUnitTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine(new TestEngineContext());
        }

        [TestCase("IF(2 >= $CustomName1, \"yes\", \"no\")", "yes")]
        [TestCase("IF(10 < $CustomName2, \"yes\", \"no\")", "no")]
        [TestCase("\"TestString\" = $CustomName3", true)]
        public void SuccessTests(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output), result.Error);
        }

        [TestCase("CustomNames", "no")]
        [TestCase("IF(10 < $CustomNae2, \"yes\", \"no\")", "yes")]
        [TestCase("\"TestString\" = $Name", false)]
        public void FailureTests(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }

    public class TestEngineContext : IEngineContext
    {
        public object Resolve(string key)
        {
            switch (key)
            {
                case "CustomName1":
                    return 2;

                case "CustomName2":
                    return 10;

                case "CustomName3":
                    return "TestString";
            }

            throw new Exception("Invalid custom name");
        }
    }
}

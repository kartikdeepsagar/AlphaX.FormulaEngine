using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class UpperFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("UPPER(\"john\")", "JOHN")]
        [TestCase("UPPER(\"siMoN\")", "SIMON")]
        [TestCase("UPPER(\"ROBERT\")", "ROBERT")]
        [TestCase("UPPER(\"xYz1\")", "XYZ1")]
        public void UpperFormula_SuccessTests(string input, string output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("UPPER(\"\"d)")]
        [TestCase("UPPER(1,\"SD SD\")")]
        [TestCase("UPPER()")]
        [TestCase("UPPER\"SD SD\"")]
        public void UpperFormula_FailureTests(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}
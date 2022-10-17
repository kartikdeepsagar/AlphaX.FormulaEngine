using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class LowerFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("LOWER(\"john\")", "john")]
        [TestCase("LOWER(\"siMoN\")", "simon")]
        [TestCase("LOWER(\"ROBERT\")", "robert")]
        [TestCase("LOWER(\"xYz1\")", "xyz1")]
        public void LowerFormula_SuccessTest(string input, string output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("LOWER(\"\"\"a)")]
        [TestCase("LOWER(1,\"SD SD\")")]
        [TestCase("LOWER()")]
        [TestCase("LOWER\"SD SD\"")]
        public void LowerFormula_FailureTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}
using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class ArrayFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("ARRAYCONTAINS([1,2,3,4], 4)", true)]
        [TestCase("ARRAYCONTAINS([1,4], 2)", false)]
        [TestCase("ARRAYCONTAINS([1,2,\"test\",4], \"test\")", true)]
        public void ArrayContainsFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("ARRAYINCLUDES([1,2,3,4], [1,2])", true)]
        [TestCase("ARRAYINCLUDES( [1,2], [1,2,3,4])", false)]
        [TestCase("ARRAYINCLUDES([1,4], [5])", false)]
        [TestCase("ARRAYINCLUDES([1,2,\"test\",4], [\"test\", 2])", true)]
        public void ArrayIncludesFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }
    }
}
using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class SumFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("SUM(1,2,3,4)", 10)]
        [TestCase("SUM(0 ,  12,  3,1)", 16)]
        [TestCase("SUM(-1 ,  12,  3,1)", 15)]
        [TestCase("SUM(1.1,2.1, 3, 4.2)", 10.4)]
        [TestCase("SUM(1, SUM(1,2,SUM(2,2)), 4)", 12)]
        public void SumFormula_SuccessTests(string input, double output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("SUM(1,2,3 4)")]
        [TestCase("SUM(0- ,  12,  3,1)")]
        [TestCase("SUM,  12,  3,1)")]
        [TestCase("SUM(..1,2.1, 3, 4.2)")]
        public void SumFormula_FailureTests(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}
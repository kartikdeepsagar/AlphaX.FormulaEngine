using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class IFFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("IF(1 > 2, true, false)", false)]
        [TestCase("IF(1 == 1, true, false)", true)]
        [TestCase("IF(UPPER(UPPER(\"GraPecity\")) == UPPER(\"Grapecity\"), true, false)", true)]
        [TestCase("IF(1 != 1, true, false)", false)]
        [TestCase("IF(5 <= 6, \"true\", \"false\")", "true")]
        [TestCase("IF(\"test\" == \"test\", \"true\", \"false\")", "true")]
        public void IFFormula_SuccessTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("IF(1s > 2, true, false)", false)]
        [TestCase("IF(1s == \"sdsd\", true, false)", false)]
        [TestCase("IF(1 + 1, true, false)", true)]
        [TestCase("IF(5 << 6, \"true\", \"false\")", "sad")]
        [TestCase("IF(\"test\" sd \"test\", \"true\", \"false\")", "asd")]
        public void IFFormula_FailureTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}
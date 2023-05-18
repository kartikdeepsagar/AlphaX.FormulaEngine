using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class ConditionTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("1 < 2", true)]
        [TestCase("1 <= 2", true)]
        [TestCase("1.23545 <= 2.24555", true)]
        [TestCase("5.65 > 2.987", true)]
        [TestCase("23478 >= 2234", true)]
        [TestCase("1 > 2", false)]
        [TestCase("1000 == 1000", true)]
        [TestCase("1.5 == 1.5", true)]
        [TestCase("1.5 != 1.5", false)]
        [TestCase("\"string\" == \"string\"", true)]
        [TestCase("\"string\" == \"String\"", false)]
        [TestCase("\"string\" != \"String\"", true)]
        [TestCase("\"string\" == \"1\"", false)]
        [TestCase("\"string\" == \"1\"", false)]
        [TestCase("true == true", true)]
        [TestCase("true == false", false)]
        [TestCase("true && true", true)]
        [TestCase("true || true", true)]
        [TestCase("true && true && false", false)]
        [TestCase("true && true && true", true)]
        [TestCase("false || false || true", true)]
        [TestCase("false || true && false", false)]
        public void ConditionBasic_SuccessTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("SUM([1,2]) > SUM([0])", true)]
        [TestCase("SUM([1,20]) > 5.5", true)]
        [TestCase("SUM([1, SUM([5,3])]) == 9", true)]
        [TestCase("SUM([1, SUM([5,3])]) != 9", false)]
        [TestCase("SUM([1, SUM([5,3])]) == SUM([1, SUM([5,3])])", true)]
        [TestCase("SUM([1, SUM([5,3,SUM([5,9.42])])]) == SUM([1, SUM([5,3,SUM([5,9.42])])])", true)]
        [TestCase("1.323 <= SUM([1.23])", false)]
        [TestCase("1.323 > SUM([1.23])", true)]
        public void ConditionComplex_SuccessTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("== SUM([1,2])")]
        [TestCase(">= SUM([1,2])")]
        [TestCase("<= SUM([1,2])")]
        [TestCase("<= SUM([1,2])")]
        [TestCase("==1")]
        [TestCase("!==1")]
        [TestCase("<=2")]
        public void Condition_FailureTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}
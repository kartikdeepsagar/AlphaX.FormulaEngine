using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class LogicalFormulaQueryOperatorTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
            _formulaEngine.ApplySettings(new EngineSettings()
            {
                LogicalOperatorMode = LogicalOperatorMode.Query
            });
        }

        #region Condition Tests
        [TestCase("1 lt 2", true)]
        [TestCase("1 le 2", true)]
        [TestCase("1.23545 le 2.24555", true)]
        [TestCase("5.65 gt 2.987", true)]
        [TestCase("23478 ge 2234", true)]
        [TestCase("1 gt 2", false)]
        [TestCase("1000 eq 1000", true)]
        [TestCase("1.5 eq 1.5", true)]
        [TestCase("1.5 ne 1.5", false)]
        [TestCase("\"string\" eq \"string\"", true)]
        [TestCase("\"string\" eq \"String\"", false)]
        [TestCase("\"string\" ne \"String\"", true)]
        [TestCase("\"string\" eq \"1\"", false)]
        [TestCase("\"string\" eq \"1\"", false)]
        [TestCase("true eq true", true)]
        [TestCase("true eq false", false)]
        [TestCase("true and true", true)]
        [TestCase("true or true", true)]
        [TestCase("true and true and false", false)]
        [TestCase("true and true and true", true)]
        [TestCase("false or false or true", true)]
        [TestCase("false or true and false", false)]
        public void ConditionBasic_SuccessTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("SUM([1,2]) gt SUM([0])", true)]
        [TestCase("SUM([1,20]) gt 5.5", true)]
        [TestCase("SUM([1, SUM([5,3])]) eq 9", true)]
        [TestCase("SUM([1, SUM([5,3])]) ne 9", false)]
        [TestCase("SUM([1, SUM([5,3])]) eq SUM([1, SUM([5,3])])", true)]
        [TestCase("SUM([1, SUM([5,3,SUM([5,9.42])])]) eq SUM([1, SUM([5,3,SUM([5,9.42])])])", true)]
        [TestCase("1.323 le SUM([1.23])", false)]
        [TestCase("1.323 gt SUM([1.23])", true)]
        public void ConditionComplex_SuccessTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("eqeq SUM([1,2])")]
        [TestCase("ge SUM([1,2])")]
        [TestCase("le SUM([1,2])")]
        [TestCase("le SUM([1,2])")]
        [TestCase("eqeq1")]
        [TestCase("neeq1")]
        [TestCase("le2")]
        public void Condition_FailureTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
        #endregion

        [TestCase("IF(1 gt 2, true, false)", false)]
        [TestCase("IF(1 eq 1, true, false)", true)]
        [TestCase("IF(UPPER(UPPER(\"GraPecity\")) eq UPPER(\"Grapecity\"), true, false)", true)]
        [TestCase("IF(1 ne 1, true, false)", false)]
        [TestCase("IF(5 le 6, \"true\", \"false\")", "true")]
        [TestCase("IF(\"test\" eq \"test\", \"true\", \"false\")", "true")]
        [TestCase("IF(true and true, true, false)", true)]
        [TestCase("IF(true and false, true, false)", false)]
        [TestCase("IF(SUM([1,2]) eq SUM([2,1]), true, false)", true)]
        public void IFFormula_SuccessTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("IF(1s gt 2, true, false)", false)]
        [TestCase("IF(1s eq \"sdsd\", true, false)", false)]
        [TestCase("IF(1 + 1, true, false)", true)]
        [TestCase("IF(5 ltlt 6, \"true\", \"false\")", "sad")]
        [TestCase("IF(\"test\" sd \"test\", \"true\", \"false\")", "asd")]
        public void IFFormula_FailureTest(string input, object output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}

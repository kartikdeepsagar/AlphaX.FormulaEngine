using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class OperatorFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("EQUALS(1,1)", true)]
        [TestCase("EQUALS(1,2)", false)]
        [TestCase("EQUALS(\"test\",\"test\")", true)]
        [TestCase("EQUALS(\"test\",\"tests\")", false)]
        [TestCase("EQUALS(1.22,1.22)", true)]
        [TestCase("EQUALS(1.22,1.223)", false)]
        [TestCase("EQUALS(true,false)", false)]
        [TestCase("EQUALS(true,true)", true)]
        public void EqualsFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }


        [TestCase("GREATERTHAN(1,1)", false)]
        [TestCase("GREATERTHAN(5,2)", true)]
        [TestCase("GREATERTHAN(1.22,1.22)", false)]
        [TestCase("GREATERTHAN(1.22,1.223)", false)]
        [TestCase("GREATERTHAN(98934,32423)", true)]
        public void GreaterThanFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("GREATERTHANEQUALS(1,1)", true)]
        [TestCase("GREATERTHANEQUALS(5,2)", true)]
        [TestCase("GREATERTHANEQUALS(1.22,1.22)", true)]
        [TestCase("GREATERTHANEQUALS(1.22,1.223)", false)]
        [TestCase("GREATERTHANEQUALS(98934,32423)", true)]
        public void GreaterThanEqualsFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("LESSTHAN(1,1)", false)]
        [TestCase("LESSTHAN(5,2)", false)]
        [TestCase("LESSTHAN(1.22,1.22)", false)]
        [TestCase("LESSTHAN(1.22,1.223)", true)]
        [TestCase("LESSTHAN(98934,32423)", false)]
        public void LessThanFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("LESSTHANEQUALS(1,1)", true)]
        [TestCase("LESSTHANEQUALS(5,2)", false)]
        [TestCase("LESSTHANEQUALS(1.22,1.22)", true)]
        [TestCase("LESSTHANEQUALS(1.22,1.223)", true)]
        [TestCase("LESSTHANEQUALS(98934,32423)", false)]
        public void LessThanEqualsFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("NOT(EQUALS(1,1))", false)]
        [TestCase("NOT(EQUALS(1,2))", true)]
        [TestCase("NOT(true)", false)]
        [TestCase("NOT(false)", true)]
        [TestCase("NOT(1 == 2)", true)]
        [TestCase("NOT(2 == 2)", false)]
        [TestCase("NOT(true && false)", true)]
        [TestCase("NOT(\"test\" == \"tes\")", true)]
        public void NotFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("AND(1 == 2, 2 != 2)", false)]
        [TestCase("AND(true, false)", false)]
        [TestCase("AND(false, false)", false)]
        [TestCase("AND(true, true)", true)]
        [TestCase("AND(true && false, true)", false)]
        [TestCase("AND(\"test\" == \"tes\", false)", false)]
        public void ANDFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("OR(1 == 2, 2 != 2)", false)]
        [TestCase("OR(true, false)", true)]
        [TestCase("OR(false, false)", false)]
        [TestCase("OR(true, true)", true)]
        [TestCase("OR(true && false, true)", true)]
        [TestCase("OR(\"test\" == \"tes\", false)", false)]
        public void ORFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }
    }
}
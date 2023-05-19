using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class StringFormulaTests
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

        [TestCase("LENGTH(\"john\")", 4)]
        [TestCase("LENGTH(\"siMoN\")", 5)]
        [TestCase("LENGTH(\"ROBERT\")", 6)]
        [TestCase("LENGTH(\"xYz1\")", 4)]
        [TestCase("LENGTH(\"\")", 0)]
        public void LengthFormula_SuccessTest(string input, int output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("STARTSWITH(\"TestCom\", \"Test\")", true)]
        [TestCase("STARTSWITH(\"TestCom\", \"TEST\")", true)]
        [TestCase("STARTSWITH(\"TestCom\", \"TEST\", false)", true)]
        [TestCase("STARTSWITH(\"TestCom\", \"TEST\", true)", false)]
        public void StartsWithFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("CONTAINS(\"TestCom\", \"es\")", true)]
        [TestCase("CONTAINS(\"TestCom\", \"Co\")", true)]
        [TestCase("CONTAINS(\"TestCom\", \"est\")", true)]
        [TestCase("CONTAINS(\"TestComputer123\", \"er12\")", true)]
        [TestCase("CONTAINS(\"TestComputer123\", \"xyz\")", false)]
        public void ContainsFormula_SuccessTest(string input, bool output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("ENDSWITH(\"TestCom\", \"Com\")", true)]
        [TestCase("ENDSWITH(\"TestCom\", \"COM\")", true)]
        [TestCase("ENDSWITH(\"TestCom\", \"COM\", false)", true)]
        [TestCase("ENDSWITH(\"TestCom\", \"COM\", true)", false)]
        public void EndsWithFormula_SuccessTest(string input, bool output)
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

        [TestCase("TEXTSPLIT(\",\", \"1,2,3,4\")", 4)]
        [TestCase("TEXTSPLIT(\",\", \"Simon,Bob,Rob,Tim\")", 4)]
        [TestCase("TEXTSPLIT(\":\", \"Color:Red\")", 2)]
        [TestCase("TEXTSPLIT(\" \", UPPER(\"John Marley Morston\"))", 3)]
        public void TextSplitFormula_SuccessTest(string input, int output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That((result.Value as string[])?.Length, Is.EqualTo(output));
        }

        [TestCase("TEXTSPLIT(\"\",,\"\")")]
        [TestCase("TEXTSPLIT(\"johny\")")]
        [TestCase("TEXTSPLIT,  12,  3,1)")]
        [TestCase("TEXTSPLIT(12)")]
        public void TextSplitFormula_SuccessTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }

        [TestCase("UPPER(\"john\")", "JOHN")]
        [TestCase("UPPER(\"siMoN\")", "SIMON")]
        [TestCase("UPPER(\"ROBERT\")", "ROBERT")]
        [TestCase("UPPER(\"xYz1\")", "XYZ1")]
        public void UpperFormula_SuccessTest(string input, string output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("UPPER(\"\"d)")]
        [TestCase("UPPER(1,\"SD SD\")")]
        [TestCase("UPPER()")]
        [TestCase("UPPER\"SD SD\"")]
        public void UpperFormula_FailureTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}

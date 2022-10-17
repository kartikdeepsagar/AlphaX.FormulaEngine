using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class TextSplitFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("TEXTSPLIT(\",\", \"1,2,3,4\")", 4)]
        [TestCase("TEXTSPLIT(\",\", \"Simon,Bob,Rob,Tim\")", 4)]
        [TestCase("TEXTSPLIT(\":\", \"Color:Red\")", 2)]
        [TestCase("TEXTSPLIT(\" \", UPPER(\"John Marley Morston\"))", 3)]
        public void TextSplitFormula_SuccessTests(string input, int output)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That((result.Value as string[])?.Length, Is.EqualTo(output));
        }

        [TestCase("TEXTSPLIT(\"\",,\"\")")]
        [TestCase("TEXTSPLIT(\"johny\")")]
        [TestCase("TEXTSPLIT,  12,  3,1)")]
        [TestCase("TEXTSPLIT(12)")]
        public void TextSplitFormula_SuccessTests(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.IsNotNull(result.Error);
        }
    }
}
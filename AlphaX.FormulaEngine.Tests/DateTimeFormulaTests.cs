using System.Globalization;
using NUnit.Framework;

namespace AlphaX.FormulaEngine.Tests
{
    public class DateTimeFormulaTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("TODAY()")]
        public void Today_SuccessTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(DateTime.Now.Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)));
        }

        [TestCase("TODAY(\"yyyy-mm-dd\")")]
        public void TodayWithFormat_SuccessTest(string input)
        {
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(DateTime.Now.Date.ToString("yyyy-mm-dd")));
        }
    }
}

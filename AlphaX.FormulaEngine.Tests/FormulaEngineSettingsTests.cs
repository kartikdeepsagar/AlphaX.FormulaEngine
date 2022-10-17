using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaX.FormulaEngine.Tests
{
    public class FormulaEngineSettingsTests
    {
        private IFormulaEngine _formulaEngine;

        [OneTimeSetUp]
        public void Setup()
        {
            _formulaEngine = new AlphaXFormulaEngine();
        }

        [TestCase("SUM[1,2,3,4]", 10)]
        [TestCase("SUM[0 ,  12,  3,1]", 16)]
        [TestCase("SUM[-1 ,  12,  3,1]", 15)]
        [TestCase("SUM[1.1,2.1, 3, 4.2]", 10.4)]
        [TestCase("SUM[1, SUM[1,2,SUM[2,2]], 4]", 12)]
        public void FormulaSettings_SuccessTest(string input, double output)
        {
            _formulaEngine.Settings.OpenBracketSymbol = "[";
            _formulaEngine.Settings.CloseBracketSymbol = "]";
            _formulaEngine.Settings.Update();
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("SUM{1|2|3|4}", 10)]
        [TestCase("SUM{0 |  12|  3|1}", 16)]
        [TestCase("SUM{-1 |  12|  3|1}", 15)]
        [TestCase("SUM{1.1|2.1| 3| 4.2}", 10.4)]
        [TestCase("SUM{1| SUM{1|2|SUM{2|2}}| 4}", 12)]
        public void FormulaSettings_SuccessTest2(string input, double output)
        {
            _formulaEngine.Settings.OpenBracketSymbol = "{";
            _formulaEngine.Settings.CloseBracketSymbol = "}";
            _formulaEngine.Settings.ArgumentsSeparatorSymbol = "|";
            _formulaEngine.Settings.Update();
            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        public void FormulaSettings_FailureTest(string input, double output)
        {
            _formulaEngine.Settings.OpenBracketSymbol = null;
            Assert.Throws<NotSupportedException>(() => { _formulaEngine.Settings.Update(); });
        }

        public void FormulaSettings_FailureTests2(string input, double output)
        {
            _formulaEngine.Settings.CloseBracketSymbol = null;
            Assert.Throws<NotSupportedException>(() =>{ _formulaEngine.Settings.Update(); });
        }

        public void FormulaSettings_FailureTests3(string input, double output)
        {
            _formulaEngine.Settings.ArgumentsSeparatorSymbol = null;
            Assert.Throws<NotSupportedException>(() =>{ _formulaEngine.Settings.Update(); });
        }
    }
}

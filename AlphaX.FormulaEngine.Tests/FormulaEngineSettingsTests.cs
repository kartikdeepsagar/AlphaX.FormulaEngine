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

        [TestCase("SUM[[1,2,3,4]]", 10)]
        [TestCase("SUM[[0 ,  12,  3,1]]", 16)]
        [TestCase("SUM[[-1 ,  12,  3,1]]", 15)]
        [TestCase("SUM[[1.1,2.1, 3, 4.2]]", 10.4)]
        [TestCase("SUM[[1, SUM[[1,2,SUM[[2,2]]]], 4]]", 12)]
        public void FormulaSettings_SuccessTest(string input, double output)
        {
            _formulaEngine.ApplySettings(new EngineSettings()
            {
                OpenBracketSymbol = "[",
                CloseBracketSymbol = "]",
            });

            var result = _formulaEngine.Evaluate(input);
            Assert.That(result.Value, Is.EqualTo(output));
        }

        [TestCase("SUM{[1,2,3,4]}", 10)]
        [TestCase("SUM{[0 ,  12,  3,1]}", 16)]
        [TestCase("SUM{[-1 ,  12,  3,1]}", 15)]
        [TestCase("SUM{[1.1,2.1, 3, 4.2]}", 10.4)]
        [TestCase("TEXTSPLIT{\";\"| \"test;sds\"}", 23)]
        public void FormulaSettings_SuccessTest2(string input, double output)
        {
            _formulaEngine.ApplySettings(new EngineSettings()
            {
                OpenBracketSymbol = "{",
                CloseBracketSymbol = "}",
                ArgumentsSeparatorSymbol = "|",
            });

            var result = _formulaEngine.Evaluate(input);
            Assert.IsNull(result.Error);
        }

        public void FormulaSettings_FailureTest(string input, double output)
        {
            Assert.Throws<ArgumentNullException>(() => {

                _formulaEngine.ApplySettings(new EngineSettings()
                {
                    OpenBracketSymbol = null
                });
            });
        }

        public void FormulaSettings_FailureTests2(string input, double output)
        {
            Assert.Throws<ArgumentNullException>(() => {

                _formulaEngine.ApplySettings(new EngineSettings()
                {
                    OpenBracketSymbol = null
                });
            });
        }

        public void FormulaSettings_FailureTests3(string input, double output)
        {
            Assert.Throws<ArgumentNullException>(() => {

                _formulaEngine.ApplySettings(new EngineSettings()
                {
                    ArgumentsSeparatorSymbol = null
                });
            });
        }
    }
}

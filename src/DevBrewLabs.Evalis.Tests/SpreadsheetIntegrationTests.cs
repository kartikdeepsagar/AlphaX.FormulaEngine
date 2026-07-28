using DevBrewLabs.Parserly;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DevBrewLabs.Evalis.Tests
{
    internal class SpreadsheetTokenParser : Parser<StringResult>
    {
        private Regex _regex;

        public SpreadsheetTokenParser(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }

        protected override IParserState ParseInput(IParserState inputState)
        {
            var input = inputState.ActualInput.Substring(inputState.Index);

            Match match = _regex.Match(input);

            if (match.Success)
            {
                return ParserStates.Result(inputState, new StringResult(match.Value), inputState.Index + match.Value.Length);
            }
            else
            {
                return ParserStates.Error(inputState, new ParserError(inputState.Index, "No match"));
            }
        }
    }

    [TestFixture]
    public class SpreadsheetIntegrationTests
    {
        [Test]
        public void SpreadsheetTokens_Are_Extracted_Successfully()
        {
            var settings = new EngineSettings();
            settings.CustomTokenParsers = new List<IParser>
            {
                new SpreadsheetTokenParser(@"^[A-Za-z0-9_]+![A-Za-z]+[0-9]+:[A-Za-z]+[0-9]+"), // Sheet1!A1:B10
                new SpreadsheetTokenParser(@"^[A-Za-z0-9_]+![A-Za-z]+[0-9]+"), // Sheet1!A1
                new SpreadsheetTokenParser(@"^[A-Za-z]+[0-9]+:[A-Za-z]+[0-9]+"), // A1:B10
                new SpreadsheetTokenParser(@"^[A-Za-z]+[0-9]+") // A1
            };

            var engine = new FormulaEngine();
            engine.ApplySettings(settings);

            var variables = engine.ExtractVariables("SUM(Sheet1!A1:B10, A2, Sheet2!C4)");

            Assert.That(variables.Length, Is.EqualTo(3));
            Assert.That(variables[0], Is.EqualTo("Sheet1!A1:B10"));
            Assert.That(variables[1], Is.EqualTo("A2"));
            Assert.That(variables[2], Is.EqualTo("Sheet2!C4"));
        }
    }
}

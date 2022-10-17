using AlphaX.Parserz;
using AlphaX.Parserz.Extensions;
using AlphaX.Parserz.Interfaces;
using AlphaX.Parserz.Results;
using System.Reflection;

namespace AlphaX.FormulaEngine
{
    internal static class ParserProvider
    {
        static ParserProvider()
        {
            var whiteSpacesParser = Parser.WhiteSpace.Many().MapResult(x => x.ToStringResult());

            var openBracketResult = new CharResult('(');
            var closeBracketResult = new CharResult(')');
            OpenBracketParser = Parser.Char('(').AndThen(whiteSpacesParser).MapResult(x => openBracketResult);
            CloseBracketParser = Parser.Char(')').AndThen(whiteSpacesParser).MapResult(x => closeBracketResult);

            var lettersParser = Parser.Letter.Many(1)
                .MapError(x => new ParserError(x.Index, $"Position ({x.Index}). Unexpected input, Expected formula name."))
                .MapResult(x => x.ToStringResult());

            FormulaNameParser = lettersParser
                .AndThen(whiteSpacesParser)
                .MapResult(x => new FormulaNameResult(x.Value[0].Value.ToString()));

            var digitsParser = Parser.Digit.Many(1)
                .MapResult(x => x.ToDoubleResult())
                .AndThen(whiteSpacesParser).MapResult(x => x.Value[0]);

            var negativeDigitsParser = Parser.Char('-').AndThen(digitsParser)
                .MapResult(x => x.ToDoubleResult());

            var lettersOrDigitsParser = Parser.LetterOrDigit.Many(1)
                .MapError(x => new ParserError(x.Index, $"Position ({x.Index}). Unexpected input, Expected string value."))
                .MapResult(x => x.ToStringResult());

            var stringValueParser = new StringValueParser()
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            FormulaArgumentParser = Parser.Lazy(() => FormulaParser)
                .Or(Parser.Decimal)
                .Or(negativeDigitsParser)
                .Or(digitsParser)
                .Or(stringValueParser);

            var commaResult = new CharResult(',');
            var commaParser = Parser.Char(',').AndThen(whiteSpacesParser).MapResult(x => commaResult);

            FormulaParser = FormulaNameParser
                .AndThen(OpenBracketParser)
                .AndThen(FormulaArgumentParser.ManySeptBy(commaParser))
                .AndThen(CloseBracketParser);
        }

        private static IParser FormulaNameParser { get; }
        private static IParser OpenBracketParser { get; }
        private static IParser CloseBracketParser { get;  }
        private static IParser FormulaArgumentParser { get; }
        public static IParser FormulaParser { get; }
    }
}

using AlphaX.FormulaEngine.Resources;
using AlphaX.Parserz;
using AlphaX.Parserz.Extensions;
using AlphaX.Parserz.Interfaces;
using AlphaX.Parserz.Results;
using System.Globalization;

namespace AlphaX.FormulaEngine
{
    internal static class ParserFactory
    {
        public static IParser FormulaParser { get; set; }

        public static void BuildParser(FormulaEngineSettings settings)
        {
            var whiteSpacesParser = Parser.WhiteSpace.Many().MapResult(x => x.ToStringResult());

            var openBracketResult = new FormulaBracketResult(settings.OpenBracketSymbol);
            var openBracketParser = Parser.String(settings.OpenBracketSymbol)
                .AndThen(whiteSpacesParser)
                .MapResult(x => openBracketResult);

            var closeBracketResult = new FormulaBracketResult(settings.CloseBracketSymbol);
            var closeBracketParser = Parser.String(settings.CloseBracketSymbol)
                .AndThen(whiteSpacesParser)
                .MapResult(x => closeBracketResult);

            var lettersParser = Parser.Letter.Many(1)
                .MapError(x => new ParserError(x.Index, string.Format(EngineResources.UnexpectedInput, x.Index, "formula name")))
                .MapResult(x => x.ToStringResult());

            var formulaNameParser = lettersParser
                .AndThen(whiteSpacesParser)
                .MapResult(x => new FormulaNameResult(x.Value[0].Value.ToString()));

            var digitsParser = Parser.Digit.Many(1)
                .MapResult(x => x.ToDoubleResult())
                .AndThen(whiteSpacesParser).MapResult(x => x.Value[0]);

            var negativeDigitsParser = Parser.String(CultureInfo.CurrentCulture.NumberFormat.NegativeSign)
                .AndThen(digitsParser)
                .MapResult(x => x.ToDoubleResult());

            var lettersOrDigitsParser = Parser.LetterOrDigit.Many(1)
                .MapError(x => new ParserError(x.Index, string.Format(EngineResources.UnexpectedInput, x.Index, "string value")))
                .MapResult(x => x.ToStringResult());

            var stringValueParser = new StringValueParser()
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var formulaArgumentParser = Parser.Lazy(() => FormulaParser)
                .Or(Parser.Decimal)
                .Or(negativeDigitsParser)
                .Or(digitsParser)
                .Or(stringValueParser);

            var commaResult = new FormulaArgumentSeperatorResult(settings.ArgumentsSeparatorSymbol);
            var commaParser = Parser.String(settings.ArgumentsSeparatorSymbol)
                .AndThen(whiteSpacesParser)
                .MapResult(x => commaResult);

            FormulaParser = formulaNameParser
                .AndThen(openBracketParser)
                .AndThen(formulaArgumentParser.ManySeptBy(commaParser))
                .AndThen(closeBracketParser);
        }
    }
}

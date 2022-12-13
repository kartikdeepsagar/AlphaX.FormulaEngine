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
            var emtpyStringResult = new StringResult(string.Empty);
            var whiteSpacesParser = Parser.WhiteSpace.Many().MapResult(x => emtpyStringResult);

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

            var stringValueParser = new StringValueParser()
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var numberParser = Parser.Number(true)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var formulaArgumentParser = numberParser
                .Or(stringValueParser)
                .Or(Parser.Lazy(() => FormulaParser));

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

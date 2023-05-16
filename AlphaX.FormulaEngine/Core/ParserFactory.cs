using AlphaX.FormulaEngine.Resources;
using AlphaX.Parserz;

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

            var stringValueParser = Parser.StringValue(settings.DoubleQuotedStrings)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var booleanParser = Parser.Boolean
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var numberParser = Parser.Number(true)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var logicalOperatorParsers = Parser.String("==")
              .Or(Parser.String("!="))
              .Or(Parser.String("<="))
              .Or(Parser.String(">="))
              .Or(Parser.String("<"))
              .Or(Parser.String(">"))
              .AndThen(whiteSpacesParser)
              .MapResult(x => x.Value[0]);

            var baseArgumentParser = stringValueParser
                .Or(numberParser)
                .Or(booleanParser)
                .Or(Parser.Lazy(() => FormulaParser));

            var formulaArgumentParser = baseArgumentParser.Next(leftOperandResult =>
                {
                    return logicalOperatorParsers.Many(0, 1)
                    .MapResult(result =>
                    {
                        if (result.Value.Length > 0)
                        {
                            return new OperatorResult(result.Value[0].Value?.ToString());
                        }
                        else
                        {
                            return leftOperandResult;
                        }
                    })
                    .Next(operatorResult =>
                    {
                        if (operatorResult.Type != FormulaParserResultType.Operator)
                        {
                            return Parser.FromResult(leftOperandResult);
                        }
                        else
                        {
                            return baseArgumentParser.MapResult(rightOperandResult =>
                                {
                                    return new ConditionResult(new Condition(
                                    
                                        leftOperandResult.Value,
                                        operatorResult.Value?.ToString(),
                                        rightOperandResult.Value
                                    ));
                                }).MapError(x => new ParserError(x.Index, "Invalid logical expression"));
                        }
                    });
                });

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
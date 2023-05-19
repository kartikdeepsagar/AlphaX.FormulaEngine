
using AlphaX.FormulaEngine.Resources;
using AlphaX.Parserz;

namespace AlphaX.FormulaEngine
{
    internal static class ParserFactory
    {
        public static IParser FormulaParser { get; private set; }
        public static IParser ExpressionParser { get; private set; }

        internal static void BuildParser(IEngineSettings settings)
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

            var customNameParser = Parser.String(Tokens.Custom)
                .AndThen(Parser.AnyLetterOrDigit().Many().MapResult(x => x.ToStringResult()))
                .AndThen(whiteSpacesParser)
                .MapResult(x => new CustomNameResult(new CustomName(x.Value[1].Value?.ToString())));

            var stringValueParser = Parser.StringValue(settings.DoubleQuotedStrings)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var booleanParser = Parser.Boolean
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var numberParser = Parser.Number(true)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var arrayCommaResult = new StringResult(Tokens.Comma);
            var arrayCommaParser = Parser.String(Tokens.Comma)
                .AndThen(whiteSpacesParser)
                .MapResult(x => arrayCommaResult);

            var arrayParser = Parser.String(Tokens.OpenSquareBracket)
                .AndThen(whiteSpacesParser)
                .AndThen(customNameParser.Or(stringValueParser).Or(numberParser).Or(booleanParser).Or(Parser.Lazy(() => FormulaParser)).ManySeptBy(arrayCommaParser))
                .AndThen(Parser.String(Tokens.ClosedSquareBracket))
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[2]);

            var logicalOperatorParsers = Parser.String(Tokens.EqualsTo)
              .Or(Parser.String(Tokens.NotEquals))
              .Or(Parser.String(Tokens.LessThanEqualsTo))
              .Or(Parser.String(Tokens.GreaterThanEqualsTo))
              .Or(Parser.String(Tokens.LessThan))
              .Or(Parser.String(Tokens.GreaterThan))
              .Or(Parser.String(Tokens.AND))
              .Or(Parser.String(Tokens.OR))
              .AndThen(whiteSpacesParser)
              .MapResult(x => x.Value[0]);

            var baseArgumentParser = arrayParser
                .Or(customNameParser)
                .Or(stringValueParser)
                .Or(numberParser)
                .Or(booleanParser)
                .Or(Parser.Lazy(() => FormulaParser));

            var formulaArgumentParser = baseArgumentParser.Next(leftOperandResult =>
                {
                    ConditionResult conditionResult = null;
                    return logicalOperatorParsers
                    .Next(operatorResult =>
                    {
                        if (operatorResult.Type != ParserResultType.String)
                        {
                            return Parser.FromResult(leftOperandResult);
                        }
                        else
                        {
                            return baseArgumentParser.MapResult(rightOperandResult =>
                            {
                                if(conditionResult == null)
                                {
                                    conditionResult = new ConditionResult(new Condition(
                                      leftOperandResult,
                                      operatorResult,
                                      rightOperandResult));
                                }
                                else
                                {
                                    conditionResult = new ConditionResult(new Condition()
                                    {
                                        LeftOperand = conditionResult,
                                        Operator = operatorResult,
                                        RightOperand = rightOperandResult
                                    });
                                }
                                return conditionResult;
                            }).MapError(x => new ParserError(x.Index, "Invalid logical expression"));
                        }
                    })
                    .Many()
                    .MapResult(x =>
                    {
                        if (x.Value.Length == 0)
                            return leftOperandResult;

                        return conditionResult;
                    });
                }).MapError(x => new ParserError(x.Index, "Invalid argument found in expression"));

            var commaResult = new FormulaArgumentSeperatorResult(settings.ArgumentsSeparatorSymbol);
            var commaParser = Parser.String(settings.ArgumentsSeparatorSymbol)
                .AndThen(whiteSpacesParser)
                .MapResult(x => commaResult);

            FormulaParser = formulaNameParser
                .AndThen(openBracketParser)
                .AndThen(formulaArgumentParser.ManySeptBy(commaParser))
                .AndThen(closeBracketParser)
                .MapError(x => new ParserError(x.Index, "Invalid formula expression"));

            ExpressionParser = formulaArgumentParser;
        }
    }
}
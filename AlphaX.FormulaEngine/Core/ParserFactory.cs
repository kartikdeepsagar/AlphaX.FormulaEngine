using AlphaX.FormulaEngine.Resources;
using AlphaX.Parserz;

namespace AlphaX.FormulaEngine
{
    internal static class ParserFactory
    {
        public static IParser FormulaParser { get; private set; }
        public static IParser NumberParser { get; private set; }
        public static IParser BooleanParser { get; private set; }
        public static IParser StringParser { get; private set; }
        public static IParser CustomNameParser { get; private set; }
        public static IParser ArrayParser { get; private set; }
        public static IParser ExpressionParser { get; private set; }
        public static IParser NullParser { get; private set; }

        public static void BuildParser(IEngineSettings settings)
        {
            var emtpyStringResult = new StringResult(string.Empty);
            var whiteSpacesParser = Parser.WhiteSpace.Many().MapResult(x => emtpyStringResult);

            NullParser = Parser.String("null").MapResult(x => new StringResult(null));

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

            CustomNameParser = Parser.String(Tokens.Custom)
                .AndThen(Parser.AnyLetterOrDigit().Many().MapResult(x => x.ToStringResult()))
                .AndThen(whiteSpacesParser)
                .MapResult(x => new CustomNameResult(new CustomName(x.Value[1].Value?.ToString())));

            StringParser = Parser.StringValue(settings.DoubleQuotedStrings)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            BooleanParser = Parser.Boolean
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            NumberParser = Parser.Number(true)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var arrayCommaResult = new StringResult(Tokens.Comma);
            var arrayCommaParser = Parser.String(Tokens.Comma)
                .AndThen(whiteSpacesParser)
                .MapResult(x => arrayCommaResult);

            ArrayParser = Parser.String(Tokens.OpenSquareBracket)
                .AndThen(whiteSpacesParser)
                .AndThen(CustomNameParser.Or(StringParser).Or(NumberParser).Or(BooleanParser).Or(Parser.Lazy(() => FormulaParser)).ManySeptBy(arrayCommaParser))
                .AndThen(Parser.String(Tokens.ClosedSquareBracket))
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[2]);

            var baseArgumentParser = GetOrderedBaseArgumentsParser(settings.ParseOrder);

            var formulaArgumentParser = baseArgumentParser
                .Next(leftOperandResult =>
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
                                if (conditionResult == null)
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
                })
                .MapError(x => new ParserError(x.Index, "Invalid argument found in expression"));

            var openBracketResult = new FormulaBracketResult(settings.OpenBracketSymbol);
            var openBracketParser = Parser.String(settings.OpenBracketSymbol)
                .AndThen(whiteSpacesParser)
                .MapResult(x => openBracketResult);

            var closeBracketResult = new FormulaBracketResult(settings.CloseBracketSymbol);
            var closeBracketParser = Parser.String(settings.CloseBracketSymbol)
                .AndThen(whiteSpacesParser)
                .MapResult(x => closeBracketResult);

            var lettersOrDigitsParser = Parser.Letter.AndThen(Parser.AnyLetterOrDigit().Many())
                .MapError(x => new ParserError(x.Index, string.Format(EngineResources.UnexpectedInput, x.Index, "formula name")))
                .MapResult(x => x.ToStringResult());

            var formulaNameParser = lettersOrDigitsParser
                .AndThen(whiteSpacesParser)
                .MapResult(x => new FormulaNameResult(x.Value[0].Value.ToString()));

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

        private static IParser GetOrderedBaseArgumentsParser(ParseOrder parseOrder)
        {
            IParser parser = null;

            foreach(ParseMode mode in parseOrder)
            {
                if(parser == null)
                {
                    parser = GetParser(mode);
                }
                else
                {
                    parser = parser.Or(GetParser(mode));
                }
            }

            return parser.Or(NullParser);
        }

        private static IParser GetParser(ParseMode mode)
        {
            switch (mode)
            {
                case ParseMode.Array : return ArrayParser;
                case ParseMode.Boolean : return BooleanParser;
                case ParseMode.String : return StringParser;
                case ParseMode.Number : return NumberParser;
                case ParseMode.CustomName : return CustomNameParser;
                default:
                    return Parser.Lazy(() => FormulaParser);
            }
        }
    }
}
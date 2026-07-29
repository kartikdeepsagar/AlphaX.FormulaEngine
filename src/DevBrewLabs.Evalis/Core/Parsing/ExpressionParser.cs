using DevBrewLabs.Parserly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevBrewLabs.Evalis.Core.Parsing
{
    internal class ExpressionParser : IParser
    {
        private IParser _formulaParser;
        private IParser _argParser;
        private IParser _numberParser;
        private IParser _boolParser;
        private IParser _stringParser;
        private IParser _customNameParser;
        private IParser _expressionParser;
        private IParser _nullParser;
        private IParser _varParser;
        private IParserResult _openBracketResult;
        private IParserResult _closeBracketResult;

        public bool AllowTrace { get; set; }

        public ExpressionParser(IEngineSettings settings, LogicalOperator @operator)
        {
            _openBracketResult = new OpenBracketResult();
            _closeBracketResult = new CloseBracketResult();

            BuildParser(settings, @operator);
        }

        private void BuildParser(IEngineSettings settings, LogicalOperator @operator)
        {
            var emptyStringResult = new StringResult(string.Empty);
            var whiteSpacesParser = Parser.WhiteSpace().Many().MapResult(x => emptyStringResult);

            _nullParser = Parser.String("null").MapResult(x => new StringResult(null));

            var operatorParser = Parser.String(ArithmeticOperator.Add)
              .Or(Parser.String(ArithmeticOperator.Subtract))
              .Or(Parser.String(ArithmeticOperator.Multiply))
              .Or(Parser.String(ArithmeticOperator.Divide))
              .Or(Parser.String(ArithmeticOperator.Modulo))
              .Or(Parser.String(@operator.EqualsTo))
              .Or(Parser.String(@operator.NotEquals))
              .Or(Parser.String(@operator.LessThanEqualsTo))
              .Or(Parser.String(@operator.GreaterThanEqualsTo))
              .Or(Parser.String(@operator.LessThan))
              .Or(Parser.String(@operator.GreaterThan))
              .Or(Parser.String(@operator.AND))
              .Or(Parser.String(@operator.OR))
              .AndThen(whiteSpacesParser)
              .MapResult(x => new OperatorResult((string)x.Value[0].Value));

            _varParser = Parser.Var(AllowTrace);

            _customNameParser = Parser.String(SyntaxTokens.Custom)
                .AndThen(_varParser)
                .AndThen(whiteSpacesParser)
                .MapResult(x => new CustomNameResult(new CustomName(x.Value[1].Value?.ToString())));

            if (settings.CustomTokenParsers != null && settings.CustomTokenParsers.Count > 0)
            {
                IParser customTokensCombinedParser = null;
                foreach (var tokenParser in settings.CustomTokenParsers)
                {
                    var mappedParser = tokenParser
                        .AndThen(whiteSpacesParser)
                        .MapResult(x => new CustomNameResult(new CustomName(x.Value[0].Value?.ToString())));

                    if (customTokensCombinedParser == null)
                        customTokensCombinedParser = mappedParser;
                    else
                        customTokensCombinedParser = customTokensCombinedParser.Or(mappedParser);
                }

                _customNameParser = customTokensCombinedParser.Or(_customNameParser);
            }

            _stringParser = Parser.StringValue(settings.DoubleQuotedStrings)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            _boolParser = Parser.Boolean()
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            _numberParser = Parser.Number(true)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var commaParser = Parser.String(SyntaxTokens.Comma)
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            var openBracketParser = Parser.String(SyntaxTokens.OpenBracket)
                .AndThen(whiteSpacesParser)
                .MapResult(x => _openBracketResult);

            var closeBracketParser = Parser.String(SyntaxTokens.ClosedBracket)
                .AndThen(whiteSpacesParser)
                .MapResult(x => _closeBracketResult);

            var baseArgParser = CreateParserFromParseOrder(settings.EngineParseOrder)
                .Or(openBracketParser.AndThen(Parser.Lazy(() => _argParser)).AndThen(closeBracketParser))
                .MapError(x => new ParserError(x.Index, "Invalid formula argument"));

            var peekParser = Parser.Peek(SyntaxTokens.Comma);

            // --- FIX: the array of accumulated (operand, operator, operand, ...) results
            // is now built exactly once, after the Many() loop completes, instead of being
            // rebuilt (ToArray + new ArrayResult) on every single iteration of the loop.
            // For an expression with n chained operators this changes an O(n^2) allocation
            // pattern with n-1 wasted ArrayResult objects into a single O(n) allocation.
            _argParser = baseArgParser
                .Next(leftOperandResult =>
                {
                    List<IParserResult> resultsList = null;
                    return peekParser.MapResult(x => leftOperandResult)
                    .Or(
                        operatorParser
                        .Next(operatorResult =>
                        {
                            return baseArgParser.MapResult(rightOperandResult =>
                            {
                                if (resultsList == null)
                                {
                                    // small initial capacity to cut down on List<T> resizes
                                    resultsList = new List<IParserResult>(8)
                                    {
                                    leftOperandResult,
                                    operatorResult,
                                    rightOperandResult
                                    };
                                }
                                else
                                {
                                    resultsList.Add(operatorResult);
                                    resultsList.Add(rightOperandResult);
                                }

                                // Nothing downstream consumes this per-iteration value directly;
                                // the real combined result is assembled once in the Many().MapResult
                                // below. Returning rightOperandResult avoids allocating a throwaway
                                // ArrayResult on every iteration.
                                return rightOperandResult;
                            }).MapError(x => new ParserError(x.Index, "Invalid logical expression"));
                        })
                        .Many()
                        .MapResult(x =>
                        {
                            if (x.Value.Length == 0)
                                return leftOperandResult;

                            return (IParserResult)new ArrayResult(resultsList.ToArray());
                        })
                    );
                })
                .MapError(x => new ParserError(x.Index, "Invalid argument found in expression"));

            var formulaNameParser = _varParser
                .AndThen(whiteSpacesParser)
                .MapResult(x => x.Value[0]);

            _formulaParser = formulaNameParser
                .AndThen(openBracketParser)
                .AndThen(_argParser.ManySeptBy(commaParser))
                .AndThen(closeBracketParser)
                .MapResult(x => new FormulaResult(new FormulaExpr((string)x.Value[0].Value, (IParserResult[])x.Value[2].Value)))
                .MapError(x => new ParserError(x.Index, $"Invalid formula expression. Reason: {x.Message}"));

            _expressionParser = _argParser
                .Many();
        }

        private IParser CreateParserFromParseOrder(IParseOrder parseOrder, params ParseType[] parseTypesToSkip)
        {
            IParser parser = null;

            foreach (ParseType type in parseOrder)
            {
                if (parseTypesToSkip != null && parseTypesToSkip.Contains(type))
                {
                    continue;
                }

                parser = parser == null ? GetParser(type) : parser.Or(GetParser(type));
            }

            // FIX: guard against parseOrder being empty (or fully skipped), which
            // previously threw a NullReferenceException on parser.Or(_nullParser).
            return parser == null ? _nullParser : parser.Or(_nullParser);
        }

        private IParser GetParser(ParseType mode)
        {
            switch (mode)
            {
                case ParseType.Boolean: return _boolParser;
                case ParseType.String: return _stringParser;
                case ParseType.Number: return _numberParser;
                case ParseType.CustomName: return _customNameParser;
                case ParseType.Formula: return Parser.Lazy(() => _formulaParser);
                default:
                    throw new ArgumentException("Invalid parse type");
            }
        }

        public IParserState Run(string input)
        {
            return _expressionParser.Run(input);
        }

        public IParserState Parse(IParserState inputState)
        {
            return _expressionParser.Parse(inputState);
        }
    }
}
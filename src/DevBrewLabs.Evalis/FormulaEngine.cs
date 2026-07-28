using DevBrewLabs.Evalis.Core.Parsing;
using DevBrewLabs.Evalis.Formulas;
using DevBrewLabs.Parserly;
using DevBrewLabs.Parserly.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevBrewLabs.Evalis
{
    /// <summary>
    /// The main entry point of the Evalis Formula Engine. Parses, evaluates, and manages formulas and engine settings.
    /// </summary>
    public class FormulaEngine : IFormulaEngine
    {
        private IParser _expressionParser;
        private readonly object _settingsLock = new object();

        #region Internal
        internal Evaluator Evaluator {  get; private set; }
        internal IEngineSettings CurrentSettings { get; private set; }
        #endregion

        public IEngineContext Context { get; set; }
        public IFormulaStore FormulaStore { get; }

        /// <summary>
        /// Initializes a new FormulaEngine.
        /// </summary>
        /// <param name="context">Optional engine context for resolving variable/token values.</param>
        /// <param name="loadDefaultFormulas">When true, all built-in formulas (arithmetic, string, array, datetime, logical) are registered automatically.</param>
        public FormulaEngine(IEngineContext context = null, bool loadDefaultFormulas = true)
        {
            Context = context;
            FormulaStore = new FormulaStore(this);
            Evaluator = new Evaluator(FormulaStore);
            ApplySettings(new EngineSettings());

            if (loadDefaultFormulas)
            {
                LoadDefaultFormulas();
            }
        }

        /// <summary>
        /// Evaluates a formula expression string synchronously.
        /// </summary>
        /// <param name="input">The formula expression string to evaluate.</param>
        /// <returns>An IEvaluationResult containing the result or error.</returns>
        public IEvaluationResult Evaluate(string input)
        {
            return EvaluateInternal(input, Context)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Evaluates a sequenced expression synchronously.
        /// </summary>
        /// <param name="input">The sequenced expression to evaluate.</param>
        /// <returns>An IEvaluationResult for the last evaluated segment.</returns>
        public IEvaluationResult Evaluate(ISequencedExpression input)
        {
            IEvaluationResult result = null;
            SequencedExpression expr = input as SequencedExpression;

            foreach (SequencedExpressionSegment expressionSegment in expr)
            {
                result = EvaluateInternal(expressionSegment.Expression, expr.Context)
                        .GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(result.Error))
                {
                    return result;
                }

                expressionSegment.Result = result.Value;
            }

            expr.Dispose();
            return result;
        }

        /// <summary>
        /// Evaluates a formula expression string asynchronously.
        /// </summary>
        /// <param name="input">The formula expression string to evaluate.</param>
        /// <returns>A task containing the IEvaluationResult with the result or error.</returns>
        public Task<IEvaluationResult> EvaluateAsync(string input)
        {
            return EvaluateInternal(input, Context); ;
        }

        /// <summary>
        /// Evaluates a sequenced expression asynchronously.
        /// </summary>
        /// <param name="input">The sequenced expression to evaluate.</param>
        /// <returns>A task containing the IEvaluationResult for the last evaluated segment.</returns>
        public async Task<IEvaluationResult> EvaluateAsync(ISequencedExpression input)
        {
            IEvaluationResult result = null;
            SequencedExpression expr = input as SequencedExpression;

            foreach (SequencedExpressionSegment expressionSegment in expr)
            {
                result = await EvaluateInternal(expressionSegment.Expression, expr.Context);

                if (!string.IsNullOrEmpty(result.Error))
                {
                    return result;
                }

                expressionSegment.Result = result.Value;
            }

            expr.Dispose();
            return result;
        }

        private async Task<IEvaluationResult> EvaluateInternal(string input, IEngineContext context)
        {
            try
            {
                if (input == null)
                {
                    return EvaluationResult.WithError(Error.General("Input can't be null"));
                }

                var parserState = _expressionParser.Run(input);

                if (parserState.IsError)
                    return EvaluationResult.WithError(Error.Syntax(parserState.Error.Message));

                object result = await Evaluator.Evaluate(parserState.Result, context);
                if (result is IEvaluationResult evalResult)
                    return evalResult;
                return EvaluationResult.WithValue(result);
            }
            catch (Exception ex)
            {
                ex = UnwrapException(ex);
                return EvaluationResult.WithError(Error.General(ex.Message));
            }
        }

        public IParserState Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return _expressionParser.Run(input);
        }

        public string[] ExtractVariables(string input)
        {
            var parserState = Parse(input);
            if (parserState.IsError)
            {
                return Array.Empty<string>();
            }

            var variables = new HashSet<string>();
            ExtractVariablesRecursive(parserState.Result, variables);
            return variables.ToArray();
        }

        private void ExtractVariablesRecursive(IParserResult result, HashSet<string> variables)
        {
            if (result == null) return;

            if (result is CustomNameResult customNameResult)
            {
                variables.Add(customNameResult.Value.Value);
            }
            else if (result is FormulaResult formulaResult)
            {
                if (formulaResult.Value.Args != null)
                {
                    foreach (var arg in formulaResult.Value.Args)
                    {
                        ExtractVariablesRecursive(arg, variables);
                    }
                }
            }
            else if (result is ArrayResult arrayResult)
            {
                if (arrayResult.Value != null)
                {
                    foreach (var item in arrayResult.Value)
                    {
                        ExtractVariablesRecursive(item, variables);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the specified settings to the engine, rebuilding the parser and operator configuration.
        /// </summary>
        /// <param name="settings">The settings to apply. Must contain a valid EngineParseOrder.</param>
        public void ApplySettings(IEngineSettings settings)
        {
            if (settings.EngineParseOrder is null || !settings.EngineParseOrder.Any())
                throw new InvalidOperationException("Invalid engine parse order");

            lock (_settingsLock)
            {
                Evaluator.SupportedLogicalOperators = new LogicalOperator(settings.LogicalOperatorMode);
                _expressionParser = new ExpressionParser(settings, Evaluator.SupportedLogicalOperators);
                CurrentSettings = settings;
            }
        }

        private void LoadDefaultFormulas()
        {
            FormulaStore.Add(new OperatorFormula("EQUALS", () => Evaluator.SupportedLogicalOperators.EqualsTo));
            FormulaStore.Add(new OperatorFormula("NOTEQUALS", () => Evaluator.SupportedLogicalOperators.NotEquals));
            FormulaStore.Add(new OperatorFormula("OR", () => Evaluator.SupportedLogicalOperators.OR));
            FormulaStore.Add(new OperatorFormula("AND", () => Evaluator.SupportedLogicalOperators.AND));
            FormulaStore.Add(new OperatorFormula("GREATERTHAN", () => Evaluator.SupportedLogicalOperators.GreaterThan));
            FormulaStore.Add(new OperatorFormula("GREATERTHANEQUALS", () => Evaluator.SupportedLogicalOperators.GreaterThanEqualsTo));
            FormulaStore.Add(new OperatorFormula("LESSTHAN", () => Evaluator.SupportedLogicalOperators.LessThan));
            FormulaStore.Add(new OperatorFormula("LESSTHANEQUALS", () => Evaluator.SupportedLogicalOperators.LessThanEqualsTo));
            FormulaStore.Add(new NotFormula());

            // Arithmetic
            FormulaStore.Add(new SumFormula());
            FormulaStore.Add(new AverageFormula());
            FormulaStore.Add(new CeilingFormula());
            FormulaStore.Add(new FloorFormula());
            FormulaStore.Add(new AbsFormula());
            FormulaStore.Add(new MinFormula());
            FormulaStore.Add(new MaxFormula());
            FormulaStore.Add(new PowerFormula());
            FormulaStore.Add(new RoundFormula());
            FormulaStore.Add(new SqrtFormula());
            FormulaStore.Add(new ModFormula());
            FormulaStore.Add(new TruncFormula());
            FormulaStore.Add(new SignFormula());
            FormulaStore.Add(new LogFormula());
            FormulaStore.Add(new Log10Formula());
            FormulaStore.Add(new ExpFormula());
            FormulaStore.Add(new PiFormula());

            // Array
            FormulaStore.Add(new ArrayContainsFormula());
            FormulaStore.Add(new ArrayIncludesFormula());
            FormulaStore.Add(new ArrayFormula());
            FormulaStore.Add(new IndexFormula());
            FormulaStore.Add(new JoinFormula());
            FormulaStore.Add(new CountFormula());

            // String
            FormulaStore.Add(new LowerFormula());
            FormulaStore.Add(new UpperFormula());
            FormulaStore.Add(new TextSplitFormula());
            FormulaStore.Add(new ConcatFormula());
            FormulaStore.Add(new LengthFormula());
            FormulaStore.Add(new ContainsFormula());
            FormulaStore.Add(new StartsWithFormula());
            FormulaStore.Add(new EndsWithFormula());
            FormulaStore.Add(new RegexMatchFormula());
            FormulaStore.Add(new ReplaceFormula());
            FormulaStore.Add(new TrimFormula());
            FormulaStore.Add(new SubstringFormula());
            FormulaStore.Add(new IndexOfFormula());
            FormulaStore.Add(new LeftFormula());
            FormulaStore.Add(new RightFormula());
            FormulaStore.Add(new MidFormula());
            FormulaStore.Add(new PadFormula());
            FormulaStore.Add(new RepeatFormula());
            FormulaStore.Add(new ProperFormula());
            FormulaStore.Add(new IsEmptyFormula());
            FormulaStore.Add(new IsNullOrEmptyFormula());
            FormulaStore.Add(new FormatFormula());

            // DateTime
            FormulaStore.Add(new TodayFormula());
            FormulaStore.Add(new NowFormula());
            FormulaStore.Add(new DateTimeFormula());
            FormulaStore.Add(new YearFormula());
            FormulaStore.Add(new MonthFormula());
            FormulaStore.Add(new DayFormula());

            // Logical
            FormulaStore.Add(new IfFormula());
            FormulaStore.Add(new CoalesceFormula());
            FormulaStore.Add(new IsNumberFormula());
            FormulaStore.Add(new IsStringFormula());
            FormulaStore.Add(new IfsFormula());
            FormulaStore.Add(new SwitchFormula());
            FormulaStore.Add(new IfErrorFormula());
            FormulaStore.Add(new IfBlankFormula());
            FormulaStore.Add(new IsBoolFormula());
            FormulaStore.Add(new IsDateFormula());
            FormulaStore.Add(new IsArrayFormula());
            FormulaStore.Add(new IsNullFormula());
        }

        private Exception UnwrapException(Exception ex)
        {
            while (true)
            {
                switch (ex)
                {
                    case AggregateException agg when agg.InnerExceptions.Count == 1:
                        ex = agg.InnerExceptions[0];
                        continue;

                    case AggregateException agg when agg.InnerExceptions.Count > 1:
                        return agg.Flatten();

                    default:
                        if (ex.InnerException != null &&
                            (ex is TargetInvocationException || ex.GetType().Name == "EvaluationWrapperException"))
                        {
                            ex = ex.InnerException;
                            continue;
                        }
                        return ex;
                }
            }
        }
    }
}

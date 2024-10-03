using AlphaX.FormulaEngine.Formulas;
using AlphaX.Parserz;
using System;
using System.Linq;

namespace AlphaX.FormulaEngine
{
    public class AlphaXFormulaEngine : IFormulaEngine
    {
        private IParser _expressionParser;

        #region Internal
        internal Evaluator Evaluator {  get; private set; }
        internal Operator Operator { get; set; }
        #endregion

        public IEngineContext Context { get; set; }
        public IFormulaStore FormulaStore { get; }

        public AlphaXFormulaEngine(IEngineContext context = null, bool loadDefaultFormulas = true)
        {
            Context = context;
            ApplySettings(new EngineSettings());
            FormulaStore = new FormulaStore(this);
            Evaluator = new Evaluator(this);

            if (loadDefaultFormulas)
            {
                LoadDefaultFormulas();
            }
        }

        public IEvaluationResult Evaluate(string input)
        {
            try
            {
                if(input == null)
                {
                    throw new Exception("Input can't be null");
                }

                var parserState = _expressionParser.Run(input);

                if (parserState.IsError)
                    return new EvaluationResult(parserState.Error.Message);

                object result = Evaluator.Evaluate(parserState.Result);
                return new EvaluationResult(result);
            }
            catch(EvaluationException ex)
            {
                return new EvaluationResult(ex.Message);
            }
            catch(Exception ex)
            {
                return new EvaluationResult(ex.Message);
            }
        }

        public void ApplySettings(IEngineSettings settings)
        {
            if (settings.OpenBracketSymbol is null)
                throw new ArgumentNullException("Open bracket symbol cannot be null");

            if (settings.CloseBracketSymbol is null)
                throw new ArgumentNullException("Close bracket symbol cannot be null");

            if (settings.ArgumentsSeparatorSymbol is null)
                throw new ArgumentNullException("Argument separator symbol cannot be null");

            if (settings.EngineParseOrder is null || !settings.EngineParseOrder.Any())
                throw new InvalidOperationException("Invalid engine parse order");

            if (settings.ArrayParseOrder is null || !settings.ArrayParseOrder.Any())
                throw new InvalidOperationException("Invalid array parse order");

            Operator = new Operator(settings.LogicalOperatorMode);
            _expressionParser = new ExpressionParser(settings, Operator);
        }

        private void LoadDefaultFormulas()
        {
            FormulaStore.Add(new OperatorFormula("EQUALS", () => Operator.EqualsTo));
            FormulaStore.Add(new OperatorFormula("NOTEQUALS", () => Operator.NotEquals));
            FormulaStore.Add(new OperatorFormula("OR", () => Operator.OR));
            FormulaStore.Add(new OperatorFormula("AND", () => Operator.AND));
            FormulaStore.Add(new OperatorFormula("GREATERTHAN", () => Operator.GreaterThan));
            FormulaStore.Add(new OperatorFormula("GREATERTHANEQUALS", () => Operator.GreaterThanEqualsTo));
            FormulaStore.Add(new OperatorFormula("LESSTHAN", () => Operator.LessThan));
            FormulaStore.Add(new OperatorFormula("LESSTHANEQUALS", () => Operator.LessThanEqualsTo));
            FormulaStore.Add(new NotFormula());

            // Arithmetic
            FormulaStore.Add(new SumFormula());
            FormulaStore.Add(new AverageFormula());
            FormulaStore.Add(new CeilingFormula());
            FormulaStore.Add(new FloorFormula());

            // Array
            FormulaStore.Add(new ArrayContainsFormula());
            FormulaStore.Add(new ArrayIncludesFormula());

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

            // DateTime
            FormulaStore.Add(new TodayFormula());
            FormulaStore.Add(new NowFormula());

            // Logical
            FormulaStore.Add(new IFFormula());
        }
    }
}

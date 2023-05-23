using AlphaX.FormulaEngine.Formulas;
using AlphaX.Parserz;
using System;

namespace AlphaX.FormulaEngine
{
    public class AlphaXFormulaEngine : IFormulaEngine
    {
        private Evaluator _evaluator;

        public IEngineContext Context { get; set; }
        public IEngineSettings Settings { get; }
        public IFormulaStore FormulaStore { get; }

        public AlphaXFormulaEngine(IEngineContext context = null)
        {
            Context = context;
            _evaluator = new Evaluator(this);
            Settings = new FormulaEngineSettings();
            FormulaStore = new FormulaStore();
            LoadDefaultFormulas();
        }

        public IEvaluationResult Evaluate(string input)
        {
            try
            {
                if(input == null)
                {
                    throw new Exception("Input can't be null");
                }

                var parserState = ParserFactory.ExpressionParser.Run(input);

                if (parserState.IsError)
                    return new EvaluationResult(parserState.Error.Message);

                object result = _evaluator.Evaluate(parserState.Result);
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

        private void LoadDefaultFormulas()
        {
            // Operators
            FormulaStore.Add(new EqualsFormula());
            FormulaStore.Add(new LessThanFormula());
            FormulaStore.Add(new LessThanEqualsFormula());
            FormulaStore.Add(new GreaterThanFormula());
            FormulaStore.Add(new GreaterThanEqualsFormula());
            FormulaStore.Add(new AndFormula());
            FormulaStore.Add(new OrFormula());
            FormulaStore.Add(new NotFormula());

            // Arithmetic
            FormulaStore.Add(new SumFormula());
            FormulaStore.Add(new AverageFormula());

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

            // DateTime
            FormulaStore.Add(new TodayFormula());
            FormulaStore.Add(new NowFormula());

            // Logical
            FormulaStore.Add(new IFFormula());
        }
    }
}

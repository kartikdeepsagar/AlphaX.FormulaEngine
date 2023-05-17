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
                var parserState = ParserFactory.FormulaParser.Run(input);

                if (parserState.IsError)
                    return new EvaluationResult(parserState.Error.Message);

                var result = _evaluator.Evaluate(parserState.Result as ArrayResult);
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
            FormulaStore.Add(new SumFormula());
            FormulaStore.Add(new AverageFormula());
            FormulaStore.Add(new LowerFormula());
            FormulaStore.Add(new UpperFormula());
            FormulaStore.Add(new TextSplitFormula());
            FormulaStore.Add(new TodayFormula());
            FormulaStore.Add(new ConcatFormula());
            FormulaStore.Add(new NowFormula());
            FormulaStore.Add(new IFFormula());
        }
    }
}

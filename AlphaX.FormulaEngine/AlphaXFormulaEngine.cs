using AlphaX.FormulaEngine.Formulas;
using AlphaX.Parserz;
using AlphaX.Parserz.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlphaX.FormulaEngine
{
    public class AlphaXFormulaEngine : IFormulaEngine
    {
        private Dictionary<string, Formula> _formulas;

        public FormulaEngineSettings Settings { get; }

        public AlphaXFormulaEngine()
        {
            Settings = FormulaEngineSettings.Default;
            Settings.Update();
            _formulas = new Dictionary<string, Formula>();
            AddFormula(new SumFormula());
            AddFormula(new AverageFormula());
            AddFormula(new LowerFormula());
            AddFormula(new UpperFormula());
            AddFormula(new TextSplitFormula());
        }

        public IEvaluationResult Evaluate(string input)
        {
            try
            {
                var parserState = ParserFactory.FormulaParser.Run(input);

                if (parserState.IsError)
                    return new EvaluationResult(parserState.Error.Message);

                var result = Evaluate(parserState.Result as ArrayResult);
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

        /// <summary>
        /// Evaluates the result from AST.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="EvaluationException"></exception>
        private object Evaluate(ArrayResult result)
        {
            List<object> arguments = new List<object>();

            Formula formula = null;
            for(int index = 0; index < result.Value.Length; index++)
            {
                var item = result.Value[index];

                if (item.Type == FormulaParserResultType.FormulaName)
                {
                    var formulaName = item.Value.ToString();
                    if (!_formulas.ContainsKey(formulaName))
                        throw new EvaluationException($"Invalid formula '{formulaName}'");

                    formula = _formulas[formulaName];
                }
                
                if (item.Type == ParserResultType.Array)
                {
                    var argResult = Evaluate(item as ArrayResult);
                    if(argResult is IEnumerable<object> enumerable)
                        arguments.AddRange(enumerable);
                    else
                        arguments.Add(argResult);
                }
                else if(item.Type == ParserResultType.Number || item.Type == ParserResultType.String)
                {
                    arguments.Add(item.Value);
                }             
            }

            if (formula != null && (arguments.Count > formula.MaxArgsCount || arguments.Count < formula.MinArgsCount))
                throw new EvaluationException($"Invalid number of arguments for '{formula.Name}' formula");

            if (formula == null)
                return arguments;

            return formula.Evaluate(arguments.ToArray());
        }

        public void AddFormula(Formula formula)
        {
            _formulas.Add(formula.Name, formula);
        }

        public IEnumerable<FormulaInfo> GetFormulas()
        {
            return _formulas.Values.Select(x => x.Info);
        }

        public void RemoveFormula(string formulaName)
        {
            if(_formulas.ContainsKey(formulaName))
                _formulas.Remove(formulaName);

            throw new AlphaXFormulaEngineException($"Invalid formula '{formulaName}'");
        }

        public void RemoveFormula(Formula formula)
        {
            RemoveFormula(formula.Name);
        }
    }
}

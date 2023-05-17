using AlphaX.Parserz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AlphaX.FormulaEngine
{
    public class AlphaXFormulaEngine : IFormulaEngine
    {
        private IEngineContext _context;
        private Dictionary<string, Formula> _formulas;

        public FormulaEngineSettings Settings { get; }

        public AlphaXFormulaEngine(IEngineContext context = null)
        {
            _context = context;
            Settings = FormulaEngineSettings.Default;
            Settings.Update();
            _formulas = new Dictionary<string, Formula>();
            DefaultFormulas.Load(this);        
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
                    arguments.Add(argResult);
                }
                else if(item.Type == FormulaParserResultType.CustomName)
                {
                    arguments.Add(item.Value);
                }
                else if(item.Type == ParserResultType.Number || item.Type == ParserResultType.String || item.Type == FormulaParserResultType.Operator
                    || item.Type == ParserResultType.Boolean)
                {
                    arguments.Add(item.Value);
                }
                else if(item.Type == FormulaParserResultType.Condition)
                {
                    var condition = (Condition)item.Value;

                    if(condition.LeftOperand is IParserResult[] lArray)
                    {
                        condition.LeftOperand = Evaluate(new ArrayResult(lArray));
                    }

                    if (condition.RightOperand is IParserResult[] rArray)
                    {
                        condition.RightOperand = Evaluate(new ArrayResult(rArray));
                    }

                    arguments.Add(condition);
                }
            }

            ValidateAndResolveArguments(formula, arguments.ToArray());

            if (formula == null)
                return arguments.ToArray();

            return formula.Evaluate((object[])arguments[0]);
        }

        private void ValidateAndResolveArguments(Formula formula, object[] arguments)
        {
            if (formula == null)
                return;

            arguments = (object[])arguments[0];
            if (arguments.Length > formula.Info.MaxArgsCount || arguments.Length < formula.Info.MinArgsCount)
            {         
                throw new EvaluationException($"Invalid number of arguments for '{formula.Name}' formula");
            }
            else
            {
                for(int index = 0; index < formula.Info.MaxArgsCount; index++)
                {
                    var argumentData = formula.Info.Arguments[index];

                    if (index > arguments.Length - 1)
                        break;

                    var argument = arguments[index];

                    if (argument is CustomName cName)
                    {
                        if(formula.RequireContext)
                        {
                            if (_context == null)
                            {
                                throw new EvaluationException("Formula require context but no context found.");
                            }

                            var resolvedValue = _context.Resolve(cName.Value);

                            if (resolvedValue != null && resolvedValue.GetType() != argumentData.Type)
                            {
                                throw new EvaluationException($"Argument: {argumentData.Name}, Argument type doesn't match for '{formula.Name}' formula");
                            }
                            else
                            {
                                arguments[index] = resolvedValue;
                            }
                        }
                        else
                        {
                            arguments[index] = cName.Value;
                        }
                    }
                    else
                    {
                        var isArray = argumentData.Type.IsArray;

                        if (isArray && (argument.GetType() != typeof(object[])))
                        {
                            throw new EvaluationException($"Argument: {argumentData.Name}, Argument type doesn't match for '{formula.Name}' formula");
                        }

                        if (!isArray && argumentData.Type != typeof(object) && (argumentData.Type != argument.GetType()))
                        {
                            throw new EvaluationException($"Argument: {argumentData.Name}, Argument type doesn't match for '{formula.Name}' formula");
                        }
                    }
                }
            }
        }

        public void AddFormula(Formula formula)
        {
            _formulas.Add(formula.Name, formula);
        }

        public IEnumerable<Formula> GetFormulas()
        {
            return _formulas.Values;
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

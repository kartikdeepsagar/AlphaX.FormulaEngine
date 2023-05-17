using AlphaX.Parserz;
using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    internal class Evaluator
    {
        private IFormulaEngine _engine;

        public Evaluator(IFormulaEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Evaluates the result from AST.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public object Evaluate(ArrayResult nodes)
        {
            List<object> arguments = new List<object>();

            Formula formula = null;

            for (int index = 0; index < nodes.Value.Length; index++)
            {
                var item = nodes.Value[index];

                if (item.Type == FormulaParserResultType.FormulaName)
                {
                    var formulaName = item.Value.ToString();
                    if (!_engine.FormulaStore.Contains(formulaName))
                        throw new EvaluationException($"Invalid formula '{formulaName}'");

                    formula = _engine.FormulaStore.Get(formulaName);
                }
                else if (item.Type == ParserResultType.Array)
                {
                    var argResult = Evaluate(item as ArrayResult);
                    arguments.Add(argResult);
                }
                else if (item.Type == ParserResultType.Number || 
                    item.Type == ParserResultType.String || 
                    item.Type == FormulaParserResultType.Operator || 
                    item.Type == ParserResultType.Boolean ||
                    item.Type == FormulaParserResultType.CustomName)
                {
                    arguments.Add(item.Value);
                }
                else if (item.Type == FormulaParserResultType.Condition)
                {
                    var condition = (Condition)item.Value;

                    if (condition.LeftOperand is IParserResult[] lArray)
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

            if (formula == null)
                return arguments.ToArray();

            ValidateAndResolveArguments(formula, arguments.ToArray());

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
                for (int index = 0; index < formula.Info.MaxArgsCount; index++)
                {
                    var argumentData = formula.Info.Arguments[index];

                    if (index > arguments.Length - 1)
                        break;

                    var argument = arguments[index];

                    if (argument is CustomName cName)
                    {
                        if (formula.RequireContext)
                        {
                            if (_engine.Context == null)
                            {
                                throw new EvaluationException("Formula require context but no context found.");
                            }

                            var resolvedValue = _engine.Context.Resolve(cName.Value);

                            if (resolvedValue != null && resolvedValue.GetType() != argumentData.Type)
                            {
                                throw new EvaluationException($"Argument ({argumentData.Name}) type doesn't match with '{formula.Name}' formula");
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
                            throw new EvaluationException($"Argument ({argumentData.Name}) type doesn't match with '{formula.Name}' formula");
                        }

                        if (!isArray && argumentData.Type != typeof(object) && (argumentData.Type != argument.GetType()))
                        {
                            throw new EvaluationException($"Argument ({argumentData.Name}) type doesn't match with '{formula.Name}' formula");
                        }
                    }
                }
            }
        }
    }
}

using AlphaX.FormulaEngine.Formulas;
using AlphaX.Parserz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

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

                    arguments.Add(Compare(condition));
                }
            }

            if (formula == null)
                return arguments.ToArray();

            var parsedArguments = (object[])arguments[0];
            ValidateAndResolveArguments(formula, parsedArguments);
            return formula.Evaluate(parsedArguments);
        }

        private void ValidateAndResolveArguments(Formula formula, object[] arguments)
        {
            if (formula == null)
                return;

            ValidateArgumentCount(formula.Info.MinArgsCount, formula.Info.MaxArgsCount, formula.Name, arguments);

            for (int index = 0; index < formula.Info.MaxArgsCount; index++)
            {
                var argument = formula.Info.Arguments[index];

                if (index > arguments.Length - 1)
                    break;

                var argumentValue = arguments[index];

                if (argumentValue is CustomName cName)
                {
                    arguments[index] = ValidateAndResolveCustomArgument(formula, argument, cName);
                }
                else if (argument.Type.IsArray)
                {
                    ValidateArrayArgument(formula.Name, argument, argumentValue);
                }
                else
                {
                    ValidateNonArrayArgument(formula.Name, argument, argumentValue);
                }
            }
        }

        private void ValidateArgumentCount(int minArgs, int maxArgs, string formulaName, object[] arguments)
        {
            if (arguments.Length > maxArgs || arguments.Length < minArgs)
            {
                throw new EvaluationException($"Invalid number of arguments for '{formulaName}' formula");
            }
        }

        private object ValidateAndResolveCustomArgument(Formula formula, FormulaArgument argument, CustomName customName)
        {
            if (formula.RequireContext)
            {
                if (_engine.Context == null)
                {
                    throw new EvaluationException("Formula require context but no context found.");
                }

                var resolvedValue = _engine.Context.Resolve(customName.Value);
                ValidateNonArrayArgument(formula.Name, argument, resolvedValue);
                return resolvedValue;
            }
            else
            {
                return customName.Value;
            }
        }

        private void ValidateArrayArgument(string formulaName, FormulaArgument argument, object argumentValue)
        {
            if (argumentValue == null)
            {
                return;
            }

            if (argumentValue.GetType() != argument.Type)
            {
                throw new EvaluationException($"Argument ({argument.Name}) type doesn't match with '{formulaName}' formula. Expected array type.");
            }
        }

        private void ValidateNonArrayArgument(string formulaName, FormulaArgument argument, object argumentValue)
        {
            if (argumentValue == null)
            {
                return;
            }

            if (argument.Type == typeof(object))
            {
                if(!MatchesSupportedTypes(argumentValue.GetType()))
                {
                    throw new EvaluationException($"Argument ({argument.Name}) type doesn't match with '{formulaName}' formula");
                }
            }
            else if (argument.Type != argumentValue.GetType())
            {
                throw new EvaluationException($"Argument ({argument.Name}) type doesn't match with '{formulaName}' formula");
            }
        }

        private bool MatchesSupportedTypes(Type type)
        {
            return type == typeof(string) || type == typeof(double) || type == typeof(bool);
        }

        private bool Compare(Condition condition)
        {
            double.TryParse(condition.LeftOperand?.ToString(), out double operand1);
            double.TryParse(condition.RightOperand?.ToString(), out double operand2);

            switch (condition.Operator)
            {
                case "==":
                    return Comparer.Equals(condition.LeftOperand, condition.RightOperand);

                case "!=":
                    return !Comparer.Equals(condition.LeftOperand, condition.RightOperand); ;

                case "<":
                    return operand1 < operand2;

                case ">":
                    return operand1 > operand2;

                case "<=":
                    return operand1 <= operand2;

                case ">=":
                    return operand1 >= operand2;

                default:
                    return false;
            }
        }
    }
}

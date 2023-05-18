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
        /// <param name="result"></param>
        /// <returns></returns>
        public object Evaluate(IParserResult result)
        {
            if(result is ArrayResult nodes)
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
                        continue;
                    }
                    
                    if (item.Type == ParserResultType.Array 
                        || item.Type == FormulaParserResultType.CustomName 
                        || item.Type == FormulaParserResultType.Condition)
                    {
                        arguments.Add(Evaluate(item));
                    }
                    else if (item.Type == ParserResultType.Number ||
                        item.Type == ParserResultType.String ||
                        item.Type == ParserResultType.Boolean)
                    {
                        arguments.Add(item.Value);
                    }
                }

                if (formula == null)
                    return arguments.ToArray();

                var parsedArguments = (object[])arguments[0];
                ValidateArguments(formula, parsedArguments);
                return formula.Evaluate(parsedArguments);
            }
            else if (result is ConditionResult conditionResult)
            {
                return ResolveCondition(conditionResult.Value);
            }
            else if (result is CustomNameResult customNameResult)
            {
                return ResolveCustomName(customNameResult.Value);
            }
            else
            {
                return result.Value;
            }
        }

        #region Argument Validation
        private void ValidateArguments(Formula formula, object[] arguments)
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

                if (argument.Type.IsArray)
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
            return type == typeof(string) || type == typeof(double) || type == typeof(int) || type == typeof(bool);
        }
        #endregion

        #region Argument resolving
        private bool ResolveCondition(Condition condition)
        {
            var left = Evaluate(condition.LeftOperand);
            var @operator = Evaluate(condition.Operator);
            var right = Evaluate(condition.RightOperand);
            return Compare(left, @operator?.ToString(), right);
        }

        public object ResolveCustomName(CustomName customName)
        {
            if (_engine.Context == null)
            {
                throw new EvaluationException("Formula require context but no context found.");
            }

            var resolvedValue = _engine.Context.Resolve(customName.Value);

            if(resolvedValue is int)
            {
                resolvedValue = Convert.ToDouble(resolvedValue);
            }

            return resolvedValue;
        }
        #endregion

        private bool Compare(object left, string @operator, object right)
        {
            try
            {
                if (@operator == "==")
                    return Comparer.Equals(left, right);

                if (@operator == "!=")
                    return !Comparer.Equals(left, right);

                if (left is bool bool1 && right is bool bool2)
                {
                    switch (@operator)
                    {
                        case "&&":
                            return bool1 && bool2;

                        case "||":
                            return bool1 || bool2;
                    }
                }

                if (left is double num1 && right is double num2)
                {
                    switch (@operator)
                    {
                        case "<":
                            return num1 < num2;

                        case ">":
                            return num1 > num2;

                        case "<=":
                            return num1 <= num2;

                        case ">=":
                            return num1 >= num2;
                    }
                }

                return false;
            }
            catch
            {
                throw new EvaluationException($"Invalid operator used with operands. {left} {@operator} {right}");
            }
        }
    }
}

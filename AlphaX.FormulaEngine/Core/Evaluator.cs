using AlphaX.Parserz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

                if(argumentValue == null)
                {
                    if(argument.AllowNull)
                    {
                        continue;
                    }
                    else
                    {
                        throw new EvaluationException($"Null is not allowed as argument ({argument.Name}) value.");
                    }
                }

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
                throw new EvaluationException($"Invalid number of arguments for '{formulaName}' formula. Expected Min = {minArgs}, Max = {maxArgs} arguments.");
            }
        }

        private void ValidateArrayArgument(string formulaName, FormulaArgument argument, object argumentValue)
        {
            if (argumentValue.GetType() != argument.Type)
            {
                throw new EvaluationException($"{GetTypeMismatchError(formulaName, argument)}. Expected array type.");
            }
        }

        private void ValidateNonArrayArgument(string formulaName, FormulaArgument argument, object argumentValue)
        {
            if (argument is ObjectArgument)
            {
                if(!MatchesSupportedTypes(argumentValue.GetType()))
                {
                    throw new EvaluationException($"{GetTypeMismatchError(formulaName, argument)}. Expected String/Double/Int32/Boolean type.");
                }
            }
            else if (argument.Type != argumentValue.GetType())
            {
                throw new EvaluationException($"{GetTypeMismatchError(formulaName, argument)}. Expected {argument.Type.Name} type.");
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
                throw new EvaluationException("No context found to resolve custom name.");
            }

            var resolvedValue = _engine.Context.Resolve(customName.Value);

            if (resolvedValue == null)
                return resolvedValue;

            if(resolvedValue is int || resolvedValue is byte)
            {
                resolvedValue = Convert.ToDouble(resolvedValue);
            }
            else if (resolvedValue is Array array)
            {
                object[] objArray = new object[array.Length];

                for(int index = 0; index < array.Length; index++)
                {
                    var arrayItem = array.GetValue(index);

                    if(arrayItem is int || arrayItem is byte)
                    {
                        objArray[index] = Convert.ToDouble(arrayItem);
                    }
                    else
                    {
                        objArray[index] = arrayItem;
                    }
                }

                resolvedValue = objArray;
            }

            return resolvedValue;
        }
        #endregion

        private string GetTypeMismatchError(string formulaName, FormulaArgument argument)
        {
            return $"Formula : '{formulaName}' - Argument ({argument.Name}) type does't match";
        }

        public static bool Compare(object left, string @operator, object right)
        {
            try
            {
                if (@operator == Tokens.EqualsTo)
                    return Comparer.Equals(left, right);

                if (@operator == Tokens.NotEquals)
                    return !Comparer.Equals(left, right);

                if (left is bool bool1 && right is bool bool2)
                {
                    switch (@operator)
                    {
                        case Tokens.AND:
                            return bool1 && bool2;

                        case Tokens.OR:
                            return bool1 || bool2;
                    }
                }

                if (left is double num1 && right is double num2)
                {
                    switch (@operator)
                    {
                        case Tokens.LessThan:
                            return num1 < num2;

                        case Tokens.GreaterThan:
                            return num1 > num2;

                        case Tokens.LessThanEqualsTo:
                            return num1 <= num2;

                        case Tokens.GreaterThanEqualsTo:
                            return num1 >= num2;
                    }
                }

                if (left is DateTime date1 && right is DateTime date2)
                {
                    switch (@operator)
                    {
                        case Tokens.LessThan:
                            return date1 < date2;

                        case Tokens.GreaterThan:
                            return date1 > date2;

                        case Tokens.LessThanEqualsTo:
                            return date1 <= date2;

                        case Tokens.GreaterThanEqualsTo:
                            return date1 >= date2;
                    }
                }

                throw new Exception();
            }
            catch
            {
                throw new EvaluationException($"Invalid operator used with operands. {left} {@operator} {right}");
            }
        }
    }
}

using AlphaX.Parserz;
using System;
using System.Collections.Generic;

namespace AlphaX.FormulaEngine
{
    internal class Evaluator
    {
        private AlphaXFormulaEngine _engine;

        public Evaluator(AlphaXFormulaEngine engine)
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
                formula.ValidateArguments(parsedArguments);
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
                throw new EvaluationException($"No context found to resolve custom name ({customName.Value}).");
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

        public bool Compare(object left, string @operator, object right)
        {
            try
            {
                if (@operator == _engine.Operator.EqualsTo)
                    return Equals(left, right);

                if (@operator == _engine.Operator.NotEquals)
                    return !Equals(left, right);

                if(@operator == _engine.Operator.AND)
                {
                    return (bool)left && (bool)right;
                }

                if (@operator == _engine.Operator.OR)
                {
                    return (bool)left || (bool)right;
                }

                if (@operator == _engine.Operator.LessThan)
                {
                    if (left is double num1 && right is double num2)
                    {
                        return num1 < num2;
                    }
                    else if(left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 < date2;
                    }
                }

                if (@operator == _engine.Operator.LessThanEqualsTo)
                {
                    if (left is double num1 && right is double num2)
                    {
                        return num1 <= num2;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 <= date2;
                    }
                }

                if (@operator == _engine.Operator.GreaterThan)
                {
                    if (left is double num1 && right is double num2)
                    {
                        return num1 > num2;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 > date2;
                    }
                }

                if (@operator == _engine.Operator.GreaterThanEqualsTo)
                {
                    if (left is double num1 && right is double num2)
                    {
                        return num1 >= num2;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
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

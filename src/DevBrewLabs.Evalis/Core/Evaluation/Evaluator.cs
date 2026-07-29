using DevBrewLabs.Evalis.Formulas;
using DevBrewLabs.Evalis.Resources;
using DevBrewLabs.Parserly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DevBrewLabs.Evalis
{
    internal class Evaluator : IEvaluator
    {
        private IFormulaStore _formulaStore;
        private static readonly ConditionalWeakTable<ArrayResult, IParserResult> _postfixCache = new ConditionalWeakTable<ArrayResult, IParserResult>();

        internal static bool TryGetOperatorPriority(string op, out int priority)
        {
            switch (op)
            {
                case ArithmeticOperator.Add:
                case ArithmeticOperator.Subtract:
                    priority = 2; return true;
                case ArithmeticOperator.Multiply:
                case ArithmeticOperator.Divide:
                case ArithmeticOperator.Modulo:
                    priority = 3; return true;
                default:
                    priority = 0; return false;
            }
        }

        internal LogicalOperator SupportedLogicalOperators { get; set; }

        public Evaluator(IFormulaStore formulaStore)
        {
            _formulaStore = formulaStore;
        }

        public Task<object> Evaluate(IParserResult result, IEngineContext context)
        {
            if (result is ArrayResult arrResult)
            {
                if (!_postfixCache.TryGetValue(arrResult, out var cachedPostfix))
                {
                    cachedPostfix = InfixToPostfix(arrResult.Normalize());
                    _postfixCache.Add(arrResult, cachedPostfix);
                }
                result = cachedPostfix;
            }

            if(result is ErrorResult errorResult)
            {
                return Task.FromResult<object>(EvaluationResult.WithError(Error.Syntax(errorResult.Message)));
            }

            if (result is ArrayResult)
            {
                return Evaluate(result, context);
            }

            if (result is FormulaResult formulaResult)
            {
                return EvaluateFormula(formulaResult, context);
            }

            if (result is CustomNameResult customNameResult)
            {
                return Resolve(customNameResult.Value, context);
            }

            if (result is OperatorResult opResult)
            {
                return EvaluateOperator(opResult, context);
            }

            if(result == null)
            {
                return Task.FromResult<object>(EvaluationResult.WithError(Error.Syntax("Expression is invalid.")));
            }

            return Task.FromResult<object>(result.Value);
        }

        private async Task<object> EvaluateFormula(FormulaResult result, IEngineContext context)
        {
            var formulaName = result.Value.Name;

            if (!(_formulaStore as FormulaStore).TryGet(formulaName, out var formula))
            {
                return EvaluationResult.WithError(Error.Name($"Invalid formula '{formulaName}'"));
            }

            var args = result.Value.Args;

            if (args.Length > formula.Info.MaxArgsCount || args.Length < formula.Info.MinArgsCount)
            {
                return EvaluationResult.WithError(Error.Value(string.Format(
                    FormulaResources.InvalidArgumentCount,
                    formula.Info.MinArgsCount,
                    formula.Info.MaxArgsCount)));
            }

            var tasks = new Task<object>[args.Length];
            bool allCompleted = true;

            for (int i = 0; i < args.Length; i++)
            {
                var task = Evaluate(args[i], context);
                tasks[i] = task;
                if (!task.IsCompleted)
                {
                    allCompleted = false;
                }
            }

            if (!allCompleted)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            
            // Materialize results directly
            var arguments = new object[args.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                var argResult = tasks[i].Result;
                if (argResult is IEvaluationResult evalResult)
                {
                    if (evalResult.Error != null)
                    {
                        if (!formula.HandlesErrors) return evalResult;
                        arguments[i] = evalResult;
                    }
                    else
                    {
                        arguments[i] = evalResult.Value;
                    }
                }
                else
                {
                    arguments[i] = argResult;
                }
            }

            try
            {
                FormulaContext formulaContext = new FormulaContext(arguments)
                {
                    Evaluator = this
                };

                if (formula.IsAsync)
                {
                    return await (formula as AsyncFormula).EvaluateAsync(formulaContext);
                }
                else
                {
                    return (formula as Formula).Evaluate(formulaContext);
                }
            }
            catch (Exception ex)
            {
                return EvaluationResult.WithError(Error.General($"Failed to evaluate '{formula.Name}' formula. {ex.Message}"));
            }
        }

        private async Task<object> EvaluateOperator(OperatorResult result, IEngineContext context)
        {
            var left = Evaluate(result.Children[0], context);
            var right = Evaluate(result.Children[1], context);

            if (!left.IsCompleted || !right.IsCompleted)
            {
                await Task.WhenAll(left, right).ConfigureAwait(false);
            }
            var leftVal = left.Result;
            if (leftVal is IEvaluationResult leftRes)
            {
                if (leftRes.Error != null) return leftRes;
                leftVal = leftRes.Value;
            }

            var rightVal = right.Result;
            if (rightVal is IEvaluationResult rightRes)
            {
                if (rightRes.Error != null) return rightRes;
                rightVal = rightRes.Value;
            }

            string @operator = result.Value;

            switch (result.Value)
            {
                case ArithmeticOperator.Add:
                    {
                        var leftOp = EvalisUtil.AsDouble(leftVal);
                        var rightOp = EvalisUtil.AsDouble(rightVal);
                        if (leftOp.HasValue && rightOp.HasValue)
                            return EvaluationResult.WithValue(leftOp.Value + rightOp.Value);
                    }
                    return EvaluationResult.WithError(Error.Value($"Invalid operator used with operands. '{leftVal} {@operator} {rightVal}'."));

                case ArithmeticOperator.Subtract:
                    {
                        var leftOp = EvalisUtil.AsDouble(leftVal);
                        var rightOp = EvalisUtil.AsDouble(rightVal);
                        if (leftOp.HasValue && rightOp.HasValue)
                            return EvaluationResult.WithValue(leftOp.Value - rightOp.Value);
                    }
                    return EvaluationResult.WithError(Error.Value($"Invalid operator used with operands. '{leftVal} {@operator} {rightVal}'."));

                case ArithmeticOperator.Divide:
                    {
                        var leftOp = EvalisUtil.AsDouble(leftVal);
                        var rightOp = EvalisUtil.AsDouble(rightVal);
                        if (leftOp.HasValue && rightOp.HasValue)
                        {
                            if (rightOp.Value == 0) return EvaluationResult.WithError(Error.DivideByZero);
                            return EvaluationResult.WithValue(leftOp.Value / rightOp.Value);
                        }
                    }
                    return EvaluationResult.WithError(Error.Value($"Invalid operator used with operands. '{leftVal} {@operator} {rightVal}'."));

                case ArithmeticOperator.Multiply:
                    {
                        var leftOp = EvalisUtil.AsDouble(leftVal);
                        var rightOp = EvalisUtil.AsDouble(rightVal);
                        if (leftOp.HasValue && rightOp.HasValue)
                            return EvaluationResult.WithValue(leftOp.Value * rightOp.Value);
                    }
                    return EvaluationResult.WithError(Error.Value($"Invalid operator used with operands. '{leftVal} {@operator} {rightVal}'."));

                case ArithmeticOperator.Modulo:
                    {
                        var leftOp = EvalisUtil.AsDouble(leftVal);
                        var rightOp = EvalisUtil.AsDouble(rightVal);
                        if (leftOp.HasValue && rightOp.HasValue)
                            return EvaluationResult.WithValue(leftOp.Value % rightOp.Value);
                    }
                    return EvaluationResult.WithError(Error.Value($"Invalid operator used with operands. '{leftVal} {@operator} {rightVal}'."));

                default:
                    bool? comparisonResult = EvalisUtil.Compare(leftVal, result.Value, rightVal, SupportedLogicalOperators);

                    if (comparisonResult.HasValue)
                    {
                        return EvaluationResult.WithValue(comparisonResult.Value);
                    }
                    else
                    {
                        return EvaluationResult.WithError(Error.Value($"Invalid operator/operands used in expression. '{leftVal} {@operator} {rightVal}'."));
                    }
            }
        }

        internal static IParserResult InfixToPostfix(ArrayResult infixResult)
        {
            var openBracketResult = new OpenBracketResult();
            var closeBracketResult = new CloseBracketResult();

            int openBrackets = 0;
            int closedBrackets = 0;
            int length = infixResult.Value.Length;
            var reverse = new IParserResult[length];
            for (int i = 0; i < length; i++)
            {
                var x = infixResult.Value[length - 1 - i];
                if (x is OpenBracketResult)
                {
                    openBrackets++;
                    reverse[i] = closeBracketResult;
                }
                else if (x is CloseBracketResult)
                {
                    closedBrackets++;
                    reverse[i] = openBracketResult;
                }
                else
                {
                    reverse[i] = x;
                }
            }

            if (openBrackets != closedBrackets)
            {
                return new ErrorResult("Mismatched brackets in expression.");
            }

            var operatorStack = new Stack<IParserResult>(length);
            var outputList = new List<IParserResult>(length);

            foreach (var cur in reverse)
            {
                if (cur is CloseBracketResult)
                {
                    var op = operatorStack.Count > 0 ? operatorStack.Pop() : null;

                    while (op != null && op.Type != openBracketResult.Type)
                    {
                        outputList.Add(op);
                        op = operatorStack.Count > 0 ? operatorStack.Pop() : null;
                    }
                }
                else if (cur is OpenBracketResult)
                {
                    operatorStack.Push(cur);
                }
                else if (cur is OperatorResult opResult)
                {
                    opResult.Children.Clear();
                    int c = operatorStack.Count;
                    // stack is empty, push operator
                    if (c == 0)
                    {
                        operatorStack.Push(cur);
                    }
                    else
                    {
                        var lastOperator = operatorStack.Peek();
                        int opPriority;
                        bool hasPriority = TryGetOperatorPriority(opResult.Value, out opPriority);
                        int lastOpPriority = 0;
                        if (lastOperator is OperatorResult lastOpTemp)
                            TryGetOperatorPriority(lastOpTemp.Value, out lastOpPriority);

                        if (lastOperator is OpenBracketResult || !hasPriority || opPriority > lastOpPriority)
                        {
                            operatorStack.Push(cur);
                        }
                        else
                        {
                            while (lastOperator != null &&
                                lastOperator is OperatorResult lastOpResult &&
                                TryGetOperatorPriority(lastOpResult.Value, out lastOpPriority) &&
                                lastOpPriority > opPriority)
                            {
                                outputList.Add(lastOperator);
                                operatorStack.Pop();
                                lastOperator = operatorStack.Count > 0 ? operatorStack.Peek() : null;
                            }

                            operatorStack.Push(cur);
                        }
                    }
                }
                else
                {
                    outputList.Add(cur);
                }
            }

            while (operatorStack.Count > 0)
            {
                outputList.Add(operatorStack.Pop());
            }

            outputList.Reverse();


            var pendingNodes = new Stack<IParserResult>(length);
            IParserResult root = null;

            for (var i = 0; i < outputList.Count; i++)
            {
                if (root == null)
                {
                    root = outputList[i];
                }

                if (pendingNodes.Count > 0)
                {
                    var lastPending = pendingNodes.Peek() as OperatorResult;
                    lastPending.Children.Add(outputList[i]);
                    if (lastPending.Children != null && lastPending.Children.Count == 2)
                    {
                        pendingNodes.Pop();
                    }
                }

                if (outputList[i] is OperatorResult)
                {
                    pendingNodes.Push(outputList[i]);
                }
            }

            if (pendingNodes.Count > 0)
            {
                return new ErrorResult("Mismatched brackets in expression.");
            }

            return root;
        }

        #region Resolver
        public async Task<object> Resolve(CustomName customName, IEngineContext context = null)
        {
            if (context == null)
            {
                return EvaluationResult.WithError(Error.Name($"No context found to resolve custom name ({customName.Value})."));
            }

            var resolvedValue = await context.Resolve(customName.Value);

            if (resolvedValue == null)
                return resolvedValue;

            return NormalizeValue(resolvedValue);
        }

        private static object NormalizeValue(object value)
        {
            if (value is Array array)
            {
                var normalized = new object[array.Length];
                for (int i = 0; i < array.Length; i++)
                    normalized[i] = NormalizeValue(array.GetValue(i));
                return normalized;
            }

            return value;
        }
        #endregion
    }
}

using System;
using System.Collections;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class IFFormula : Formula
    {
        public IFFormula() : base("IF")
        {

        }

        public override object Evaluate(params object[] args)
        {
            if (args.Length == 3)
            {
                var condition = args[0] as Condition;
                var result = Compare(condition);

                if (result)
                {
                    return args[1];
                }
                else
                {
                    return args[2];
                }
            }
            else
            {
                return string.Empty;
            }
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

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo("checks a given condition and returns a particular value if it is TRUE. " +
                "It will return another value if the condition is FALSE",
                new FormulaArgument("condition", typeof(string), true, 0, "condition to check"),
                new FormulaArgument("value1", typeof(string), true, 1, "value if true"),
                new FormulaArgument("value2", typeof(string), true, 2, "value if false"));
        }
    }
}

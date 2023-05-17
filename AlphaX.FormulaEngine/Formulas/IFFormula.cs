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
                var condition = (Condition)args[0];
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
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new ConditionArgument("condition", true));
            info.AddArgument(new ObjectArgument("value1", true));
            info.AddArgument(new ObjectArgument("value2", true));
            return info;
        }
    }
}

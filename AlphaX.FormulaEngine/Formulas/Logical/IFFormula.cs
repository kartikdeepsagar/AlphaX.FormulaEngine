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
                var condition = (bool)args[0];
                if (condition)
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
                return "#ERROR";
            }
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Checks whether condition is met. Returns first value if true and return second value if false."
            };
            info.AddArgument(new BooleanArgument("condition", true)
            {
                Description = "Condition to evaluate."
            });
            info.AddArgument(new ObjectArgument("value1", true)
            {
                Description = "Value to return if condition is true."
            });
            info.AddArgument(new ObjectArgument("value2", true)
            {
                Description = "Value to return if condition is false."
            });
            return info;
        }
    }
}

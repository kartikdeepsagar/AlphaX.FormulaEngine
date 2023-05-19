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
                return false;
            }
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new BooleanArgument("condition", true));
            info.AddArgument(new ObjectArgument("value1", true));
            info.AddArgument(new ObjectArgument("value2", true));
            return info;
        }
    }
}

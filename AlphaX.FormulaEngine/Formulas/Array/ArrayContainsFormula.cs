using System;
using System.Collections;

namespace AlphaX.FormulaEngine.Formulas
{
    public class ArrayContainsFormula : Formula
    {
        public ArrayContainsFormula() : base("ARRAYCONTAINS")
        {
        }

        public override object Evaluate(params object[] args)
        {
            var sourceArray = (object[])args[0];
            var targetItem = args[1];
            return Array.Exists(sourceArray, x => Comparer.Equals(x, targetItem));
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new ArrayArgument("source", true));
            info.AddArgument(new ObjectArgument("value2", true));
            return info;
        }
    }
}

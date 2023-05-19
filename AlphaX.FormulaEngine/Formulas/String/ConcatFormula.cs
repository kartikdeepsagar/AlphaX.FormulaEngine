using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class ConcatFormula : Formula
    {
        public ConcatFormula() : base("CONCAT")
        {
        }

        public override object Evaluate(params object[] args)
        {
            return string.Concat(args);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new ArrayArgument("values", true));
            return info;
        }
    }
}

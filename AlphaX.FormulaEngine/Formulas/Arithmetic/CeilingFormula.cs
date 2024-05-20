using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class CeilingFormula : Formula
    {
        public CeilingFormula() : base("CEILING") { }

        public override object Evaluate(params object[] args)
        {
            double value = (double)args[0];
            return Math.Ceiling(value);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new DoubleArgument("value", true));
            return info;
        }
    }
}

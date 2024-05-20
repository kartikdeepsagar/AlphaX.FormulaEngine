using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class FloorFormula : Formula
    {
        public FloorFormula() : base("FLOOR") { }

        public override object Evaluate(params object[] args)
        {
            double value = (double)args[0];
            return Math.Floor(value);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new DoubleArgument("value", true));
            return info;
        }
    }
}

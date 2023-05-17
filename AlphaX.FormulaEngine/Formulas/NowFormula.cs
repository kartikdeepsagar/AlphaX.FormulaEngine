using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class NowFormula : Formula
    {
        public NowFormula() : base("NOW")
        {
        }

        public override object Evaluate(params object[] args)
        {
            return DateTime.Now.ToString();
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo();
        }
    }
}

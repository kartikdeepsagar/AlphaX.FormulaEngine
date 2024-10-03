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
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Returns the smallest integral value that is greater than or equal to the specified decimal number."
            };
            info.AddArgument(new DoubleArgument("value", true)
            {
                Description = "A decimal number."
            });
            return info;
        }
    }
}

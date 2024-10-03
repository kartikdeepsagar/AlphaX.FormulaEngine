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
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Concatenate all the values present in the provided array."
            };
            info.AddArgument(new ArrayArgument("values", true)
            {
                Description = "Array to concatenate."
            });
            return info;
        }
    }
}

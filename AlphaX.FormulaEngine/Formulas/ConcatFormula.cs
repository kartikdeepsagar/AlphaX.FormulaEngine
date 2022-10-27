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
            return new FormulaInfo("Joines/merges multiple text strings into one text string", 1, int.MaxValue,
                new FormulaArgument("values", typeof(string[]), true, 0, "Array of string values"));
        }
    }
}

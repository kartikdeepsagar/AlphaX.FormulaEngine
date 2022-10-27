namespace AlphaX.FormulaEngine.Formulas
{
    internal class LowerFormula : Formula
    {
        public LowerFormula() : base("LOWER") { }

        public override object Evaluate(params object[] args)
        {
            if (args.Length > 0)
                return args[0].ToString().ToLowerInvariant();
            else
                return string.Empty;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo("Converts all letters in a text string to lowercase.",
                new FormulaArgument("value", typeof(string), true, 0, "String to convert."));
        }
    }
}
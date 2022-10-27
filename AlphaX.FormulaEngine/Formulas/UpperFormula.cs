namespace AlphaX.FormulaEngine.Formulas
{
    internal class UpperFormula : Formula
    {
        public UpperFormula() : base("UPPER") { }

        public override object Evaluate(params object[] args)
        {
            if (args.Length > 0)
                return args[0].ToString().ToUpperInvariant();
            else
                return string.Empty;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo("Converts all letters in a text string to uppercase.",
                new FormulaArgument("value", typeof(string), true, 0, "String to convert."));
        }
    }
}

namespace AlphaX.FormulaEngine.Formulas
{
    public class LowerFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public LowerFormula() : base("LOWER", 1, 1)
        {
            Info = new FormulaInfo("Converts all letters in a text string to lowercase.",
                new FormulaArgument("value", typeof(string), true, 0, "String to convert."));
        }

        public override object Evaluate(params object[] args)
        {
            if (args.Length > 0)
                return args[0].ToString().ToLowerInvariant();
            else
                return string.Empty;
        }
    }
}
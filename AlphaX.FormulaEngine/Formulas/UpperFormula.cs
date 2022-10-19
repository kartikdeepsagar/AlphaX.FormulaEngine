namespace AlphaX.FormulaEngine.Formulas
{
    public class UpperFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public UpperFormula() : base("UPPER", 1, 1)
        {
            Info = new FormulaInfo("Converts all letters in a text string to uppercase.",
                new FormulaArgument("value", typeof(string), true, 0, "String to convert."));
        }

        public override object Evaluate(params object[] args)
        {
            if (args.Length > 0)
                return args[0].ToString().ToUpperInvariant();
            else
                return string.Empty;
        }
    }
}

namespace AlphaX.FormulaEngine.Formulas
{
    public class LowerFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public LowerFormula() : base("LOWER", 1, 1)
        {
            Info = new FormulaInfo("",
                new FormulaArgument("value", typeof(string), true, 0, ""));
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
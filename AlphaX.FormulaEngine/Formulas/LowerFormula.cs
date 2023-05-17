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
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new StringArgument("value", true));
            return info;
        }
    }
}
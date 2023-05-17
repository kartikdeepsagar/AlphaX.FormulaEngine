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
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new StringArgument("value", true));
            return info;
        }
    }
}

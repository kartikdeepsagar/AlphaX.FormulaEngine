namespace AlphaX.FormulaEngine.Formulas
{
    internal class EndsWithFormula : Formula
    {
        public EndsWithFormula() : base("ENDSWITH") { }

        public override object Evaluate(params object[] args)
        {
            var source = args[0].ToString();
            var value = args[1].ToString();
            var matchCase = args.Length == 3 ? (bool)args[2] : false;
            return source.EndsWith(value, matchCase ? System.StringComparison.Ordinal : System.StringComparison.InvariantCultureIgnoreCase);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new StringArgument("source", true));
            info.AddArgument(new StringArgument("value", true));
            info.AddArgument(new BooleanArgument("matchCase", false));
            return info;
        }
    }
}

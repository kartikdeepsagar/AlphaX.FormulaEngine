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
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Checks if the provided string ends with the speicifed value."
            };
            info.AddArgument(new StringArgument("source", true)
            {
                Description = "The source string."
            });
            info.AddArgument(new StringArgument("value", true)
            {
                Description = "The value to check for."
            });
            info.AddArgument(new BooleanArgument("matchCase", false)
            {
                Description = "Match case while checking."
            });
            return info;
        }
    }
}

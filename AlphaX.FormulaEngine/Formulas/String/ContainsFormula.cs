namespace AlphaX.FormulaEngine.Formulas
{
    internal class ContainsFormula : Formula
    {
        public ContainsFormula() : base("CONTAINS") { }

        public override object Evaluate(params object[] args)
        {
            var source = args[0].ToString();
            var value = args[1].ToString();
            return source.Contains(value);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Checks if the provided string contains the speicifed value."
            };
            info.AddArgument(new StringArgument("source", true)
            {
                Description = "The source string."
            });
            info.AddArgument(new StringArgument("value", true)
            {
                Description = "The value to check for."
            });
            return info;
        }
    }
}

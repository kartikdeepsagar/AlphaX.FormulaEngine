namespace AlphaX.FormulaEngine.Formulas
{
    internal class UpperFormula : StringFormula
    {
        public UpperFormula() : base("UPPER") { }

        protected override object EvaluateString(string value)
        {
            return value?.ToUpperInvariant();
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Returns the uppercase string."
            };
            info.AddArgument(new StringArgument("value", true)
            {
                Description = "The value to convert."
            });

            return info;
        }
    }
}
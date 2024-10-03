namespace AlphaX.FormulaEngine.Formulas
{
    internal class LowerFormula : StringFormula
    {
        public LowerFormula() : base("LOWER") { }

        protected override object EvaluateString(string value)
        {
            return value?.ToLowerInvariant();
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Returns the lowercase string."
            };
            info.AddArgument(new StringArgument("value", true)
            {
                Description = "The value to convert."
            });

            return info;
        }
    }
}
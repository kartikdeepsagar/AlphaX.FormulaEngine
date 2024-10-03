namespace AlphaX.FormulaEngine.Formulas
{
    internal class LengthFormula : StringFormula
    {
        public LengthFormula() : base("LENGTH")
        {
        }

        protected override object EvaluateString(string value)
        {
            return value?.Length;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Returns the length of string."
            };
            info.AddArgument(new StringArgument("value", true)
            {
                Description = "The string value to check length."
            });

            return info;
        }
    }
}
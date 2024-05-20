namespace AlphaX.FormulaEngine.Formulas
{
    internal class UpperFormula : StringFormula
    {
        public UpperFormula() : base("UPPER") { }

        protected override object EvaluateString(string value)
        {
            return value?.ToUpperInvariant();
        }
    }
}
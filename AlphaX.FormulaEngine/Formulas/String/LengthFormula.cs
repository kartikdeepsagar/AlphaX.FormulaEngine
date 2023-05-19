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
    }
}
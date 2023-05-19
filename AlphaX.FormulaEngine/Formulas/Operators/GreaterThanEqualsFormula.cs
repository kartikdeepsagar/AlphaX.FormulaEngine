namespace AlphaX.FormulaEngine.Formulas
{
    internal class GreaterThanEqualsFormula : OperatorFormula
    {
        public GreaterThanEqualsFormula() : base("GREATERTHANEQUALS")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.GreaterThanEqualsTo;
        }
    }
}

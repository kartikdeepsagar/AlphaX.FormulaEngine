namespace AlphaX.FormulaEngine.Formulas
{
    internal class LessThanEqualsFormula : OperatorFormula
    {
        public LessThanEqualsFormula() : base("LESSTHANEQUALS")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.LessThanEqualsTo;
        }
    }
}

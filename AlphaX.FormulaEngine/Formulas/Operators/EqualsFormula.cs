namespace AlphaX.FormulaEngine.Formulas
{
    internal class EqualsFormula : OperatorFormula
    {
        public EqualsFormula() : base("EQUALS")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.EqualsTo;
        }
    }
}

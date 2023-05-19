namespace AlphaX.FormulaEngine.Formulas
{
    internal class AndFormula : OperatorFormula
    {
        public AndFormula() : base("AND")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.AND;
        }
    }
}

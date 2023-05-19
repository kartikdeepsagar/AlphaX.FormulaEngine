namespace AlphaX.FormulaEngine.Formulas
{
    internal class GreaterThanFormula : OperatorFormula
    {
        public GreaterThanFormula() : base("GREATERTHAN")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.GreaterThan;
        }
    }
}

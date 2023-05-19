namespace AlphaX.FormulaEngine.Formulas
{
    internal class LessThanFormula : OperatorFormula
    {
        public LessThanFormula() : base("LESSTHAN")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.LessThan;
        }
    }
}

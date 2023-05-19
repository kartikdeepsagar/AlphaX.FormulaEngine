using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class OrFormula : OperatorFormula
    {
        public OrFormula() : base("OR")
        {
        }

        protected override string GetOperator()
        {
            return Tokens.OR;
        }
    }
}
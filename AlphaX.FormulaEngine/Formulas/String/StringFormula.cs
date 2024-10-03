namespace AlphaX.FormulaEngine.Formulas
{
    internal abstract class StringFormula : Formula
    {
        protected StringFormula(string name) : base(name)
        {
        }

        public override object Evaluate(params object[] args)
        {
            return EvaluateString(args[0].ToString());
        }

        protected abstract object EvaluateString(string value);
    }
}

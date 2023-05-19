namespace AlphaX.FormulaEngine.Formulas
{
    internal class NotFormula : Formula
    {
        public NotFormula() : base("NOT")
        {
        }

        public override object Evaluate(params object[] args)
        {
            return !(bool)args[0];
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new BooleanArgument("value1", true));
            return info;
        }
    }
}

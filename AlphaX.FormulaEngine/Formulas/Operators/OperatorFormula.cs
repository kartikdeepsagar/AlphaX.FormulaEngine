namespace AlphaX.FormulaEngine.Formulas
{
    internal abstract class OperatorFormula : Formula
    {
        protected OperatorFormula(string name) : base(name)
        {
        }

        public override object Evaluate(params object[] args)
        {
            return Evaluator.Compare(args[0], GetOperator(), args[1]);
        }

        protected abstract string GetOperator();

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new ObjectArgument("value1", true));
            info.AddArgument(new ObjectArgument("value2", true));
            return info;
        }
    }
}

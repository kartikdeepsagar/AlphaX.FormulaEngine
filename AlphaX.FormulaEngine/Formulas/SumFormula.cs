namespace AlphaX.FormulaEngine.Formulas
{
    internal class SumFormula : Formula
    {
        public SumFormula() : base("SUM") { }

        public override object Evaluate(params object[] args)
        {
            double sum = 0d;

            for (int index = 0; index < args.Length; index++)
                sum += (double)args[index];

            return sum;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new DoubleArrayArgument("values", true));
            return info;
        }
    }
}

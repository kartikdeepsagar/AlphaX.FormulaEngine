namespace AlphaX.FormulaEngine.Formulas
{
    internal class AverageFormula : Formula
    {
        public AverageFormula() : base("AVERAGE") { }

        public override object Evaluate(params object[] args)
        {
            double sum = 0;

            for (int index = 0; index < args.Length; index++)
                sum += (double)args[index];

            return sum / args.Length;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new DoubleArrayArgument("values", true));
            return info;
        }
    }
}

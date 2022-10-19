namespace AlphaX.FormulaEngine.Formulas
{
    public class SumFormula : Formula
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
            return new FormulaInfo("Returns the sum of its arguments.", 1, int.MaxValue,
                new FormulaArgument("values", typeof(double[]), true, 0, "Array of numeric values"));
        }
    }
}

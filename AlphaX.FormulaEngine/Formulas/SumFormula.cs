namespace AlphaX.FormulaEngine.Formulas
{
    public class SumFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public SumFormula() : base("SUM", 1, int.MaxValue)
        {
            Info = new FormulaInfo("Returns the sum of its arguments.",
                new FormulaArgument("values", typeof(double[]), true, 0, "Array of numeric values"));
        }

        public override object Evaluate(params object[] args)
        {
            double sum = 0d;

            for (int index = 0; index < args.Length; index++)
                sum += (double)args[index];

            return sum;
        }
    }
}

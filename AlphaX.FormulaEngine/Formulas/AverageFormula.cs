namespace AlphaX.FormulaEngine.Formulas
{
    public class AverageFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public AverageFormula() : base("AVERAGE", 1, int.MaxValue)
        {
            Info = new FormulaInfo("Returns the average (arithmetic mean) of its arguments.", 
                new FormulaArgument("values", typeof(double[]), true, 0, "Array of numeric values."));
        }


        public override object Evaluate(params object[] args)
        {
            double sum = 0;

            for (int index = 0; index < args.Length; index++)
                sum += (double)args[index];

            return sum / args.Length;
        }
    }
}

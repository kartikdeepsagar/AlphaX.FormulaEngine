namespace AlphaX.FormulaEngine.Formulas
{
    internal class SumFormula : Formula
    {
        public SumFormula() : base("SUM") { }

        public override object Evaluate(params object[] args)
        {
            var values = (object[])args[0];
            double sum = 0d;

            for (int index = 0; index < values.Length; index++)
                sum += (double)values[index];

            return sum;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Returns sum of provided values"
            };
            info.AddArgument(new ArrayArgument("values", true)
            {
                Description = "Array of numeric values"
            });
            return info;
        }
    }
}

namespace AlphaX.FormulaEngine.Formulas
{
    internal class AverageFormula : Formula
    {
        public AverageFormula() : base("AVERAGE") { }

        public override object Evaluate(params object[] args)
        {
            var values = (object[])args[0];
            double sum = 0;

            for (int index = 0; index < values.Length; index++)
                sum += (double)values[index];

            return sum / values.Length;
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Returns average of provided values."
            };
            info.AddArgument(new ArrayArgument("values", true)
            {
                Description = "Array of numeric values."
            });
            return info;
        }
    }
}

namespace AlphaX.FormulaEngine.Formulas
{
    internal class NotFormula : Formula
    {
        public NotFormula() : base("NOT")
        {
        }

        public override object Evaluate(params object[] args)
        {
            try
            {
                return !(bool)args[0];
            }
            catch
            {
                return "#ERROR";
            }
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Inverse a boolean value."
            };
            info.AddArgument(new BooleanArgument("value1", true)
            {
                Description = "Value to inverse."
            });
            return info;
        }
    }
}

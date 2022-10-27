using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class NowFormula : Formula
    {
        public NowFormula() : base("NOW")
        {
        }

        public override object Evaluate(params object[] args)
        {
            return DateTime.Now.ToString();
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo("Gets current system date and time");
        }
    }

    internal class IFFormula : Formula
    {
        public IFFormula() : base("IF")
        {

        }

        public override object Evaluate(params object[] args)
        {
            throw new NotImplementedException();
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo("checks a given condition and returns a particular value if it is TRUE. " +
                "It will return another value if the condition is FALSE",
                new FormulaArgument("condition", typeof(string), true, 0, "condition to check"),
                new FormulaArgument("value1", typeof(string), true, 1, "value if true"),
                new FormulaArgument("value2", typeof(string), true, 2, "value if false"));
        }
    }
}

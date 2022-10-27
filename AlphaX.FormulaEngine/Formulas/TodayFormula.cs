using System;
using System.Globalization;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class TodayFormula : Formula
    {
        public TodayFormula() : base("TODAY")
        {
        }

        public override object Evaluate(params object[] args)
        {
            return DateTime.Now.Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            return new FormulaInfo("Gets current system date");
        }
    }
}

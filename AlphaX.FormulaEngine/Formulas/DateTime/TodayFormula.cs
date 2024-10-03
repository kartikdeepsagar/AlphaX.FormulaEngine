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
            try
            {
                if (args.Length > 0)
                {
                    return DateTime.Now.Date.ToString(args[0].ToString());
                }

                return DateTime.Now.Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
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
                Description = "Returns system current date."
            };
            info.AddArgument(new StringArgument("format", false)
            {
                Description = "Format string."
            });
            return info;
        }
    }
}

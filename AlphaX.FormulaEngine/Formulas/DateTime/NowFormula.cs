using System;
using System.Globalization;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class NowFormula : Formula
    {
        public NowFormula() : base("NOW")
        {
        }

        public override object Evaluate(params object[] args)
        {
            try
            {
                if(args.Length > 0)
                {
                    return DateTime.Now.ToString(args[0].ToString());
                }

                return DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);
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
                Description = "Returns system current date time."
            };
            info.AddArgument(new StringArgument("format", false)
            {
                Description = "Format string."
            });
            return info;
        }
    }
}

using System.Text.RegularExpressions;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class RegexMatchFormula : Formula
    {
        public RegexMatchFormula() : base("REGEXMATCH")
        {
            
        }

        public override object Evaluate(params object[] args)
        {
            Regex regex = new Regex(args[0].ToString());
            return regex.IsMatch(args[1].ToString());
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Searches the input string for the first occurence of regular expression."
            };

            info.AddArgument(new StringArgument("pattern", true)
            {
                Description = "Pattern to match."
            });

            info.AddArgument(new StringArgument("value", true)
            {
                Description = "Input value."
            });
            return info;
        }
    }
}

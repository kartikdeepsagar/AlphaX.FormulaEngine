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
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new StringArgument("pattern", true));
            info.AddArgument(new StringArgument("value", true));
            return info;
        }
    }
}

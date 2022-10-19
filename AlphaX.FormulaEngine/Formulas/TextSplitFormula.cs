using System;

namespace AlphaX.FormulaEngine.Formulas
{
    public class TextSplitFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public TextSplitFormula() : base("TEXTSPLIT", 2, 2)
        {
            Info = new FormulaInfo("Splits a text string into substrings using the provided seperator.",
                new FormulaArgument("seperator", typeof(string), true, 0, "A string that delimits the substrings in this string."),
                new FormulaArgument("value", typeof(string), true, 1, "String to split."));
        }

        public override object Evaluate(params object[] args)
        {
            return args[1].ToString().Split(new string[] { args[0].ToString() }, StringSplitOptions.None);
        }
    }
}

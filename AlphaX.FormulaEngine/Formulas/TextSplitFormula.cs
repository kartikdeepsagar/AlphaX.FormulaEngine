using System;

namespace AlphaX.FormulaEngine.Formulas
{
    public class TextSplitFormula : Formula
    {
        public override FormulaInfo Info { get; }

        public TextSplitFormula() : base("TEXTSPLIT", 2, 2)
        {
            Info = new FormulaInfo("",
                new FormulaArgument("seperator", typeof(string), true, 0, ""),
                new FormulaArgument("value", typeof(string), true, 1, ""));
        }

        public override object Evaluate(params object[] args)
        {
            return args[1].ToString().Split(new string[] { args[0].ToString() }, StringSplitOptions.None);
        }
    }
}

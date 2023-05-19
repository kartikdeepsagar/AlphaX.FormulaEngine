using System;

namespace AlphaX.FormulaEngine.Formulas
{
    internal class TextSplitFormula : Formula
    {
        public TextSplitFormula() : base("TEXTSPLIT") { }

        public override object Evaluate(params object[] args)
        {
            return args[1].ToString().Split(new string[] { args[0].ToString() }, StringSplitOptions.None);
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            FormulaInfo info = new FormulaInfo();
            info.AddArgument(new StringArgument("seperator", true));
            info.AddArgument(new StringArgument("value", true));
            return info;
        }
    }
}

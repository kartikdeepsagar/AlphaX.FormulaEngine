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
           FormulaInfo info = new FormulaInfo(Name)
            {
                Description = "Splits the input string into an array using the provided delimiter."
            };

            info.AddArgument(new StringArgument("separator", true)
            {
                Description = "The delimiter to use for string split."
            });

            info.AddArgument(new StringArgument("value", true)
            {
                Description = "The input string."
            });

            return info;
        }
    }
}
